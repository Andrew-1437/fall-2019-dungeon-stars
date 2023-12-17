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

    #region Objects
    protected GM gm;
    private GameObject camera;
    public HexStatus hex;
    #endregion

    #region Visual FX
    public GameObject explosion;
    public GameObject hitFX;
    private SpriteRenderer sprite;
    #endregion

    #region Score
    public int score;   // SCore gained upon killing this obstacle
    private GameObject floatingScoreText;    // Reference to the text GameObject to spawn
    private GameObject floatingDamageText;   
    #endregion

    #region Boolean Flags
    public bool awake;  // Is the obstacle active?
    [Tooltip("Is this obstacle a turret? Expects a LargeEnemyBehavior class in the parent gameobject")]
    public bool isATurret;  // Is this obstacle a turret? Expects a LargeEnemyBehavior class in the parent gameobject 
    [Tooltip("Should this obstacle persist when it exits the screen?")]
    public bool dontDieOnScreenExit;    // Should this obstacle persist when it exits the screen?
    [Tooltip("Should this obstacle ignore collisions with the player?")]
    public bool ignorePlayerCollisions; // Should this obstacle ignore collisions with the player?
    [Tooltip("Should this obstacle not wake when it enters the screen?")]
    public bool ignoreAwakeOnEnterBounds;   // Should this obstacle not wake when it enters the screen?
    #endregion

    #region Events
    public delegate void ObstacleDelegate(ObstacleBehavior thisObstacle);
    public event ObstacleDelegate OnObstacleDeath;
    #endregion

    private void Awake()
    {
        hex = new HexStatus();
    }

    protected void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        gm = GM.GameController;

        hp = hp * OmniController.omniController.obstacleHpScale;
        
        sprite = GetComponentInChildren<SpriteRenderer>();

        floatingScoreText = OmniController.omniController.FloatingScoreText;
        floatingDamageText = OmniController.omniController.FloatingDamageText;
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            Die();
        }

        hex.Update();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.tag == "Missile")
        {
            if (!awake) { return; }
            ProjectileBehavior hit = other.gameObject.GetComponent<ProjectileBehavior>();
            hit.ApplyProjectile(this);
            if (hitFX)
                Destroy(Instantiate(hitFX, transform.position, transform.rotation), 1f);
        }
        if (other.tag == "Bounds")
        {
            if(!ignoreAwakeOnEnterBounds)
                awake = true;
        }
    }

    public void Damage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * OmniController.omniController.obstacleIncommingDamageScale * hex.GetHexDmgMod();
            hp -= dmg;
            DisplayDamage(dmg);
            StartCoroutine(OnHitFx());
        }
    }

    public void Die()
    {
        OmniController.omniController.enemiesKilled++;
        Destroy(gameObject);
        if (explosion)
        {
            Destroy(
                Instantiate(explosion, transform.position, transform.rotation),
                5f);
        }
        DisplayScore();
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 60.0f);

        if (hex.Stacks > 0)
        {
            hex.Hexplosion(transform);
        }

        gm.AddScore(score);
        if (isATurret)
        {
            // Some turrets might not be on a LargeEnemyBehavior. Ignore if there is none.
            try
            {
                GetComponentInParent<LargeEnemyBehavior>().turrets--;
            }
            catch (System.NullReferenceException)
            {
                // Do nothing
            }
        }
        OnObstacleDeath?.Invoke(this);
    }

    private void DisplayScore()
    {
        if (score != 0 && floatingScoreText)
        {
            GameObject scoreText = Instantiate(floatingScoreText, 
                transform.position, 
                Quaternion.Euler(0f, 0f, Random.Range(-30f,30f)));
            scoreText.GetComponent<TextMeshPro>().text = (score * gm.scoreMultiplier).ToString();
            scoreText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere, ForceMode2D.Impulse);
            Destroy(scoreText, 1);
        }
    }

    private void DisplayDamage(float dmg)
    {
        if (!OmniController.omniController.showDamageNumbers) { return; }

        if (dmg != 0 && floatingDamageText)
        {
            GameObject dmgText = Instantiate(floatingDamageText,
                transform.position + Random.insideUnitSphere,
                Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)));
            dmgText.GetComponent<TextMeshPro>().text = ((int)dmg).ToString();
            dmgText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere * 3f, ForceMode2D.Impulse);
            Destroy(dmgText, .5f);
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
