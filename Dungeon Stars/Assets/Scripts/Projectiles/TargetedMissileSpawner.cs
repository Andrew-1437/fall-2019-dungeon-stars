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

    #region Bounds
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    #endregion

    #region Boolean Flags
    public bool awake;
    [Tooltip("If true, only fire a burst when the FireBurst() method is called")]
    public bool onlyTrigger;    
    [Tooltip("If true, the missiles will be randomized around the player. \n" +
        "If false, they are random around the screen")]
    public bool targetPlayer = false;
    #endregion

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
        if (!onlyTrigger && !awake && Time.time > burstBegin)
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

    // Fires a single missile at a randomized location
    public void FireMissile()
    {
        if (GM.GameController.player && targetPlayer)
        {
            Destroy(
                Instantiate(missiles, 
                GM.GameController.player.transform.position + Random.insideUnitSphere * 15f, 
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

    // Immediately fires a single burst
    public void FireBurst()
    {
        awake = true;
        burstBegin = Time.time;
        burstEnd = burstBegin + burstTime;
    }

    // Simultaniously fires x missiles at random positions
    public void FireSimultanious(int x)
    {
        for(int i = 0; i < x; i++)
        {
            FireMissile();
        }
    }
}
