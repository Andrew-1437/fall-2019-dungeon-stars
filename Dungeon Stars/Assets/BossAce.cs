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


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("FlyAcrossScreen");
            aceFlowchart.SendFungusMessage("FlyAcrossScreen");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("FlyScreenTopAimDown");
            aceFlowchart.SendFungusMessage("FlyScreenTopAimDown");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetTrigger("FlyScreenLeft");
            aceFlowchart.SendFungusMessage("FlyScreenLeft");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            animator.SetBool("FlyToScreenBottom", !animator.GetBool("FlyToScreenBottom"));
        }

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

        base.Update();
    }

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
}
