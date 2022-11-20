using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBehavior : MonoBehaviour {

    public float speed;
    public float rotate;

    //Weapons*********************
    public GameObject projectile;
    public Transform[] hardpoints;
    public float shootDelay;
    public float fireRate;
    public float burstTime;
    private float burstEnd;
    private float nextBurst;
    private float nextFire;

    protected bool awake;

    protected GM gm;
    protected Rigidbody2D rb;
    protected ObstacleBehavior ob;

    protected void Start()
    {
        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();
        ob = GetComponent<ObstacleBehavior>();
    }

    protected void FixedUpdate()
    {
        if (gm.gameStart)
        {
            float hexSpeedMod;
            if (ob == null) { hexSpeedMod = 1f; }
            else { hexSpeedMod = ob.hex.GetHexSpeedMod(); }

            if (awake)
                rb.velocity = Vector2.down * speed * OmniController.omniController.obstacleSpeedScale * hexSpeedMod;
            else
                rb.velocity = Vector2.down;
            rb.angularVelocity = rotate;
        }
    }

    // Update is called once per frame
    protected void Update()
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
                // For each projectile spawn location, shoot a projectile from there
                foreach (Transform hardpoint in hardpoints)
                {
                    Destroy(
                        Instantiate(projectile, hardpoint.position, hardpoint.rotation), 5f);
                }
                nextFire = Time.time + fireRate * OmniController.omniController.enemyFireRateScale;
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }

}
