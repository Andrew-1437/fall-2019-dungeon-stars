using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject projectile;

    public float shootDelay;
    public float fireRate;
    public float burstTime;
    private float burstEnd;
    private float nextBurst;
    private float nextFire;

    public bool awake;

    // Use this for initialization
    void Start () {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Destroy(
                    Instantiate(projectile, transform.position, transform.rotation), 10f);
                nextFire = Time.time + fireRate;
            }

        }
    }

    public void SpawnSingle()
    {
        Destroy(
            Instantiate(projectile, transform.position, transform.rotation), 10f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //print(other.tag);
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }
}
