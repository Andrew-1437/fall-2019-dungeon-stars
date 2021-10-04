using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class StationBossBehavior : StationBehavior
{
    public float interval;

    public float newFireRate;

    public GameObject missileBurst;
    public float missileBurstFireRate;
    float nextMissileBurst = Mathf.Infinity;

    public GameObject[] turrets;  // Array of all turrets
    public GameObject[] weakPoints; // Array of weak point objects to be revealed in the second stage

    private List<ObstacleBehavior> destroyables;  // Combined array of turrets and weak points

    private Animator anim;
    private BossBehavior boss;

    bool fastStage = false;

    private new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        boss = GetComponent<BossBehavior>();

        destroyables = new List<ObstacleBehavior>();
        foreach (GameObject turret in turrets)
        {
            destroyables.Add(turret.GetComponent<ObstacleBehavior>());
        }
        foreach (GameObject weakPoint in weakPoints)
        {
            destroyables.Add(weakPoint.GetComponent<ObstacleBehavior>());
        }
        UpdateHp();
        GM.gameController.SetBossHpBar(boss.bossTitle, boss.hp);
    }

    private new void Update()
    {
        base.Update();
        if (!fastStage && boss.turrets <= 5)
        {
            fastStage = true;
            shootDelay = 0f;
            fireRate = newFireRate;
            nextMissileBurst = Time.time + missileBurstFireRate;
            anim.SetTrigger("FinalStage");
            RevealWeakPoints();
        }
        if(fastStage && Time.time >= nextMissileBurst)
        {
            Destroy(
                Instantiate(missileBurst, transform.position, transform.rotation), 10f);
            nextMissileBurst = Time.time + missileBurstFireRate;
        }

        UpdateHp();
    }

    protected new void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
            StartCoroutine(RotateFasterOnIntervals());
        }
    }

    public void RevealWeakPoints()
    {
        foreach (GameObject go in weakPoints)
        {
            go.GetComponent<Collider2D>().enabled = true;
            go.GetComponent<Light2D>().enabled = true;
            go.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void UpdateHp()
    {
        float newHp = 0;
        foreach (ObstacleBehavior weakPoint in destroyables)
        {
            if (weakPoint != null)
                newHp += weakPoint.hp;
        }
        boss.hp = newHp;
        gm.UpdateBossHpBar(newHp);
    }

    IEnumerator RotateFasterOnIntervals()
    {
        for(; ; )
        {
            yield return new WaitForSeconds(interval);

            anim.SetTrigger("RotateFaster");

            yield return new WaitForSeconds(6f);    // Wait for the animation to finish
        }
    }
}
