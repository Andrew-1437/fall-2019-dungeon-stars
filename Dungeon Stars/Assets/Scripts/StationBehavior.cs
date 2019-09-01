using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBehavior : MonoBehaviour {

    public float speed;
    public float rotate;

    //Weapons*********************
    public GameObject projectile;
    public Transform hardpoint0;
    public Transform hardpoint1;
    public Transform hardpoint2;
    public Transform hardpoint3;
    public Transform hardpoint4;
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
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by Station!");
        }
    }

    private void FixedUpdate()
    {
        if (gm.GetComponent<GM>().gameStart)
        {
            if (awake)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -speed);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -1.0f);
            }
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = rotate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Instantiate(projectile, hardpoint0.position, hardpoint0.rotation);
                Instantiate(projectile, hardpoint1.position, hardpoint1.rotation);
                Instantiate(projectile, hardpoint2.position, hardpoint2.rotation);
                Instantiate(projectile, hardpoint3.position, hardpoint3.rotation);
                Instantiate(projectile, hardpoint4.position, hardpoint4.rotation);
                nextFire = Time.time + fireRate;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }

}
