﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    public float speed; //Speed of projectile
    public float dmgValue; //Damage projectile will do
    public float lifeTime;
    private float deathTime;
    public bool perist;
    public GameObject particleFX;

    protected void Start()
    {
        deathTime = Time.time + lifeTime;
    }

    protected void FixedUpdate () {
        transform.position = transform.position + transform.up * speed;
	}

    protected void Update()
    {
        if(Time.time >= deathTime)
        {
            this.DestroyProjectile();
        }
    }

    public virtual void DestroyProjectile()
    {
        if (particleFX)
        {
            particleFX.transform.parent = null;
            if(particleFX.GetComponent<ParticleSystem>()) particleFX.GetComponent<ParticleSystem>().Stop();
            Destroy(particleFX, 5);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "EnemyProjectile" && other.tag == "AntiProjectile")
        {
            this.DestroyProjectile();
        }
        
    }
}
