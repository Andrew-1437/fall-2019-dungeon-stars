using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : LargeEnemyBehavior {

    public string bossTitle;

    //Health
    public float hp;
    public float dmgMod;

    //Visual FX
    public GameObject miniExplosion;

    public bool awake;
    [Tooltip("If true, this boss does not take damage from the player's weapons directly.")]
    public bool ignoreProjectileDamage; // If true, this boss does not take damage from the player's weapons directly

    private GameObject[] triggers;

    //private bool dying;
    //private float dieTime;

    //float explosionDelay = .12f;
    //float nextExplosion = 0f;

    // Events
    public delegate void BossDelegate();
    public static event BossDelegate OnBossDeath;

    protected void Awake()
    {
        hp = hp * OmniController.omniController.obstacleHpScale;
        dieTime = Mathf.Infinity;
        GM.OnBossActivate += GM_OnBossActivate;
        GM.OnLevelEnd += GM_OnLevelEnd;
    }

    private void GM_OnLevelEnd()
    {
        GM.OnBossActivate -= GM_OnBossActivate;
        GM.OnLevelEnd -= GM_OnLevelEnd;
    }

    private void GM_OnBossActivate()
    {
        Wake();
        GM.OnBossActivate -= GM_OnBossActivate;
        gameObject.SetActive(true);
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
        OnBossDeath?.Invoke();
        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
        gameCamera?.GetComponent<CameraShaker>().HugeShake();
        GM.OnLevelEnd -= GM_OnLevelEnd;
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
            hp = 0;
            BeginDeathSequence();
        }
        GM.gameController.UpdateBossHpBar(hp);
    }

    //Wakes boss from another script
    public void Wake()
    {
        awake = true;
        GM.gameController.SetBossHpBar(bossTitle, hp);
    }


}
