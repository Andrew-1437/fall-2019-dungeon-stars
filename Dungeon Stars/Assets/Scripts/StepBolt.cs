using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBolt : ProjectileBehavior
{
    public GameObject children;
    public float interval;
    float nextSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        nextSpawnTime = Time.time + interval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Instantiate(children, transform.position, transform.rotation);
            nextSpawnTime = Time.time + interval;
        }
        base.Update();
    }
}
