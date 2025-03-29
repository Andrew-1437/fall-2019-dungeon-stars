using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMoveEnemyBehavior : StdEnemyBehavior
{
    public float Amplitude;
    public float Frequency;
    float startTime;

    private new void FixedUpdate()
    {
        if (!gm.gameStart) { return; }

        if (!awake)
        {
            rb.velocity = Vector2.down;
            return;
        }

        rb.velocity = (Vector2.down * speed) + 
            (Vector2.right * Mathf.Cos(Frequency * (Time.time - startTime)) * Amplitude);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        startTime = Time.time;
    }
}
