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

    public GameObject[] weakPoints; // Array of weak point objects to be revealed in the second stage

    private Animator anim;
    private BossBehavior boss;

    bool fastStage = false;

    private new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        boss = GetComponent<BossBehavior>();
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
