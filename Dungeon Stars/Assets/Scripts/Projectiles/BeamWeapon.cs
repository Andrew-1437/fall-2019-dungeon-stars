﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    public float sync;

    public float aimTime;       // Time before beam "charges up" where it can aim
    public float targetTime;    // Time beam "charges up" before it shoots where it stops aiming
    public float shootTime;     // Time while beam is doing firing animation where it doesnt aim or do anything

    float aimEndTime;
    float targetEndTime;
    float shootEndTime;

    public float turn;
    float turnSpeedMod = 1f;
    GameObject target;

    public GameObject damager;
    float damageTime = .2f;
    float damageEndTime;
    bool firing = false;

    public AudioSource chargeSFX;
    public AudioSource fireSFX;

    bool aim = true;

    // Auto mode is when the turret fires on its own on a set interval. Use Automatic(bool) to set this during runtime
    bool auto = false;
    public bool autoOnAwake;    // Start in automatic mode
    

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        Automatic(autoOnAwake);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > aimEndTime)
        {
            anim.SetTrigger("LockTarget");
            chargeSFX.Play();
            aim = false;

            // If in auto mode, set the next aimEndTime to what it should be. Otherwise, don't so that we don't shoot again
            aimEndTime = auto ? 
                shootEndTime + aimTime * OmniController.omniController.enemyFireRateScale : 
                Mathf.Infinity;

        }
        else if (Time.time > targetEndTime)
        {
            anim.SetTrigger("Fire");
            fireSFX.Play();
            damageEndTime = Time.time + damageTime;
            firing = true;

            // Same logic as with aimEndTime
            targetEndTime = auto ? 
                aimEndTime + targetTime * OmniController.omniController.enemyFireRateScale : 
                Mathf.Infinity;
        }
        else if (Time.time > shootEndTime)
        {
            aim = true;

            // Same logic as with aimEndTime
            shootEndTime = auto ? 
                targetEndTime + shootTime : 
                Mathf.Infinity;
        }

        if (aim) { RotateTowards(Tags.Player); }

        if (firing && Time.time > damageEndTime) firing = false;

        damager.SetActive(firing);

    }

    void RotateTowards(string tag)
    {
        if (turn == 0) { return; }

        target = FindClosestByTag("Player");
        
        if (target != null)
        {
            // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
            if (target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                turnSpeedMod = .35f;
            else
                turnSpeedMod = 1f;

            Vector3 targetDir = target.transform.position - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime * turnSpeedMod);
        }
    }

    // Fire the weapon by cycling through its fire states once (charge, shoot)
    public void FireCycleOnce()
    {
        aimEndTime = Time.time;
        targetEndTime = aimEndTime * OmniController.omniController.enemyFireRateScale + targetTime;
        shootEndTime = targetEndTime + shootTime;
    }

    // Set automatic firing of the weapon
    public void Automatic(bool state)
    {
        if(state)
        {
            aimEndTime = Time.time + aimTime * OmniController.omniController.enemyFireRateScale + sync;
            targetEndTime = aimEndTime + targetTime * OmniController.omniController.enemyFireRateScale;
            shootEndTime = targetEndTime + shootTime;
        }
        else
        {
            aimEndTime = Mathf.Infinity;
            targetEndTime = Mathf.Infinity;
            shootEndTime = Mathf.Infinity;
        }
        auto = state;
    }

    GameObject FindClosestByTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (tag == "Player")
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            else
            {
                if (go.GetComponent<ObstacleBehavior>().awake)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < distance)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }
}
