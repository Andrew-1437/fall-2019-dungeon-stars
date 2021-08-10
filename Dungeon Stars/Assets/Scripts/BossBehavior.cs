using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : LargeEnemyBehavior {

    //Health
    public float hp;
    public float dmgMod;

    //Camera Shake
    //private GameObject camera;

    //Visual FX
    //public GameObject explosion;
    public GameObject miniExplosion;

    public bool awake;
    [Tooltip("If true, this boss does not take damage from the player's weapons directly.")]
    public bool ignoreProjectileDamage; // If true, this boss does not take damage from the player's weapons directly

    private GameObject[] triggers;

    private bool dying;
    private float dieTime;

    float explosionDelay = .12f;
    float nextExplosion = 0f;

    //Fungus Flowchart
    public Fungus.Flowchart mainFlowchart;


    private void Start()
    {
        dieTime = Mathf.Infinity;
        base.Start();
    }

    protected void Update()
    {
        if ((hp <= 0 || turrets <= 0) && !dying)
        {
            dieTime = Time.time + 1.6f;
            dying = true;
            
        }
        if(dying && Time.time > dieTime)
        {
            Die();
        }
        else if(dying && Time.time > nextExplosion)
        {
            Vector3 pos = (Random.insideUnitSphere * 10) + transform.position;
            Instantiate(miniExplosion, pos, transform.rotation);
            nextExplosion = Time.time + explosionDelay;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ignoreProjectileDamage && other.tag == "Projectile")
        {
            hp -= other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            if(!other.gameObject.GetComponent<ProjectileBehavior>().perist)
                other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
        if (other.tag == "Missile")
        {
            other.gameObject.GetComponent<MissileBehavior>().Detonate();
        }
    }

    private void Die()
    {
        
        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
        if(gameCamera)
            gameCamera.GetComponent<CameraShaker>().HugeShake();
        mainFlowchart.SendFungusMessage("LevelComplete");
    }

    public void activeAllTriggers(bool x)
    {
        gameObject.SetActive(x);
        /*
        foreach (GameObject trigger in triggers)
        {
            trigger.SetActive(x);
        } */
    }
    
    //Takes damage from another source (another script)
    public void TakeDmg(float dmg)
    {
        hp -= dmg * dmgMod;
    }

    //Wakes boss from another script
    public void Wake()
    {
        awake = true;
    }


}
