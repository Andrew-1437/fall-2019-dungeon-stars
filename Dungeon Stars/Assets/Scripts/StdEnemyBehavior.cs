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

    private bool stunned = false;
    private float stunTimer;

    GM gm;
    Rigidbody2D rb;


    private void Start()
    {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;

        /*
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by stdEnemy!");
        } */
        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (gm.gameStart)
        {
            if (!stunned)
            {
                if (awake)
                {
                    rb.velocity = (transform.up * speed) + new Vector3(0.0f, -1.0f, 0.0f);
                }
                else
                {
                    rb.velocity = new Vector2(0.0f, -1.0f);
                }
            }
            else
            {
                if(Time.time >= stunTimer)
                {
                    stunned = false;
                }
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

    public void Stun(float stunTime)
    {
        stunned = true;
        stunTimer = Time.time + stunTime;
    }


}
