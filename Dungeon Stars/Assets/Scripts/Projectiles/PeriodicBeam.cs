using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.LightningBolt;

public class PeriodicBeam : ProjectileBehavior
{
    [Header("Periodic Beam")]
    public LineRenderer lr;
    public LightningBoltScript bolt;
    public ParticleSystem particleL;
    public ParticleSystem particleR;
    Collider2D coll;

    public float waitTime;
    public float chargeTime;
    public float fireTime;
    float chargeBegin;
    float fireBegin;
    float waitBegin;
    int stage;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        coll = GetComponent<Collider2D>();

        stage = -1;
        coll.enabled = false;
        lr.enabled = false;
        chargeBegin = Time.time + waitTime * 2f;

        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        // Not firing
        if (stage == 0 && Time.time >= chargeBegin)
        {
            //print("Prepping to fire...");
            stage = 1;
            bolt.ChaosFactor = 0f;
            lr.enabled = true;
            lr.widthMultiplier = .2f;
            fireBegin = Time.time + chargeTime;
            particleL.Play();
            particleR.Play();
        }
        else if (stage == 1 && Time.time >= fireBegin)
        {
            //print("Firing!");
            stage = 2;
            bolt.ChaosFactor = .01f;
            lr.widthMultiplier = 4f;
            coll.enabled = true;
            waitBegin = Time.time + fireTime;
            particleL.Stop();
            particleR.Stop();
        }
        else if (stage == 2 && Time.time >= waitBegin)
        {
            //print("Stopped firing.");
            stage = -1;
            
            lr.enabled = false;
            coll.enabled = false;
            chargeBegin = Time.time + waitTime;

            
        }
        //print(stage);
    }

    public void StartNow()
    {
        stage = 1;
        bolt.ChaosFactor = 0f;
        lr.enabled = true;
        lr.widthMultiplier = .2f;
        fireBegin = Time.time + chargeTime;

        particleL.Play();
        particleR.Play();
    }
}
