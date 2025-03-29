using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Obsolete("Use Shooter component")]
    [Tooltip("Obsolete - Use Shooter component")]
    public GameObject projectile;
    [Obsolete("Use Shooter component")]
    [Tooltip("Obsolete - Use Shooter component")]
    public Transform hardpoint;
    [Obsolete("Use Shooter component")]
    [Tooltip("Obsolete - Use Shooter component")]
    public float shootDelay;
    [Obsolete("Use Shooter component")]
    [Tooltip("Obsolete - Use Shooter component")]
    public float fireRate;
    [Obsolete("Use Shooter component")]
    [Tooltip("Obsolete - Use Shooter component")]
    public float burstTime;
    private float nextFire;
    public bool ShootOnAwake;

    [Header("References")]
    public EnemyIndicator Indicator;

    [HideInInspector]
    public bool awake;

    private bool stunned = false;
    private float stunTimer;

    protected GM gm;
    protected Rigidbody2D rb;
    protected ObstacleBehavior ob;
    protected Shooter[] shooters;


    protected void Start()
    {
        nextFire = 0.0f;
        awake = false;

        gm = GM.GameController;
        rb = GetComponent<Rigidbody2D>();
        ob = GetComponent<ObstacleBehavior>();
        shooters = GetComponents<Shooter>();

        // Check to convert all enemies to use the new Shooter component.
        // Enemies will not work until they have been converted to use it
        if (projectile != null)
        {
            Assert.IsNotNull(shooters, "Enemy is using old shooting behavior - update this one dipshit");
        }

        if (Indicator != null)
        {
            Indicator.InitIndicator(gameObject);
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
        if (awake && (shooters?.Length > 0) && (Time.time > nextFire))
        {
            Shoot();
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

    public void Shoot()
    {
        Array.ForEach(shooters, shooter => shooter.Shoot());
        nextFire = Time.time + shooters.First().TimeBetweenBursts;
    }

    protected virtual void OnAwake()
    {
        awake = true;
        if (ShootOnAwake) { Shoot(); }
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
