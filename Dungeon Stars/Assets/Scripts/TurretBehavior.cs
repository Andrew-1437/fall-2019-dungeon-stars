using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    public GameObject projectile;
    private GameObject target;

    public float shootDelay;
    public float fireRate;
    public float burstTime;
    private float burstEnd;
    private float nextBurst;
    private float nextFire;

    public float turn;

    public Transform hardpoint;

    private bool awake;

    ObstacleBehavior thisObstacle;  // Reference to this gameObject's ObstacleBehavior script

    // Use this for initialization
    void Start () {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;

        thisObstacle = GetComponent<ObstacleBehavior>();

        if (hardpoint == null)
        {
            hardpoint = transform;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        awake = thisObstacle.awake;

        if (awake)
        {
            target = FindClosestByTag("Player");
            if (target != null)
            {
                Vector3 targetDir = target.transform.position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime);
            }
        }

        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
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
        Instantiate(projectile, hardpoint.position, hardpoint.rotation);
        nextFire = Time.time + fireRate;
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
