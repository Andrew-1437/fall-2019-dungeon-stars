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

    private void Update()
    {
        if (hp <= 0 && !dying)
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
        if (other.tag == "Projectile")
        {
            hp -= other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            if(!other.gameObject.GetComponent<ProjectileBehavior>().perist)
                other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Instantiate(explosion, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
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
