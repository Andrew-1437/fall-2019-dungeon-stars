﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : ProjectileBehavior {
    
    public float turn; //Turning speed of projectile
    public string targetTag;
    private GameObject target;

    public GameObject explosion;

    float turnSpeedMod = 1f;
    

    private void Start()
    {
        // Acquire target when instantiated
        base.Start();
        if (targetTag != "")
        {
            target = Functions.FindClosestByTag(targetTag, transform); 
        }
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();

        // Only do missile tracking if missile can actually turn to a target
        if (turn > 0 && targetTag != "")
        {
            if (target != null)
            {
                if (targetTag == Tags.Player && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .5f;
                else
                    turnSpeedMod = 1f;

                // If we have a target, turn towards it
                Vector3 targetDir = target.GetComponent<Transform>().position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.fixedDeltaTime * turnSpeedMod);
            }
            else
            {
                // Acquire new target
                target = Functions.FindClosestByTag(targetTag, transform);
            }
        }

        if (Time.time >= deathTime)
        {
            DestroyProjectile();
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag(Tags.EnemyMissile) && other.CompareTag(Tags.AntiProjectile))
        {
            DestroyProjectile();
        }
        if (other.CompareTag(Tags.Wall))
        {
            Detonate();
        }
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(ObstacleBehavior target)
    {
        Detonate();
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(BossBehavior target)
    {
        Detonate();
    }

    /// <summary>
    /// Applies the projectile's effects. For missiles, this detonates it, instatiating an explosion
    /// object that is considered a projectile
    /// </summary>
    /// <param name="target">Target this missile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(PlayerController target)
    {
        Detonate();
    }

    /// <summary>
    /// Detonates the missile and instatiates it's explosion object. 
    /// The explosion object does the damage.
    /// </summary>
    public void Detonate()
    {
        DestroyProjectile();
        // Clean up payload if it doesn't clean itself up
        Destroy(
            Instantiate(explosion, transform.position, transform.rotation),5);
    }
}
