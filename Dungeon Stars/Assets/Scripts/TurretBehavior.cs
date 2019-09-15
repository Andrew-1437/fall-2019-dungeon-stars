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

    private bool awake;

    // Use this for initialization
    void Start () {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (target != null)
        {
            Vector3 targetDir = target.GetComponent<Transform>().position - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime);
        }
        else
        {
            target = GameObject.FindWithTag("Player");
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
                Instantiate(projectile, transform.position, transform.rotation);
                nextFire = Time.time + fireRate;
            }

        }
        
        if (GetComponentInParent<ObstacleBehavior>().awake)
        {
            awake = true;
        }
        
        
    }


}
