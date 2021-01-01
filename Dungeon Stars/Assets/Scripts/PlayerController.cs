using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Name and Description
    [Header("Name and Descriptions")]
    public string shipName;
    public string desc;
    public string primaryWeap;
    public string secondaryWeap;
    public string tertiaryWeap;
    public ShipsEnum.ShipID id;

    [HideInInspector]
    public bool isPlayer2 = false;

    //HP & Shields***********
    [Header("HP & Shields")]
    public float maxHp; //Represents max hp
    public float maxShield; //Represents max shields

    public float hp;   //Represents current hp
    public float shield;   //Represents current shields

    bool alive = true;

    public Animator shieldSprite;
    public float shieldRecharge;    //Represents rate at which shield recharges/frame
    public float shieldRegenDelay; //Represents time in seconds since last damage before shield will regen IF SHIELD IS ACTIVE
    public float shieldDelay;   //Represents time in seconds the shield will take to reboot after being disabled
    [HideInInspector]
    public bool shieldDown;    //True if shields are disabled -"Shields offline!"
    private float shieldRegenTime;  //Time relative to game time when shield will regen IF SHIELD IS ACTIVE
    private float shieldUpTime; //Time relative to game time when shield will reactivate
    private float shieldOpacity;

    public int level;


    //Movement***********
    [Header("Movement")]
    public float speed;
    public float rotate;

    //private float rotation = 0.0f;

    //Weapons**********
    [Header("Weapons")]
    public GameObject[] primary; //Primary Fire Projectiles
    public Transform spawner;   //"Hardpoint" Location (Where to spawn projectiles)
    public GameObject[] secondary;  //Secondary Fire Projectiles
    public GameObject explosive;    

    public float fireRate;  //Represents time until next shot
    public float secondaryFireRate;
    private float nextFire;
    private float nextSecondary;

    //Missile count
    public int maxMissile;
    public int currentMissileCount;

    
    //PowerUp Mods
    public float dmgMod;    //Value to modify damage taken, for things like armor (1 = full damage, 0 = no damage, >1 = Extra damage, <0 = Healing??)
    private float fireRateMod;
    private float heatGenMod;  // Fire rate boost also reduces heat generated if using heat
    private float fireRateEnd;

    private float speedMod;
    private float speedEnd;

    private float shieldBoostEnd;
    private float attackSpeedBuff;
    

    [HideInInspector]
    public bool invincible;  // When true, take no damage
    private float endSpawnInvincible;  // Time after spawn to end invulnerability

    // Heat Management
    [HideInInspector]
    public bool enableHeat;
    [HideInInspector]
    public float heat;
    private float heatMod = 1f;
    [HideInInspector]
    public bool overheating = false;
    [HideInInspector]
    public float heatDisperse;
    [HideInInspector]
    public bool primaryUsesHeat;
    [HideInInspector]
    public float primHeatGen; 
    [HideInInspector]
    public bool secondaryUsesHeat;
    [HideInInspector]
    public float secHeatGen;
    [HideInInspector]
    public AudioSource heatWarnAudio;
    //public ParticleSystem heatFX;

    //score
    [Header("Score")]
    public int dieCost;
    public int weapon1Cost;
    public int weapon2Cost;



    //Ship Components & Hardmode (WIP)
    // TODO: Make this work
    //[Header("Hard Mode")]
    //public bool hardmode;

    /*
    // Placeholder for eventual ship components
    [System.Serializable]
    public struct ShipComponents
    {
        public bool hull;
        public bool engines;
        public bool weapons;
        public bool shieldpow;
    } */

    //Rigidbody
    private Rigidbody2D rb;

    //Camera Shake
    private new GameObject camera;

    //GM
    private GM gm;

    //Visual FX************
    [Header("Referenced Game Objects")]
    public GameObject explosionFx;
    public ParticleSystem speedFX;
    public ParticleSystem fireRateFX;
    public ParticleSystem smokeFX;

    //Sound FX**************
    private new AudioSource[] audio;

    //Initialize**********
    void Start()
    {
        hp = maxHp;
        shield = maxShield;
        shieldDown = false;

        audio = gameObject.GetComponents<AudioSource>();

        level = 0;

        fireRateEnd = 0.0f;
        speedEnd = 0.0f;
        shieldBoostEnd = 0.0f;
        currentMissileCount = maxMissile;

        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Camera not found by player!");
        }

        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();

        invincible = true;
        endSpawnInvincible = Time.time + 1.5f;
    }

    //Movement***********
    void FixedUpdate()
    {
        float horizontal;
        float vertical;

        if (!isPlayer2)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal2");
            vertical = Input.GetAxis("Vertical2");
        }

        //rotation -= horizontal;
        Vector2 move = new Vector2(horizontal, vertical);
        if(move.magnitude > 1)
        {
            move.Normalize();
        }
        rb.velocity = move * speed * speedMod;
        transform.rotation = Quaternion.Euler(0.0f,horizontal * rotate, 0.0f);

        Vector2 position = transform.position;
        if (position.x>26.97f)
        {
            position.x = 26.97f;
        } else if(position.x<-26.97f)
        {
            position.x = -26.97f;
        }
        if (position.y>12.6f)
        {
            position.y = 12.6f;
        } else if (position.y<-12.6f)
        {
            position.y = -12.6f; 
        }
        transform.position = position;
    }

    void Update()
    {
        // Primary Fire
        if(((!isPlayer2 && Input.GetButton("Fire1")) || (isPlayer2 && Input.GetButton("Fire12"))) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate * fireRateMod * heatMod;
            Instantiate(primary[level], spawner.position, spawner.rotation);
            gm.AddRawScore(-Mathf.Max(weapon1Cost - Mathf.FloorToInt(weapon1Cost * attackSpeedBuff),0));
            if(enableHeat && primaryUsesHeat)
            {
                heat += primHeatGen * heatGenMod;
            }
        }

        // Secondary Fire
        if(((!isPlayer2 && Input.GetButton("Fire2")) || (isPlayer2 && Input.GetButton("Fire22"))) && Time.time > nextSecondary)
        {
            nextSecondary = Time.time + secondaryFireRate * fireRateMod * heatMod;
            Instantiate(secondary[level], spawner.position, spawner.rotation);
            gm.AddRawScore(-Mathf.Max(weapon2Cost - Mathf.FloorToInt(weapon2Cost * attackSpeedBuff), 0));
            if (enableHeat && secondaryUsesHeat)
            {
                heat += secHeatGen * heatGenMod;
            }
        }

        // Tertiary Fire
        if((!isPlayer2 && Input.GetButtonDown("Fire3")) || (isPlayer2 && Input.GetButtonDown("Fire32")))
        {
            if (currentMissileCount>0)
            {
                Instantiate(explosive, spawner.position, spawner.rotation);
                currentMissileCount--;
            }
            
        }

        // Heat updates
        if(enableHeat)
        {
            heat -= heatDisperse * (1f/heatGenMod) * Time.deltaTime;
            heat = Mathf.Clamp(heat, 0f, 100f);

            // If heat is over 80, considered overheating and take damage
            if(heat >= 80f)
            {
                HullDamage(maxHp * .03f * Time.deltaTime);

                if (!overheating)
                {
                    heatMod = 1.4f;
                    overheating = true;
                    heatWarnAudio.Play();
                }
            }
            else
            {
                if (overheating)
                {
                    heatMod = 1f;
                    overheating = false;
                    heatWarnAudio.Stop();
                }
            }
        }


        //Debug Tools
        if (OmniController.omniController.enableDebug)
        {
            //Quick Level Up
            if (Input.GetKeyDown(KeyCode.RightShift))
                LevelUp();
            //Self Destruct
            if (hp <= 0 || Input.GetKeyDown("backspace"))
                Die();
            //Break Shields 
            if (Input.GetKeyDown("delete") && !shieldDown)
            {
                //Mark shield is down and set time when shield returns
                shieldDown = true;
                shieldUpTime = Time.time + shieldDelay;
                shield = 0;
                shieldSprite.SetTrigger("Broken");
                gameObject.GetComponent<AudioSource>().Play();
            }
            
            //Godmode 
            /*
            if (Input.GetKeyDown(KeyCode.Alpha1) && dmgMod != 0f)
            {
                dmgMod = 0f;
                print("Godmode on");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && dmgMod == 0f)
            {
                dmgMod = 1f;
                print("Godmode off");
            } */
        }
        //Freeze Time
        if (Input.GetKeyDown(KeyCode.P) && Time.timeScale == 1f)
            Time.timeScale = 0f;
        else if (Input.GetKeyDown(KeyCode.P) && Time.timeScale != 1f)
            Time.timeScale = 1f;

        // Shield mechanics************
        if (shieldDown)
        {
            //After we have reached the time when shield is back up, shield returns with 25% hp
            if(Time.time >= shieldUpTime)
            {
                shieldDown = false;
                shield = maxShield / 4;
                shieldSprite.SetTrigger("Restored");
                audio[1].Play();
            }
        }
        // Passively regenerate shield
        if(shield < maxShield && !shieldDown && Time.time >= shieldRegenTime)
        {
            shield += shieldRecharge * maxShield * Time.deltaTime;
        }
        // Upper bound shield
        if(shield > maxShield && Time.time > shieldBoostEnd)
        {
            shield = maxShield;
        }

        // PowerUps
        if(Time.time > speedEnd)
        {
            speedMod = 1.0f;
            speedFX.Stop();
        }
        if(Time.time > fireRateEnd)
        {
            fireRateMod = 1.0f;
            heatGenMod = 1.0f;
            attackSpeedBuff = 0;
            fireRateFX.Stop();
        }

        // FX
        if ((hp <= maxHp * .3f || overheating) && !smokeFX.isPlaying)
        {
            smokeFX.Play();
        }
        if ((hp > maxHp * .3f || !overheating) && smokeFX.isPlaying)
        {
            smokeFX.Stop();
        }

        //Debug
        //print(shield);

        // End spawn invulnerability after enought time has passed
        if(Time.time > endSpawnInvincible)
        {
            invincible = false;
        }
    }

    //Collisions and damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Damage from projectiles
        if(other.tag == "EnemyProjectile")
        {
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue;

            Damage(dmg);

            camera.GetComponent<CameraShaker>().SmallShake();
            if (!other.gameObject.GetComponent<ProjectileBehavior>().perist)
            {
                other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
            }
            
        }

        if(other.tag == "EnemyMissile") //Damage from missiles will be done by the MissileExplosion object
        {
            camera.GetComponent<CameraShaker>().SmallShake(); ;
            other.gameObject.GetComponent<MissileBehavior>().DestroyProjectile();
        }

        // Collision damage
        if(other.tag == "Obstacle")
        {
            ObstacleBehavior obstacle = other.gameObject.GetComponent<ObstacleBehavior>();

            // Can't collide with a turret
            if (!obstacle.isATurret &&
                !obstacle.ignorePlayerCollisions)
            {
                //Total collision dmg = collision value of other * (player speed + other speed)
                float collisionDmg = obstacle.collisionVal
                    * (rb.velocity.magnitude + other.GetComponent<Rigidbody2D>().velocity.magnitude);
                HullDamage(collisionDmg);
                obstacle.hp -= collisionDmg;
                audio[2].Play();
                camera.GetComponent<CameraShaker>().LargeShake();
            }
        }

        if (other.tag == "Laser")
        {
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            Damage(dmg);
        }

        //PowerUps
        if (other.tag == "PowerUp")
        {
            OmniController.omniController.powerUpsCollected++;
            PowerUpBehavior pow = other.gameObject.GetComponent<PowerUpBehavior>();
            // Immediately restore half the shield
            if (pow.type == PowerUpBehavior.PowerUps.Repair)
            {
                shield = Mathf.Min(maxShield,shield+maxShield*0.5f);
                shieldDown = false;
                shieldSprite.SetTrigger("Restored");
                
            }
            // Increases fire rate and reduces heat gen
            if (pow.type == PowerUpBehavior.PowerUps.FireUp)
            {
                fireRateMod = 0.75f;
                heatGenMod = 0.2f;
                attackSpeedBuff = 0.25f;
                fireRateEnd = Time.time + pow.duration;
                fireRateFX.Play();
            }
            // Increases speed
            if (pow.type == PowerUpBehavior.PowerUps.SpeedUp)
            {
                speedMod = 1.25f;
                speedEnd = Time.time + pow.duration;
                speedFX.Play();
            }
            // Increases power level by 1
            if (pow.type == PowerUpBehavior.PowerUps.LevelUp)
            {
                LevelUp();
            }
            // Resets ammo to max
            if (pow.type == PowerUpBehavior.PowerUps.Ammo)
            {
                currentMissileCount = maxMissile;
            }
            audio[3].Play();
            Destroy(other.gameObject);
        }
        /*
        //Events
        if(other.tag == "Event")
        {
            print("Calling event: " + other.GetComponent<EventCaller>().eventName);
            gm.GetComponent<GM>().CallEvent(other.GetComponent<EventCaller>().eventName);
        }
        */


        //Ripperoni
        if (hp < 0 && alive)
        {
            alive = false;
            Die();
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        // Continuous damage
        if(other.tag == "Dps")
        {
            Damage(other.gameObject.GetComponent<ProjectileBehavior>().dmgValue);
        }
    }
    

    //Kills player "Ripperoni"
    public void Die()
    {
        OmniController.omniController.timesDied++;
        Destroy(gameObject);
        Instantiate(explosionFx, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().HugeShake();
        gm.playerLives--;
        if (gm.playerLives < 0)
            gm.playerLives = 0;
        gm.DeathText(isPlayer2);
        gm.AddRawScore(-dieCost);
        gm.ResetMultiplier();
    }


    private void LevelUp()
    {
        level++;
        if (level >= primary.Length || level >= secondary.Length)
        {
            level = primary.Length - 1;
        }
        else
        {
            maxHp += 125f;
            hp += 125f;
            maxShield += 25f;
        }
    }

    // Take damage normally. First absorbed by shield, then hull
    public void Damage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod;

            shield -= dmg;  //All damage hits shield first
            if (shield <= 0 && !shieldDown)
            {
                //Mark shield is down and set time when shield returns
                shieldDown = true;
                shieldUpTime = Time.time + shieldDelay;
                hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
                shield = 0;
                shieldSprite.SetTrigger("Broken");
                gameObject.GetComponent<AudioSource>().Play();
            }
            else if (shield <= 0)
            {
                hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
                shield = 0;
            }
            if (!shieldDown)
            {
                shieldSprite.SetTrigger("Hit");
                shieldRegenTime = Time.time + shieldRegenDelay;
            }
        }
        if (hp < 0 && alive)
        {
            alive = false;
            Die();
        }
    }

    // Damage IGNORES shields. Directly to hull
    public void HullDamage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod;

            hp -= dmg;
        }
        if (hp < 0 && alive)
        {
            alive = false;
            Die();
        }
    }

    

}
