using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAce : BossBehavior
{
    Animator animator;

    [Header("Ace Specific Features")]
    public Fungus.Flowchart aceFlowchart;

    [Header("Ace Weapons")]
    public GameObject delayedMissiles;
    public float missileFireRate;
    public bool fireMissiles;
    float nextMissileTime = 0;

    public GameObject basicAttack;
    public float basicFireRate;
    public bool fireBasicAttack;
    float nextAttackTime = 0;

    public GameObject horizAttack;

    public TurretBehavior frontTurret;
    public float frontTurretFireRate;
    public bool fireFrontTurret;
    float nextFTurretAttackTime = 0;

    public TurretBehavior leftTurret;
    public TurretBehavior rightTurret;
    public float sideTurretsFireRate;
    public bool fireSideTurrets;
    float nextSTurretsAttackTime = 0;

    public GameObject dualMissiles;
    public GameObject flak180;

    float halfHp;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        halfHp = hp / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        // Handles shooting of weapons. Fungus toggles bool which fires corresponding weapons if true
        if(fireMissiles && Time.time > nextMissileTime)
        {
            Instantiate(delayedMissiles, transform.position, transform.rotation);
            nextMissileTime = Time.time + missileFireRate;
        }

        if (fireBasicAttack && Time.time > nextAttackTime)
        {
            Instantiate(basicAttack, transform.position, transform.rotation);
            nextAttackTime = Time.time + basicFireRate;
        }

        if(fireFrontTurret && Time.time > nextFTurretAttackTime)
        {
            frontTurret.Fire();
            nextFTurretAttackTime = Time.time + frontTurretFireRate;
        }
        if (fireSideTurrets && Time.time > nextSTurretsAttackTime)
        {
            leftTurret.Fire();
            rightTurret.Fire();
            nextSTurretsAttackTime = Time.time + sideTurretsFireRate;
        }

        base.Update();
    }

    // Method called by Fungus Flowchart on game object. 
    // It activates the animator and fungus flowchart for the corresponding move.
    // This keeps them in sync with each other.
    public void TriggerAction(int i)
    {
        // Fly across the screen and shoot delayed missiles
        if (i == 0)
        {
            animator.SetTrigger("FlyAcrossScreen");
            aceFlowchart.SendFungusMessage("FlyAcrossScreen");
        }
        // Fly to the top of the screen, aim downwards, and shoot BasicAttack
        else if (i == 1)
        {
            animator.SetTrigger("FlyScreenTopAimDown");
            aceFlowchart.SendFungusMessage("FlyScreenTopAimDown");
        }
        // Fly to the left of the screen and fire three wide bolts
        else if (i == 2)
        {
            animator.SetTrigger("FlyScreenLeft");
            aceFlowchart.SendFungusMessage("FlyScreenLeft");
        }


        // At less than half hp, when we begin a move, shoot a flak 180 and 2 missiles
        if(hp < halfHp)
        {
            Instantiate(dualMissiles, transform.position, transform.rotation);
            Instantiate(flak180, transform.position, transform.rotation);
        }
    }

    // Helper functions for Fungus to call which activate the shooting of weapons
    public void SetFireMissiles(bool trigger)
    {
        fireMissiles = trigger;
    }

    public void SetFireBasicAttack(bool trigger)
    {
        fireBasicAttack = trigger;
    }

    public void FireHorizAttack()
    {
        Instantiate(horizAttack, transform.position, transform.rotation);
    }

    public void SetFireFrontTurret(bool trigger)
    {
        fireFrontTurret = trigger;
    }

    public void SetFireSideTurrets(bool trigger)
    {
        fireSideTurrets = trigger;
    }
}
