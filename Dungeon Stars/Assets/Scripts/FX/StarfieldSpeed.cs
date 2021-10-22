using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldSpeed : MonoBehaviour
{
    ParticleSystem[] emitters;
    public float initialSpeed;
    public float acceleration;
    float targetSpeed;
    float currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        emitters = GetComponentsInChildren<ParticleSystem>();
        SetSpeed(initialSpeed);
        currentSpeed = targetSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
            SpeedUp(.1f);
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            SpeedDown(.1f);

        if (targetSpeed > currentSpeed)
        {
            currentSpeed += acceleration;
            currentSpeed = Mathf.Clamp(currentSpeed, currentSpeed, targetSpeed);
        }
        else if (targetSpeed < currentSpeed)
        {
            currentSpeed -= acceleration;
            currentSpeed = Mathf.Clamp(currentSpeed, targetSpeed, currentSpeed);
        }

        foreach (ParticleSystem ps in emitters)
        {
            ParticleSystem.MainModule p = ps.main;
            p.simulationSpeed = currentSpeed;
        }
    }

    public void SpeedUp(float x)
    {
        targetSpeed += x;
    }

    public void SpeedDown(float x)
    {
        targetSpeed -= x;
    }

    public void SetSpeed(float x)
    {
        targetSpeed = x;
    }

    
}
