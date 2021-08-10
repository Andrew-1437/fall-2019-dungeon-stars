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
    //public GameObject miniExplosion;

    public bool awake;
    [Tooltip("If true, this boss does not take damage from the player's weapons directly.")]
    public bool ignoreProjectileDamage; // If true, this boss does not take damage from the player's weapons directly

    private GameObject[] triggers;

    //private bool dying;
    //private float dieTime;

    //float explosionDelay = .12f;
    //float nextExplosion = 0f;

    //Fungus Flowchart
    public Fungus.Flowchart mainFlowchart;


    private void Start()
    {
        hp = hp * OmniController.omniController.obstacleHpScale;
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ignoreProjectileDamage && other.tag == "Projectile")
        {
            Damage(other.gameObject.GetComponent<ProjectileBehavior>().dmgValue);
            if(!other.gameObject.GetComponent<ProjectileBehavior>().perist)
                other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
        if (other.tag == "Missile")
        {
            other.gameObject.GetComponent<MissileBehavior>().Detonate();
        }
    }

    public override void Die()
    {
        base.Die();
        if(gameCamera)
            gameCamera.GetComponent<CameraShaker>().HugeShake();
        mainFlowchart.SendFungusMessage("LevelComplete");
    }

    public void activeAllTriggers(bool x)
    {
        gameObject.SetActive(x);
    }
    
    //Takes damage from another source (another script)
    public void Damage(float dmg)
    {
        hp -= dmg * dmgMod * OmniController.omniController.obstacleIncommingDamageScale;
        if ((hp <= 0 || turrets <= 0) && !dying)
        {
            dieTime = Time.time + 1.6f;
            dying = true;
        }
    }

    //Wakes boss from another script
    public void Wake()
    {
        awake = true;
    }


}
