using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedMissileSpawner : MonoBehaviour
{
    public GameObject missiles;
    public float fireRate;
    float nextFire = Mathf.Infinity;

    public float burstTime;
    public float burstOffTime;
    float burstBegin = Mathf.Infinity;
    float burstEnd = Mathf.Infinity;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public bool awake;
    public bool targetPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        nextFire = Time.time + fireRate;
        burstBegin = Time.time + burstOffTime;
        burstEnd = burstBegin + burstTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!awake && Time.time > burstBegin)
            awake = true;
        if(awake && Time.time > burstEnd)
        {
            awake = false;
            burstBegin = burstEnd + burstOffTime;
            burstEnd = burstBegin + burstTime;
        }

        if (awake)
        {
            if(Time.time > nextFire)
            {
                FireMissile();
                nextFire = Time.time + fireRate;
            }
        }
    }

    public void FireMissile()
    {
        if (GM.gameController.player && targetPlayer)
        {
            Destroy(
                Instantiate(missiles, 
                GM.gameController.player.transform.position + Random.insideUnitSphere * 15f, 
                transform.rotation), 1.5f);
        }
        else
        {
            Vector3 pos = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0);

            Destroy(
                Instantiate(missiles, pos, transform.rotation), 1.5f);
        }
    }
}
