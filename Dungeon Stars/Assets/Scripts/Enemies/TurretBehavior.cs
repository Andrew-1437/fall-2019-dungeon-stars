﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    #region Targeting
    [Tooltip("Leave empty or specify \"Player\" to target player")]
    public string targetTag;    // Default target is player
    public GameObject projectile;
    protected GameObject target;
    #endregion

    #region Shooting & Aiming
    public float shootDelay;
    public float fireRate;
    public float burstTime;
    protected float burstEnd = 0f;
    protected float nextBurst = Mathf.Infinity;
    protected float nextFire = Mathf.Infinity;

    public float turn;
    float turnSpeedMod = 1f;

    public Transform hardpoint;
    #endregion

    #region Boolean Flags
    public bool awake;
    bool wasSleeping;
    [Tooltip("If true, will not shoot, even if awake")]
    public bool holdFire;   
    [Tooltip("If true, the turret will not care about an attached ObstacleBehavior script. " +
        "Mainly used for player's turrets or invulnerable turrets that should not be targeted.")]
    public bool ignoreObstacle;     
    #endregion

    ObstacleBehavior thisObstacle;  // Reference to this gameObject's ObstacleBehavior script
    GM gm;

    public delegate void TurretDelegate();
    public event TurretDelegate OnBurstEnd;

    // Use this for initialization
    protected void Start () {
        nextBurst = Time.time + shootDelay;
        nextFire = 0f;
        bool wasSleeping = !awake;

        if (targetTag == "") targetTag = Tags.Player;  // If targetTag is empty, target player

        if (!ignoreObstacle)
            thisObstacle = GetComponent<ObstacleBehavior>();

        if (hardpoint == null)
        {
            hardpoint = transform;
        }

        gm = GM.GameController;

        StartCoroutine(FindTargetsAsync());
    }
	
	// Update is called once per frame
	protected void Update ()
    {
        if (!ignoreObstacle)
        {
            awake = thisObstacle.awake;
        }

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
            // Target is found by FindTargetsAsync() every .2 seconds, not in the update method
            
            if (target != null)
            {
                // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
                if (targetTag == Tags.Player && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .35f;
                else
                    turnSpeedMod = 1f;

                Functions.RotateTowardsTarget(gameObject, target, turn * turnSpeedMod);
            }
        }

        if (awake && target != null && !holdFire && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
                OnBurstEnd?.Invoke();
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
            30f);
        nextFire = Time.time + fireRate * OmniController.omniController.enemyFireRateScale;
    }

    public void HoldFire(bool state)
    {
        holdFire = state;
    }

    public void Awaken()
    {
        awake = true;
        nextBurst = Time.time + burstTime;
        nextFire = 0.0f;
    }

    /// <summary>
    /// Searches for a target every .2 seconds when awake
    /// </summary>
    private IEnumerator FindTargetsAsync()
    {
        while (true)
        {
            if (awake) { target = Functions.FindClosestByTag(targetTag, transform); }
            yield return new WaitForSeconds(.2f);
        }
    }


}
