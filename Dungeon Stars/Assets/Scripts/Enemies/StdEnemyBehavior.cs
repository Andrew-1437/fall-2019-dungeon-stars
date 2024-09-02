using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Assertions;

public class StdEnemyBehavior : MonoBehaviour {

    //Movement**************
    [Header("Movement")]
    public float speed;
    public float turn;
    public Vector2 additionalMovementVector;
    public bool lookAtPlayer;

    //Weapons*********************
    [Header("Attack")]
    [Obsolete]
    public GameObject projectile;
    [Obsolete]
    public Transform hardpoint;
    public float shootDelay;
    [Obsolete]
    public float fireRate;
    [Obsolete]
    public float burstTime;
    private float nextFire;
    public bool ShootOnAwake;

    protected bool awake;

    private bool stunned = false;
    private float stunTimer;

    protected GM gm;
    protected Rigidbody2D rb;
    protected ObstacleBehavior ob;
    protected Shooter shooter;


    protected void Start()
    {
        nextFire = 0.0f;
        awake = false;

        gm = GM.GameController;
        rb = GetComponent<Rigidbody2D>();
        ob = GetComponent<ObstacleBehavior>();
        shooter = GetComponent<Shooter>();

        // Check to convert all enemies to use the new Shooter component.
        // Enemies will not work until they have been converted to use it
        if (projectile != null)
        {
            Assert.IsNotNull(shooter, "Enemy is using old shooting behavior - update this one dipshit");
        }
    }

    protected void FixedUpdate()
    {
        if (gm.gameStart)
        {
            if (!stunned)
            {
                if (awake)
                {
                    rb.velocity = ((transform.up + (Vector3)additionalMovementVector).normalized *
                        speed * GetSpeedMod()) + Vector3.down;
                }
                else
                {
                    rb.velocity = Vector2.down;
                }
            }
            else
            {
                if (Time.time >= stunTimer)
                {
                    stunned = false;
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (lookAtPlayer)
        {
            GameObject target = Functions.FindNearestPlayer(transform);
            if (target != null)
            {
                /*
                // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
                if (target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .35f;
                else
                    turnSpeedMod = 1f;*/

                Functions.RotateTowardsTarget(gameObject, target, turn);
            }
        }
    }
    

    // Update is called once per frame
    protected void Update () 
    {
        if (awake && (shooter != null) && (Time.time > nextFire))
        {
            shooter.Shoot();
            nextFire = Time.time + shootDelay;
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(Tags.Bounds))
        {
            OnAwake();
        }
    }

    public void Stun(float stunTime)
    {
        stunned = true;
        stunTimer = Time.time + stunTime;
    }

    protected virtual void OnAwake()
    {
        awake = true;
        if (ShootOnAwake) { shooter.Shoot(); }
        nextFire = Time.time + shootDelay;
    }

    /// <summary>
    /// Returns the modifer to multiply to the enemy's speed based on effects acting on this enemy
    /// </summary>
    /// <returns>Modifier to multiply the speed by</returns>
    protected float GetSpeedMod()
    {
        float hexSpeedMod = (ob == null) ? 1f : ob.hex.GetHexSpeedMod();

        return OmniController.omniController.obstacleSpeedScale * hexSpeedMod;
    }


}
