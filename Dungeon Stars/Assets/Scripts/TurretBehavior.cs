using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    [Tooltip("Leave empty or specify \"Player\" to target player")]
    public string targetTag;    // Default target is player
    public GameObject projectile;
    private GameObject target;

    public float shootDelay;
    public float fireRate;
    public float burstTime;
    float burstEnd = 0f;
    float nextBurst = Mathf.Infinity;
    float nextFire = Mathf.Infinity;

    public float turn;
    float turnSpeedMod = 1f;

    public Transform hardpoint;

    public bool awake;
    bool wasSleeping;

    public bool ignoreObstacle;     // If true, the turret will not care about an attached ObstacleBehavior script
                                    // Mainly used for player's turrets or invulnerable turrets that should not be targeted
    ObstacleBehavior thisObstacle;  // Reference to this gameObject's ObstacleBehavior script

    // Use this for initialization
    void Start () {
        nextBurst = Time.time + shootDelay;
        nextFire = 0f;
        bool wasSleeping = !awake;

        if (targetTag == "") targetTag = "Player";  // If targetTag is empty, target player

        if (!ignoreObstacle)
            thisObstacle = GetComponent<ObstacleBehavior>();

        if (hardpoint == null)
        {
            hardpoint = transform;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!ignoreObstacle)
            awake = thisObstacle.awake;

        if(awake && wasSleeping)
        {
            wasSleeping = false;
            Awaken();
        }
        else if (!awake && !wasSleeping)
        {
            wasSleeping = true;
        }

        if (awake)
        {
            target = FindClosestByTag(targetTag);
            // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
            if (targetTag == "Player" && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                turnSpeedMod = .35f;
            else
                turnSpeedMod = 1f;
            if (target != null)
            {
                Vector3 targetDir = target.transform.position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime * turnSpeedMod);
            }
        }

        if (awake && target != null && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Fire();
            }

        }
        
        
    }

    public void Fire()
    {
        Destroy(
            Instantiate(projectile, hardpoint.position, hardpoint.rotation), 
            5f);
        nextFire = Time.time + fireRate * OmniController.omniController.obstacleSpeedScale;
    }

    public void Awaken()
    {
        awake = true;
        nextBurst = Time.time + burstTime;
        nextFire = 0.0f;
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
