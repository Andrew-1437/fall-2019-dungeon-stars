﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

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
    private bool shieldDown;    //True if shields are disabled -"Shields offline!"
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

    //PowerUp Mods
    public float dmgMod;    //Value to modify damage taken, for things like armor (1 = full damage, 0 = no damage, >1 = Extra damage, <1 = Healing??)
    private float fireRateMod;
    private float fireRateEnd;

    private float speedMod;
    private float speedEnd;

    private float shieldBoostEnd;

    
    //Camera Shake
    private GameObject camera;

    //GM
    private GameObject gm;

    //Visual FX************
    [Header("Referenced Game Objects")]
    public GameObject explosionFx;
    public ParticleSystem speedFX;
    public ParticleSystem fireRateFX;

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

        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Camera not found by player!");
        }
        
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by player!");
        }
        

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
        gameObject.GetComponent<Rigidbody2D>().velocity = move * speed * speedMod;
        gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0.0f,horizontal * rotate, 0.0f);

    }

    void Update()
    {
        

        //Primary Fire
        if(Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate * fireRateMod;
            Instantiate(primary[level], spawner.position, spawner.rotation);
        }

        //Secondary Fire
        if(Input.GetButton("Fire2") && Time.time > nextSecondary)
        {
            nextSecondary = Time.time + secondaryFireRate * fireRateMod;
            Instantiate(secondary[level], spawner.position, spawner.rotation);
        }

        if(Input.GetButtonDown("Fire3"))
        {
            Instantiate(explosive, spawner.position, spawner.rotation);
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
            fireRateFX.Stop();
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
            float dmg = other.gameObject.GetComponent<ProjectileBehavior>().dmgValue * dmgMod;
            shield -= dmg;  //All damage hits shield first
            if(!shieldDown)
            {
                shieldOpacity = 1.0f;
                ShieldFlash(shieldRef, shieldOpacity);
                shieldRegenTime = Time.time + shieldRegenDelay;
            }
            if(shield < 0 && !shieldDown)
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
            else if(shield < 0) {
                hp += shield;   //Excess damage to shield carries over. If shield is already 0, this does full damage to hp
                shield = 0;
            }

            camera.GetComponent<CameraShaker>().SmallShake(); ;
            Destroy(other.gameObject);
            
        }

        if(other.tag == "EnemyMissile") //Damage from missiles will be done by the MissileExplosion object
        {
            camera.GetComponent<CameraShaker>().SmallShake(); ;
            Destroy(other.gameObject);
        }

        if(other.tag == "Obstacle")
        {
            //Total collision dmg = collision value of other * (player speed + other speed)
            float collisionDmg = other.gameObject.GetComponent<ObstacleBehavior>().collisionVal 
                * (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude 
                + other.GetComponent<Rigidbody2D>().velocity.magnitude);
            hp -= collisionDmg;
            other.gameObject.GetComponent<ObstacleBehavior>().hp -= collisionDmg;
            audio[2].Play();
            camera.GetComponent<CameraShaker>().LargeShake();
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
        if(other.tag == "Repair")
        {
            hp = maxHp;
            shield = maxShield;
            shieldDown = false;
            shieldOpacity = 4.0f;
            ShieldFlash(shieldRef, shieldOpacity);
            audio[3].Play();
            Destroy(other.gameObject);
        }
        if(other.tag == "FireRateBoost")
        {
            fireRateMod = 0.5f;
            audio[3].Play();
            fireRateEnd = Time.time + other.GetComponent<PowerUpBehavior>().duration;
            Destroy(other.gameObject);
            fireRateFX.Play();
        }
        if(other.tag == "SpeedBoost")
        {
            speedMod = 1.5f;
            audio[3].Play();
            speedEnd = Time.time + other.GetComponent<PowerUpBehavior>().duration;
            Destroy(other.gameObject);
            speedFX.Play();
        }
        if (other.tag == "LevelUp")
        {
            levelUp();
            audio[3].Play();
            Destroy(other.gameObject);

        }
        /*
        if (other.tag == "ShieldBoost")
        {
            shield = 200;
            audio[3].Play();
            shieldBoostEnd = Time.time + other.GetComponent<PowerUpBehavior>().duration;
            Destroy(other.gameObject);
            audio[1].Play();
        }
        */
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
    /*
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Laser")
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
        }

        //Ripperoni
        if (hp < 0)
        {
            Die();
        }
    }
    */

    //Kills player "Ripperoni"
    public void Die()
    {
        camera.GetComponent<CameraShaker>().HugeShake();
        Destroy(gameObject);
        Instantiate(explosionFx, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
        gm.GetComponent<GM>().DeathText();
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

    

}
