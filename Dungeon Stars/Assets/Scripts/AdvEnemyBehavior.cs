using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvEnemyBehavior : StdEnemyBehavior
{
    public GameObject extraProjectile;
    public Transform extraHardpoint;
    public float extraShootDelay;
    public float extraFireRate;
    public float extraBurstTime;
    private float burstEnd = 0;
    private float nextBurst = 0;
    private float nextFire = 0;

    protected new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + extraShootDelay;
                burstEnd = Time.time + extraBurstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Destroy(
                    Instantiate(extraProjectile, extraHardpoint.position, extraHardpoint.rotation), 5f);
                nextFire = Time.time + extraFireRate * OmniController.omniController.enemyFireRateScale;
            }
        }
    }
}
