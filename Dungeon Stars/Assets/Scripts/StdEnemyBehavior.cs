using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StdEnemyBehavior : MonoBehaviour {

    

    //Movement**************
    public float speed;

    //Weapons*********************
    public GameObject projectile;
    public Transform hardpoint;
    public float shootDelay;
    public float fireRate;
    public float burstTime;
    private float burstEnd;
    private float nextBurst;
    private float nextFire;

    private bool awake;

    private GameObject gm;


    private void Start()
    {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;

        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by stdEnemy!");
        }
    }

    private void FixedUpdate()
    {
        if (gm.GetComponent<GM>().gameStart)
        {
            if (awake)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = (gameObject.GetComponent<Transform>().up * speed) + new Vector3(0.0f, -1.0f, 0.0f);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -1.0f);
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if(Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Instantiate(projectile, hardpoint.position, hardpoint.rotation);
                nextFire = Time.time + fireRate;
            }
            
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Bounds")
        {
            awake = true;
        }
    }


}
