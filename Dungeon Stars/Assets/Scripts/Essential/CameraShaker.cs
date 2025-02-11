﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

    //Camera Shake
    public float cameraShakeMod;
    private float shakeIntensity;
    public float smallShake;    //I'll have a small shake please
    public float largeShake;    //I'll have a large shake please
    private Vector3 origPos;

    // Use this for initialization
    void Start () 
    {
        origPos = transform.position;
        shakeIntensity = 0.0f;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        //Camera Shake
        if (OmniController.omniController.enableCameraShake)
        {
            if (shakeIntensity > 0)
            {
                ShakeCamera(shakeIntensity);
                shakeIntensity -= Mathf.Max(1.9f, 3f * shakeIntensity) * Time.deltaTime;
            }
            else
            {
                transform.position = origPos;
                shakeIntensity = 0.0f;
            }
        }
    }

    private void ShakeCamera(float intensity)
    {
        transform.position = (Vector2)origPos + (Random.insideUnitCircle * Mathf.Min(intensity, .4f));
    }

    public void SmallShake()
    {
        shakeIntensity += smallShake;
    }

    public void LargeShake()
    {
        shakeIntensity += largeShake;
    }

    public void LargeShake(float mod)
    {
        shakeIntensity += largeShake + mod;
    }

    public void HugeShake()
    {
        shakeIntensity += largeShake + 1;
    }

    public void CustomShake(float shake)
    {
        shakeIntensity += shake;
    }
}
