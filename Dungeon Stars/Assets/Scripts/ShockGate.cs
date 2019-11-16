using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockGate : ProjectileBehavior
{
    public GameObject endpoint1;
    public GameObject endpoint2;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        // If one of the two endpoints are destroyed, do not damage
        if(endpoint1 == null || endpoint2 == null)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<LineRenderer>().enabled = false;
        }
    }
}
