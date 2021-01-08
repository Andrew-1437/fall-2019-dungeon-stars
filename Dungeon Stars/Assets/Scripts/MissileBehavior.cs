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
        target = FindClosestByTag(targetTag);

        // If the target is the player and they are playing the "Vector Hunter" stealth ship, the missile's turn speed is halved
        if (targetTag == "Player" && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
            turnSpeedMod = .5f;
        else
            turnSpeedMod = 1f;
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void Update()
    {

        
        if (target != null)
        {
            // If we have a target, turn towards it
            Vector3 targetDir = target.GetComponent<Transform>().position - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime * turnSpeedMod);
        }
        else 
        { 
            // Acquire new target
            target = FindClosestByTag(targetTag);
            if (targetTag == "Player" && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                turnSpeedMod = .5f;
            else
                turnSpeedMod = 1f;
        }

        if (Time.time >= deathTime)
        {
            DestroyProjectile();
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            Detonate();
        }
        if (gameObject.tag == "EnemyMissile" && other.tag == "AntiProjectile")
        {
            DestroyProjectile();
        }
    }

    public void Detonate()
    {
        DestroyProjectile();
        Instantiate(explosion, transform.position, transform.rotation);
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
