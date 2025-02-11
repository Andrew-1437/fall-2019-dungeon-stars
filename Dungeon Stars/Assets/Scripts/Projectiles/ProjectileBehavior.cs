﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    #region Variables
    public float speed; // Speed of projectile
    public float dmgValue; // Damage projectile will do
    public float lifeTime;
    protected float deathTime;
    public bool perist;
    public float inaccuracy; // Amount of random rotation to apply to projectile when it spawns
    public GameObject particleFX; // GameObject that contains the projectile's effects
    public ParticleSystem hitFx; // Particle system to play when this projectile hits something
    #endregion

    protected void Start()
    {
        deathTime = Time.time + lifeTime;
        dmgValue = dmgValue * OmniController.omniController.projectileDamageScale;

        // Random rotate the projectile from its starting position if inaccurancy is specified
        if (inaccuracy > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f,
                transform.rotation.eulerAngles.z + Random.Range(-inaccuracy, inaccuracy));
        }

        if (particleFX != null)
        {
            AudioSource[] audioSources = particleFX.GetComponents<AudioSource>();
            foreach (AudioSource audio in audioSources)
            {
                audio.pitch += Random.Range(-.3f, .3f);
            }
        }
    }

    protected void FixedUpdate () {
        transform.position = transform.position + transform.up * speed * OmniController.omniController.projectileSpeedScale;
	}

    protected void Update()
    {
        if(Time.time >= deathTime)
        {
            DestroyProjectile();
        }
    }

    /// <summary>
    /// Damage the target hit by the projectile and apply any effects
    /// </summary>
    /// <param name="target">The Obstacle Behavior hit by this projectile. Pass in "this" from the obstacle getting hit</param>
    public virtual void ApplyProjectile(ObstacleBehavior target)
    {
        target.Damage(dmgValue);

        if (hitFx) { hitFx.Play(); }

        if (!perist)
        {
            DestroyProjectile();
        }
    }

    public virtual void ApplyProjectile(BossBehavior target)
    {
        target.Damage(dmgValue);

        if (hitFx) { hitFx.Play(); }

        if (!perist)
        {
            DestroyProjectile();
        }
    }

    /// <summary>
    /// Damage the target hit by the projectile and apply any effects
    /// </summary>
    /// <param name="target">The Player Controller hit by this projectile. Pass in "this" from the player getting hit</param>
    public virtual void ApplyProjectile(PlayerController target)
    {
        target.Damage(dmgValue);

        if (hitFx) { hitFx.Play(); }

        if (!perist)
        {
            DestroyProjectile();
        }
    }

    public virtual void DestroyProjectile()
    {
        if (particleFX)
        {
            particleFX.transform.parent = null;
            if(particleFX.GetComponent<ParticleSystem>()) particleFX.GetComponent<ParticleSystem>().Stop();
            Destroy(particleFX, 5);

            if (hitFx) { hitFx.Play(); }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag(Tags.EnemyProjectile) && other.CompareTag(Tags.AntiProjectile))
        {
            DestroyProjectile();
        }

        if (other.CompareTag(Tags.Wall))
        {
            DestroyProjectile();
        }
    }
}
