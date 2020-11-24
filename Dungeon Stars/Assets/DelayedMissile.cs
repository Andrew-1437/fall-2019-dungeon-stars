using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedMissile : MissileBehavior
{
    public float delay;
    bool thrust;
    float timeToThrust;
    float actualSpeed;

    public AudioSource thrustSound;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        timeToThrust = Time.time + delay;
        actualSpeed = speed;
        speed = 0f;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(Time.time > timeToThrust && !thrust)
        {
            thrust = true;
            turn = 0f;
            speed = actualSpeed;
            thrustSound.Play();
        }

        base.Update();
    }
}
