using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Metadata
    [Header("Name and Descriptions")]
    public string shipName;
    public string desc;
    public string primaryWeap;
    public string secondaryWeap;
    public string tertiaryWeap;
    public ShipsEnum.ShipID id;
    
    [HideInInspector]
    public bool isPlayer2 = false;
    #endregion

    #region HP & Shields
    [Header("HP & Shields")]
    public float maxHp; // Represents max hp
    public float maxShield; // Represents max shields

    public float hp;   // Represents current hp
    public float shield;   // Represents current shields

    bool alive = true;

    public Animator shieldSprite;
    [Tooltip("Rate at which shield recharges/second")]
    public float shieldRecharge;    // Represents rate at which shield recharges/second
    [Tooltip("Time in seconds since last damage before shield will regen IF SHIELD IS ACTIVE")]
    public float shieldRegenDelay; // Represents time in seconds since last damage before shield will regen IF SHIELD IS ACTIVE
    [Tooltip("Time in seconds the shield will take to reboot after being disabled")]
    public float shieldDelay;   // Represents time in seconds the shield will take to reboot after being disabled
    [HideInInspector]
    public bool shieldDown;    // True if shields are disabled -"Shields offline!"
    private float shieldRegenTime;  // Time relative to game time when shield will regen IF SHIELD IS ACTIVE
    private float shieldUpTime; // Time relative to game time when shield will reactivate
    private float shieldOpacity;

    public int level;
    #endregion

    #region Movement
    [Header("Movement")]
    public float speed;
    public float rotate;

    bool stunned = false;   // Prevents movement when true
    bool disabled = false;  // Prevents shooting when true;
    float stunEndTime = 0;
    float disableEndTime = 0;
    #endregion
    
    #region Weapons
    [Header("Weapons")]
    public GameObject[] primary; //Primary Fire Projectiles
    public Transform spawner;   //"Hardpoint" Location (Where to spawn projectiles)
    public GameObject[] secondary;  //Secondary Fire Projectiles
    public GameObject explosive;    

    public float fireRate;  //Represents time until next shot
    public float secondaryFireRate;
    private float nextFire;
    private float nextSecondary;

    // Missile count
    public int maxMissile;
    public int currentMissileCount;
    #endregion

    #region Local Modifiers
    public float dmgMod;    // Value to modify damage taken, for things like armor (1 = full damage, 0 = no damage, >1 = Extra damage, <0 = Healing??)
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
    #endregion

    #region Heat Management
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
    #endregion

    #region Weapon Costs
    [Header("Score")]
    public int weapon1Cost;
    public int weapon2Cost;
    #endregion

    #region References
    private GM gm;  
    private Rigidbody2D rb; 
    private GameObject camera;
    #endregion

    #region Visual & Audio FX
    [Header("Referenced Game Objects")]
    public GameObject explosionFx;
    public ParticleSystem speedFX;
    public ParticleSystem fireRateFX;
    public ParticleSystem smokeFX;

    // Sound FX**************
    private new AudioSource[] audio;
    #endregion

    #region Events
    public delegate void PlayerDelegate(PlayerController pc);
    public static event PlayerDelegate OnPlayerSpawn;
    public static event PlayerDelegate OnPlayerDeath;
    #endregion

    // Initialize**********
    void Start()
    {
        maxHp = maxHp * OmniController.omniController.playerHpScale;
        maxShield = maxShield * OmniController.omniController.playerShieldScale;

        hp = maxHp;
        shield = maxShield;
        shieldDown = false;

        audio = gameObject.GetComponents<AudioSource>();

        level = 0;

        fireRateEnd = 0.0f;
        speedEnd = 0.0f;
        shieldBoostEnd = 0.0f;
        currentMissileCount = maxMissile;

        camera = Camera.main.gameObject;
        if(camera == null)
        {
            print("Ohshit! Camera not found by player!");
        }

        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();

        invincible = true;
        endSpawnInvincible = Time.time + 1.5f;

        OnPlayerSpawn?.Invoke(this);
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
        // If not stunned, allow movement
        if (!stunned)
        {
            rb.velocity = move * speed * speedMod * OmniController.omniController.playerSpeedScale;
            transform.rotation = Quaternion.Euler(0.0f, horizontal * rotate, 0.0f);
            
            // Only voluntary movement can be restricted to the screen bounds
            Vector2 position = transform.position;
            if (position.x > gm.rightBounds)
            {
                position.x = gm.rightBounds;
            }
            else if (position.x < gm.leftBounds)
            {
                position.x = gm.leftBounds;
            }
            if (position.y > gm.upperBounds)
            {
                position.y = gm.upperBounds;
            }
            else if (position.y < gm.lowerBounds)
            {
                position.y = gm.lowerBounds;
            }
            transform.position = position;
            
        }
    }

    void Update()
    {
        // If the game is paused, do not allow firing of weapons
        // If disabled, do not allow firing of weapons
        if (!GM.gameController.gamePaused && !disabled)
        {
            // Primary Fire
            if (((!isPlayer2 && Input.GetButton("Fire1")) || (isPlayer2 && Input.GetButton("Fire12"))) && Time.time > nextFire)
            {
                FirePrimaryWeapon();
            }

            // Secondary Fire
            if (((!isPlayer2 && Input.GetButton("Fire2")) || (isPlayer2 && Input.GetButton("Fire22"))) && Time.time > nextSecondary)
            {
                FireSecondaryWeapon();
            }

            // Tertiary Fire
            if ((!isPlayer2 && Input.GetButtonDown("Fire3")) || (isPlayer2 && Input.GetButtonDown("Fire32")))
            {
                if (currentMissileCount > 0)
                {
                    Instantiate(explosive, spawner.position, spawner.rotation);
                    currentMissileCount--;
                }
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

        // Out of bounds death
        // Involuntary movement (walls) could move the player beyond the screen bounds and thus kill them
        if (transform.position.x > 28f || transform.position.x < -28f || 
            transform.position.y > 13.6f || transform.position.y < -13.6f)
            Die();

        //Debug Tools
        if (OmniController.omniController.enableDebug)
        {
            //Quick Level Up
            if (Input.GetKeyDown(KeyCode.RightShift))
                LevelUp();
            //Self Destruct
            if (hp <= 0 || Input.GetKeyDown(KeyCode.Backspace))
                StartCoroutine(DieSequence());
            //Break Shields 
            if (Input.GetKeyDown(KeyCode.Delete) && !shieldDown)
            {
                //Mark shield is down and set time when shield returns
                shieldDown = true;
                shieldUpTime = Time.time + shieldDelay;
                shield = 0;
                shieldSprite.SetTrigger("Broken");
                gameObject.GetComponent<AudioSource>().Play();
            }
            
            //Godmode 
            if (Input.GetKeyDown(KeyCode.F1) && !invincible)
            {
                invincible = true;
                print("Godmode on");
            }
            else if (Input.GetKeyDown(KeyCode.F1) && invincible)
            {
                invincible = false;
                print("Godmode off");
            } 
        }

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

        // Stunned
        if (stunned && Time.time >= stunEndTime)
            stunned = false;
        // Disabled
        if (disabled && Time.time >= disableEndTime)
            disabled = false;

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
                    * (rb.velocity.magnitude + other.GetComponent<Rigidbody2D>().velocity.magnitude)
                    * OmniController.omniController.collisionDamageScale;
                HullDamage(collisionDmg);
                obstacle.Damage(collisionDmg);
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
            // Immediately restore 75% of missing hp and shield
            if (pow.type == PowerUpBehavior.PowerUps.HpRepair)
            {
                hp = Mathf.Min(maxHp, hp + (maxHp - hp) * 0.75f);
                shield = Mathf.Min(maxShield, shield + (maxShield - shield) * 0.75f);
                shieldDown = false;
                shieldSprite.SetTrigger("Restored");
            }
            // Increases fire rate and reduces heat gen
            if (pow.type == PowerUpBehavior.PowerUps.FireUp)
            {
                fireRateMod = 0.75f;
                heatGenMod = 0.2f;
                attackSpeedBuff = 0.25f;
                fireRateEnd = Time.time + pow.duration * OmniController.omniController.powerUpDurationScale;
                fireRateFX.Play();
            }
            // Increases speed
            if (pow.type == PowerUpBehavior.PowerUps.SpeedUp)
            {
                speedMod = 1.25f;
                speedEnd = Time.time + pow.duration * OmniController.omniController.powerUpDurationScale;
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
            // Summons a bolt-shooting drone to assist
            if (pow.type == PowerUpBehavior.PowerUps.BoltDrone)
            {
                pow.Summon(isPlayer2);
            }
            pow.OnCollected();
        }


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
    
    // Disables and stuns player while they fly around, on fire, until they explode and die.
    public IEnumerator DieSequence()
    {
        Stun(100);
        Disable(100);

        smokeFX.Play();

        rb.freezeRotation = false;
        rb.angularVelocity = Random.Range(-420f, 420f);
        rb.angularDrag = 0f;
        rb.velocity = rb.velocity + Random.insideUnitCircle * Random.Range(3f, 10f);
        
        yield return new WaitForSeconds(Random.Range(1.5f, 4f));

        Die();
    }

    //Kills player "Ripperoni"
    public void Die()
    {
        OmniController.omniController.timesDied++;
        Destroy(gameObject);
        Instantiate(explosionFx, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().HugeShake();
        
        OnPlayerDeath?.Invoke(this);
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
            float levelUpHp = Mathf.Floor(maxHp * .2f * OmniController.omniController.hpPerLevelScale);
            maxHp += levelUpHp;
            hp += levelUpHp;
            maxShield += Mathf.Floor(maxShield * .2f * OmniController.omniController.shieldPerLevelScale);
        }
    }

    // Take damage normally. First absorbed by shield, then hull
    public void Damage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod * OmniController.omniController.playerIncommingDamageScale;

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
            StartCoroutine(DieSequence());
        }
    }

    // Damage IGNORES shields. Directly to hull
    public void HullDamage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod * OmniController.omniController.playerIncommingDamageScale;

            hp -= dmg;
        }
        if (hp < 0)
        {
            alive = false;
            Die();
        }
    }

    // Shoots the primary weapon and manages what else happens when it does so
    public void FirePrimaryWeapon()
    {
        nextFire = Time.time + fireRate *
                fireRateMod * heatMod * OmniController.omniController.playerFireRateScale;
        // Spawn projectile and delete it 5 seconds later if it is not already deleted
        Destroy(
            Instantiate(primary[level], spawner.position, spawner.rotation), 
            5);
        // Subtract score for every shot fired
        gm.AddRawScore(-Mathf.Max(weapon1Cost - Mathf.FloorToInt(weapon1Cost * attackSpeedBuff), 0));
        // Add heat if the ship and weapon use heat
        if (enableHeat && primaryUsesHeat)
        {
            heat += primHeatGen * heatGenMod;
        }
    }

    // Shoots the secondary weapon and manages what else happens when it does so
    public void FireSecondaryWeapon()
    {
        nextSecondary = Time.time + secondaryFireRate *
                fireRateMod * heatMod * OmniController.omniController.playerFireRateScale;
        // Spawn projectile and delete it 5 seconds later if it is not already deleted
        Destroy(
            Instantiate(secondary[level], spawner.position, spawner.rotation), 
            5);
        // Subtract score for every shot fired
        gm.AddRawScore(-Mathf.Max(weapon2Cost - Mathf.FloorToInt(weapon2Cost * attackSpeedBuff), 0));

        // Add heat if the ship and weapon use heat
        if (enableHeat && secondaryUsesHeat)
        {
            heat += secHeatGen * heatGenMod;
        }
    }

    // Halt Movement
    public void Stun(float seconds)
    {
        stunned = true;
        // Set stun end time to be seconds from now, or extend current stun end time if we are already stunned
        stunEndTime = Mathf.Max(stunEndTime, Time.time) + seconds;
    }

    public void Disable(float seconds)
    {
        disabled = true;
        // Set stun end time to be seconds from now, or extend current stun end time if we are already stunned
        disableEndTime = Mathf.Max(disableEndTime, Time.time) + seconds;
    }



}
