using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public Transform spawner;   //"Hardpoint" Location (Where to spawn projectiles)
    public Weapon primary; 
    public Weapon secondary;  
    public Weapon explosive;   
    #endregion

    #region Local Modifiers
    [HideInInspector]
    public float dmgMod;    // Value to modify damage taken, for things like armor (1 = full damage, 0 = no damage, >1 = Extra damage, <0 = Healing??)
    [HideInInspector]
    public float fireRateMod;
    [HideInInspector]
    public float heatGenMod;  // Fire rate boost also reduces heat generated if using heat
    [HideInInspector]
    public float fireRateEnd;

    [HideInInspector]
    public float speedMod;
    [HideInInspector]
    public float speedEnd;

    private float shieldBoostEnd;
    
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

    #region References
    private GM gm;  
    private Rigidbody2D rb; 
    private GameObject camera;
    public HexStatus hex;
    private GameObject floatingDamageText;
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
        maxHp *= OmniController.omniController.playerHpScale;
        maxShield *= OmniController.omniController.playerShieldScale;

        hp = maxHp;
        shield = maxShield;
        shieldDown = false;

        audio = gameObject.GetComponents<AudioSource>();

        level = 0;

        fireRateEnd = 0.0f;
        speedEnd = 0.0f;
        shieldBoostEnd = 0.0f;

        primary.Start();
        secondary.Start();
        explosive.Start();

        camera = Camera.main.gameObject;
        if(camera == null)
        {
            print("Ohshit! Camera not found by player!");
        }

        gm = GM.GameController;
        rb = GetComponent<Rigidbody2D>();

        hex = new HexStatus();

        invincible = true;
        endSpawnInvincible = Time.time + 1.5f;

        floatingDamageText = OmniController.omniController.FloatingDamageText;

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
            rb.velocity = move * speed * speedMod * OmniController.omniController.playerSpeedScale * hex.GetHexSpeedMod();
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
        if (!GM.GameController.gamePaused && !disabled)
        {
            // Primary Fire
            if ((!isPlayer2 && Input.GetButton("Fire1")) || (isPlayer2 && Input.GetButton("Fire12")))
            {
                primary.Fire(spawner, this, fireRateMod, heatMod, isPlayer2 ? "Fire12" : "Fire1");
            }

            // Secondary Fire
            if ((!isPlayer2 && Input.GetButton("Fire2")) || (isPlayer2 && Input.GetButton("Fire22")))
            {
                secondary.Fire(spawner, this, fireRateMod, heatMod, isPlayer2 ? "Fire22" : "Fire2");
            }

            // Tertiary Fire
            if ((!isPlayer2 && Input.GetButton("Fire3")) || (isPlayer2 && Input.GetButton("Fire32")))
            {
                explosive.Fire(spawner, this, fireRateMod, heatMod, isPlayer2 ? "Fire32" : "Fire3");
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
        if (transform.position.x > gm.rightBounds + 1 || transform.position.x < gm.leftBounds - 1 ||
            transform.position.y > gm.upperBounds + 1 || transform.position.y < gm.lowerBounds - 1)
        {
            Die();
        }

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
            fireRateFX.Stop();
        }

        // Stunned
        if (stunned && Time.time >= stunEndTime)
        {
            stunned = false;
        }
        // Disabled
        if (disabled && Time.time >= disableEndTime)
        {
            disabled = false;
        }

        // End spawn invulnerability after enought time has passed
        if(Time.time > endSpawnInvincible)
        {
            invincible = false;
        }

        hex.Update();
    }

    //Collisions and damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Damage from projectiles/missiles
        if(other.CompareTag(Tags.EnemyProjectile) || other.CompareTag(Tags.EnemyMissile))
        {
            ProjectileBehavior hit = other.gameObject.GetComponent<ProjectileBehavior>();

            hit.ApplyProjectile(this);

            camera.GetComponent<CameraShaker>().SmallShake();
            
        }

        // Collision damage
        if(other.CompareTag(Tags.Obstacle))
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

        if (other.CompareTag(Tags.Laser))
        {
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            Damage(dmg);
        }

        //PowerUps
        if (other.CompareTag(Tags.PowerUp))
        {
            OmniController.omniController.powerUpsCollected++;
            PowerUpBehavior pow = other.gameObject.GetComponent<PowerUpBehavior>();
            
            pow.OnCollected(this);
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
        if(other.CompareTag(Tags.Dps))
        {
            Damage(other.gameObject.GetComponent<ProjectileBehavior>().dmgValue);
        }
    }

    /// <summary>
    /// Begins the death sequence, disabling and stunning the player while they 
    /// spiral out of control on fire, until they explode and die.
    /// </summary>
    public IEnumerator DieSequence()
    {
        if (!alive) { yield break; }

        alive = false;

        Stun(100);
        Disable(100);

        smokeFX.Play();

        rb.freezeRotation = false;
        rb.angularVelocity = Random.Range(-420f, 420f);
        rb.angularDrag = 0f;
        rb.velocity += Random.insideUnitCircle * Random.Range(3f, 10f);
        
        yield return new WaitForSeconds(Random.Range(1f, 3.5f));

        Die();
    }

    /// <summary>
    /// Kills player "Ripperoni"
    /// Increments their death counter, destroys this game object, and
    /// spawns a massive explosion as the player dies horribly
    /// </summary>
    public void Die()
    {
        OmniController.omniController.timesDied++;
        Destroy(gameObject);
        Instantiate(explosionFx, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().HugeShake();

        // Drop level ups equal to half the player's level rounded down
        for (int i = 0; i < (level + 1) / 2; i++)
        {
            Instantiate(OmniController.omniController.LevelUp,
                transform.position + Random.insideUnitSphere * 5f,
                transform.rotation).GetComponent<PowerUpBehavior>().GoToScreenTop();
        }
        
        OnPlayerDeath?.Invoke(this);
    }

    /// <summary>
    /// Increases the power level of the player when they collect a LevelUp
    /// This increases their HP & Shields and improves their weapons
    /// </summary>
    public void LevelUp()
    {
        level++;
        if (level > 4)
        {
            level = 4;
        }
        else
        {
            float levelUpHp = Mathf.Floor(maxHp * .2f * OmniController.omniController.hpPerLevelScale);
            maxHp += levelUpHp;
            hp += levelUpHp;
            maxShield += Mathf.Floor(maxShield * .2f * OmniController.omniController.shieldPerLevelScale);
        }
    }

    /// <summary>
    /// Applies damage to the player, accounting for damage modifiers this player has.
    /// First, any modifiers that apply are multiplied to the incoming damage.
    /// This damage is first applied to the shield, then applied to the hull
    /// If any is leftover from the shield, that is also applied to the hull
    /// </summary>
    /// <param name="baseDmg">Incoming damage from one source</param>
    public void Damage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod * OmniController.omniController.playerIncommingDamageScale * hex.GetHexDmgMod();

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
            DisplayDamage(dmg);
        }
        if (hp < 0 && alive)
        {
            StartCoroutine(DieSequence());
        }
    }

    /// <summary>
    /// Applies damage to the player, accounting for damage modifiers this player has.
    /// First, any modifiers that apply are multiplied to the incoming damage.
    /// This damage is applied directly to the ship's HP, ignoring shields
    /// </summary>
    /// <param name="baseDmg">Incoming damage</param>
    public void HullDamage(float baseDmg)
    {
        if (!invincible)
        {
            float dmg = baseDmg * dmgMod * OmniController.omniController.playerIncommingDamageScale * hex.GetHexDmgMod();

            hp -= dmg;

            DisplayDamage(dmg);
        }
        if (hp < 0)
        {
            alive = false;
            Die();
        }
    }

    /// <summary>
    /// Adds one instance of heat to the player
    /// </summary>
    /// <param name="applyHeat">Heat to apply</param>
    public void AddHeat(float applyHeat)
    {
        heat += applyHeat * heatGenMod;
    }

    /// <summary>
    /// Prevents movement for a time
    /// </summary>
    /// <param name="seconds">Seconds to stun</param>
    public void Stun(float seconds)
    {
        stunned = true;
        // Set stun end time to be seconds from now, or extend current stun end time if we are already stunned
        stunEndTime = Mathf.Max(stunEndTime, Time.time) + seconds;
    }

    /// <summary>
    /// Prevents shooting for a time
    /// </summary>
    /// <param name="seconds">Seconds to disable</param>
    public void Disable(float seconds)
    {
        disabled = true;
        // Set disable end time to be seconds from now, or extend current disable end time if we are already disabled
        disableEndTime = Mathf.Max(disableEndTime, Time.time) + seconds;
    }

    /// <summary>
    /// Displays a floating damage text that shows how much damage we took
    /// This text appears near the player and moves around a bit
    /// </summary>
    /// <param name="dmg">The damage to display</param>
    private void DisplayDamage(float dmg)
    {
        if (!OmniController.omniController.showDamageNumbers) { return; }

        if (dmg != 0 && floatingDamageText)
        {
            GameObject dmgText = Instantiate(floatingDamageText,
                transform.position + Random.insideUnitSphere,
                Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f))
                );
            dmgText.GetComponent<TextMeshPro>().text = ((int)dmg).ToString();
            dmgText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere * 3f, ForceMode2D.Impulse);
            Destroy(dmgText, .5f);
        }
    }

}
