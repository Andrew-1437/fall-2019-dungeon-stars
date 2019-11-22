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


    //HP & Shields***********
    [Header("HP & Shields")]
    public float maxHp; //Represents max hp
    public float maxShield; //Represents max shields

    public float hp;   //Represents current hp
    public float shield;   //Represents current shields

    public GameObject shieldRef;
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
    private GameObject camera;

    //GM
    private GM gm;

    //Visual FX************
    [Header("Referenced Game Objects")]
    public GameObject explosionFx;
    public ParticleSystem speedFX;
    public ParticleSystem fireRateFX;
    public ParticleSystem smokeFX;

    //Sound FX**************
    private AudioSource[] audio;

    //Initialize**********
    void Start()
    {
        hp = maxHp;
        shield = maxShield;
        shieldDown = false;
        shieldOpacity = 0.0f;
        shieldRef.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, shieldOpacity);

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
        /*
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by player!");
        } */

        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();
    }

    //Movement***********
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //rotation -= horizontal;
        Vector2 move = new Vector2(horizontal, vertical);
        if(move.magnitude > 1)
        {
            move.Normalize();
        }
        rb.velocity = move * speed * speedMod;
        transform.rotation = Quaternion.Euler(0.0f,horizontal * rotate, 0.0f);

        Vector2 position = transform.position;
        if (position.x>26.97)
        {
            position.x = (float)26.97;
        } else if(position.x<-26.97)
        {
            position.x = (float)-26.97;
        }
        if (position.y>12.6)
        {
            position.y = (float)12.6;
        } else if (position.y<-12.6)
        {
            position.y = (float)-12.6; 
        }
        transform.position = position;
    }

    void Update()
    {
        

        //Primary Fire
        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate * fireRateMod * heatMod;
            Instantiate(primary[level], spawner.position, spawner.rotation);
            gm.score -= weapon1Cost;
            if (gm.score<0)
            {
                gm.score = 0;
            }
            if(enableHeat && primaryUsesHeat)
            {
                heat += primHeatGen * heatGenMod;
            }
        }

        //Secondary Fire
        if(Input.GetButton("Fire2") && Time.time > nextSecondary)
        {
            nextSecondary = Time.time + secondaryFireRate * fireRateMod * heatMod;
            Instantiate(secondary[level], spawner.position, spawner.rotation);
            gm.score -= weapon2Cost;
            if (gm.score < 0)
            {
                gm.score = 0;
            }
            if (enableHeat && secondaryUsesHeat)
            {
                heat += secHeatGen * heatGenMod;
            }
        }

        if(Input.GetButtonDown("Fire3"))
        {
            if (currentMissileCount>0)
            {
                Instantiate(explosive, spawner.position, spawner.rotation);
                currentMissileCount--;
            }
            
        }

        if(enableHeat)
        {
            heat -= heatDisperse * (1f/heatGenMod);
            heat = Mathf.Clamp(heat, 0f, 100f);

            if(heat >= 80f)
            {
                //print("OVERHEAT");
                hullDamage(maxHp * .001f);

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
        //Quick Level Up
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            levelUp();
        }
        //Self Destruct
        if (hp <= 0 || Input.GetKeyDown("backspace"))
        {
            Die();
        }
        //Break Shields 
        if (Input.GetKeyDown("delete") && !shieldDown)
        {
            //Mark shield is down and set time when shield returns
            shieldDown = true;
            shieldUpTime = Time.time + shieldDelay;
            shield = 0;
            shieldOpacity = 3.0f;
            ShieldFlashRed(shieldRef, shieldOpacity);
            gameObject.GetComponent<AudioSource>().Play();
        }
        //Freeze Time
        if (Input.GetKeyDown(KeyCode.Pause) && Time.timeScale == 1f)
        {
            Time.timeScale = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Pause) && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
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

        //Shield mechanics************
        if (shieldDown)
        {
            //After we have reached the time when shield is back up, shield returns with 50% hp
            if(Time.time >= shieldUpTime)
            {
                shieldDown = false;
                shield = maxShield / 2;
                shieldOpacity = 4.0f;
                ShieldFlash(shieldRef, shieldOpacity);
                audio[1].Play();
            }
        }
        if(shield < maxShield && !shieldDown && Time.time >= shieldRegenTime)
        {
            shield += shieldRecharge;
        }
        if(shield > maxShield && Time.time > shieldBoostEnd)
        {
            shield = maxShield;
        }
        if(shield > maxShield && Time.time < shieldBoostEnd)
        {
            shieldOpacity = 2.0f;
        }

        //Shield Flashy stuff
        if(shieldOpacity > 0 && !shieldDown)
        {
            shieldOpacity -= 0.1f;
            ShieldFlash(shieldRef, shieldOpacity);
        }
        if (shieldOpacity > 0 && shieldDown)
        {
            shieldOpacity -= 0.1f;
            ShieldFlashRed(shieldRef, shieldOpacity);
        }

        //PowerUps
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

        //FX
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
    }

    //Collisions and damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Damage from projectiles
        if(other.tag == "EnemyProjectile")
        {
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue;

            damage(dmg);

            camera.GetComponent<CameraShaker>().SmallShake();
            if (!other.gameObject.GetComponent<ProjectileBehavior>().perist)
            {
                Destroy(other.gameObject);
            }
            
        }

        if(other.tag == "EnemyMissile") //Damage from missiles will be done by the MissileExplosion object
        {
            camera.GetComponent<CameraShaker>().SmallShake(); ;
            Destroy(other.gameObject);
        }

        // Collision damage
        if(other.tag == "Obstacle")
        {
            // Can't collide with a turret
            if (!other.gameObject.GetComponent<ObstacleBehavior>().isATurret)
            {
                //Total collision dmg = collision value of other * (player speed + other speed)
                float collisionDmg = other.gameObject.GetComponent<ObstacleBehavior>().collisionVal
                    * (rb.velocity.magnitude + other.GetComponent<Rigidbody2D>().velocity.magnitude);
                hullDamage(collisionDmg);
                other.gameObject.GetComponent<ObstacleBehavior>().hp -= collisionDmg;
                audio[2].Play();
                camera.GetComponent<CameraShaker>().LargeShake();
            }
        }

        if (other.tag == "Laser")
        {
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            shield -= dmg;  //All damage hits shield first
            if (!shieldDown)
            {
                shieldOpacity = 1.0f;
                ShieldFlash(shieldRef, shieldOpacity);
                shieldRegenTime = Time.time + shieldRegenDelay;
            }
            if (shield < 0 && !shieldDown)
            {
                //Mark shield is down and set time when shield returns
                shieldDown = true;
                shieldUpTime = Time.time + shieldDelay;
                hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
                shield = 0;
                shieldOpacity = 3.0f;
                ShieldFlashRed(shieldRef, shieldOpacity);
                gameObject.GetComponent<AudioSource>().Play();
            }
            else if (shield < 0)
            {
                hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
                shield = 0;
            }
            camera.GetComponent<CameraShaker>().SmallShake();
        }

        //PowerUps
        if (other.tag == "PowerUp")
        {
            OmniController.omniController.powerUpsCollected++;
            PowerUpBehavior pow = other.gameObject.GetComponent<PowerUpBehavior>();
            if (pow.type == PowerUpBehavior.PowerUps.Repair)
            {
                hp = maxHp;
                shield = maxShield;
                shieldDown = false;
                shieldOpacity = 4.0f;
                ShieldFlash(shieldRef, shieldOpacity);
                
            }
            if (pow.type == PowerUpBehavior.PowerUps.FireUp)
            {
                fireRateMod = 0.5f;
                heatGenMod = 0.2f;
                fireRateEnd = Time.time + pow.duration;
                fireRateFX.Play();
            }
            if (pow.type == PowerUpBehavior.PowerUps.SpeedUp)
            {
                speedMod = 1.5f;
                speedEnd = Time.time + pow.duration;
                speedFX.Play();
            }
            if (pow.type == PowerUpBehavior.PowerUps.LevelUp)
            {
                levelUp();
            }
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
        if (hp < 0)
        {
            Die();
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        // Continuous damage
        if(other.tag == "Dps")
        {
            damage(other.gameObject.GetComponent<ProjectileBehavior>().dmgValue);
        }
    }
    

    //Kills player "Ripperoni"
    public void Die()
    {
        OmniController.omniController.timesDied++;
        Destroy(gameObject);
        Instantiate(explosionFx, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().HugeShake();
        gm.DeathText();
        gm.score -= dieCost;
        if (gm.score < 0)
        {
            gm.score = 0;
        }
    }

    private void ShieldFlash(GameObject shieldRef, float opacity)
    {
        shieldRef.GetComponent<MeshRenderer>().material.color = new Color(120.0f / 255.0f, 200.0f / 255.0f, 220.0f / 255.0f, opacity);
    }

    private void ShieldFlashRed(GameObject shieldRef, float opacity)
    {
        shieldRef.GetComponent<MeshRenderer>().material.color = new Color(120.0f, 0.0f, 0.0f, opacity);
    }

    private void levelUp()
    {
        level++;
        if (level >= primary.Length || level >= secondary.Length)
        {
            level = primary.Length - 1;
        }
        else
        {
            maxHp = maxHp * 1.05f;
            hp += maxHp * 0.05f;
            maxShield = maxShield * 1.05f;
            shield += maxShield * 0.05f;
        }
    }

    // Take damage normally. First absorbed by shield, then hull
    public void damage(float baseDmg)
    {
        float dmg = baseDmg * dmgMod;

        shield -= dmg;  //All damage hits shield first
        if (!shieldDown)
        {
            shieldOpacity = 1.0f;
            ShieldFlash(shieldRef, shieldOpacity);
            shieldRegenTime = Time.time + shieldRegenDelay;
        }
        if (shield <= 0 && !shieldDown)
        {
            //Mark shield is down and set time when shield returns
            shieldDown = true;
            shieldUpTime = Time.time + shieldDelay;
            hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
            shield = 0;
            shieldOpacity = 3.0f;
            ShieldFlashRed(shieldRef, shieldOpacity);
            gameObject.GetComponent<AudioSource>().Play();
        }
        else if (shield <= 0)
        {
            hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
            shield = 0;
        }
    }

    // Damage IGNORES shields. Directly to hull
    public void hullDamage(float baseDmg)
    {
        float dmg = baseDmg * dmgMod;

        hp -= dmg;
    }

    

}
