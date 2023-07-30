using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : ProjectileBehavior {
    
    public float turn; //Turning speed of projectile
    public string targetTag;
    private GameObject target;

    public GameObject explosion;

    float turnSpeedMod = 1f;
    

    private void Start()
    {
        // Acquire target when instantiated
        base.Start();
        if (targetTag != "")
        {
            target = FindClosestByTag(targetTag);
        }
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();

        // Only do missile tracking if missile can actually turn to a target
        if (turn > 0 && targetTag != "")
        {
            if (target != null)
            {
                if (targetTag == "Player" && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .5f;
                else
                    turnSpeedMod = 1f;

                // If we have a target, turn towards it
                Vector3 targetDir = target.GetComponent<Transform>().position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.fixedDeltaTime * turnSpeedMod);
            }
            else
            {
                // Acquire new target
                target = FindClosestByTag(targetTag);
            }
        }

        if (Time.time >= deathTime)
        {
            DestroyProjectile();
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "EnemyMissile" && other.tag == "AntiProjectile")
        {
            DestroyProjectile();
        }
        if (other.tag == "Wall")
        {
            Detonate();
        }
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(ObstacleBehavior target)
    {
        Detonate();
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(BossBehavior target)
    {
        Detonate();
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(PlayerController target)
    {
        Detonate();
    }

    /// <summary>
    /// Detonates the missile and instatiates it's explosion object. 
    /// The explosion object does the damage.
    /// </summary>
    public void Detonate()
    {
        DestroyProjectile();
        // Clean up payload if it doesn't clean itself up
        Destroy(
            Instantiate(explosion, transform.position, transform.rotation),5);
    }

    GameObject FindClosestByTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (tag == "Player")
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            else
            {
                if (go.GetComponent<ObstacleBehavior>().awake)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < distance)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }
}
