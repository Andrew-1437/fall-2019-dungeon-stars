using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBolt : ProjectileBehavior
{
    public float amplitude;
    public float frequency;

    uint t = 0;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + transform.up * speed + transform.right * F(t);
        t++;
    }

    private float F(uint t)
    {
        return amplitude * Mathf.Sin(frequency * t);
    }
}
