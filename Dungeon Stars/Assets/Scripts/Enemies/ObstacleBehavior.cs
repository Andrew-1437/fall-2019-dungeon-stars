using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObstacleBehavior : MonoBehaviour {

    #region Hp
    public float hp;    // Hp of the enemy
    public float collisionVal;  // Base damage done on a collision with the player
    public bool invincible;
    #endregion

    #region References
    protected GM gm;
    private GameObject camera;
    #endregion

    #region Visual FX
    public GameObject explosion;
    public GameObject hitFX;
    private SpriteRenderer sprite;
    #endregion

    #region Score
    public int score;   // SCore gained upon killing this obstacle
    public GameObject floatingScoreText;    // Reference to the text GameObject to spawn
    #endregion

    #region Boolean Flags
    public bool awake;  // Is the obstacle active?
    public bool isATurret;  // Is this obstacle a turret?
    public bool dontDieOnScreenExit;    // Should this obstacle persist when it exits the screen?
    public bool ignorePlayerCollisions; // Should this obstacle ignore collisions with the player?
    public bool ignoreAwakeOnEnterBounds;   // Should this obstacle not wake when it enters the screen?
    #endregion

    #region Events
    public delegate void ObstacleDelegate(ObstacleBehavior thisObstacle);
    public event ObstacleDelegate OnObstacleDeath;
    #endregion

    protected void Start()
    {
        awake = false;
        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        gm = GM.gameController;

        hp = hp * OmniController.omniController.obstacleHpScale;
        
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            ProjectileBehavior hit = other.gameObject.GetComponent<ProjectileBehavior>();
            Damage(hit.dmgValue);
            if (!hit.perist)
            {
                hit.DestroyProjectile();
            }
            if (hitFX)
                Destroy(Instantiate(hitFX, transform.position, transform.rotation), 1f);
        }
        if (other.tag == "Bounds")
        {
            if(!ignoreAwakeOnEnterBounds)
                awake = true;
        }
    }

    public void Damage(float dmg)
    {
        if (!invincible)
        {
            hp -= dmg * OmniController.omniController.obstacleIncommingDamageScale;
            StartCoroutine(OnHitFx());
        }
    }

    public void Die()
    {
        OmniController.omniController.enemiesKilled++;
        Destroy(gameObject);
        if(explosion)
            Destroy(
                Instantiate(explosion, transform.position, transform.rotation), 
                5f);
        DisplayScore();
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 60.0f);
        gm.AddScore(score);
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
        OnObstacleDeath?.Invoke(this);
    }

    private void DisplayScore()
    {
        if (score != 0 && floatingScoreText)
        {
            GameObject scoreText = Instantiate(floatingScoreText, 
                transform.position, 
                Quaternion.Euler(0f, 0f, Random.Range(-30f,30f))
                ) as GameObject;
            scoreText.GetComponent<TextMeshPro>().text = (score * gm.scoreMultiplier).ToString();
            scoreText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere, ForceMode2D.Impulse);
            Destroy(scoreText, 1);
        }
    }

    public void SleepOnScreenExit()
    {
        awake = false;
    }

    public void SetAwake(bool setAwake)
    {
        awake = setAwake;
    }

    private IEnumerator OnHitFx()
    {
        sprite?.material?.SetFloat("Flash", 1);

        yield return new WaitForSeconds(.05f);

        sprite?.material?.SetFloat("Flash", 0);
    }
}
