using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

    //Camera Shake
    public float cameraShakeMod;
    private float shakeIntensity;
    public float smallShake;    //I'll have a small shake please
    public float largeShake;    //I'll have a large shake please

    // Use this for initialization
    void Start () {
        shakeIntensity = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        //Camera Shake
        if (OmniController.omniController.enableCameraShake)
        {
            if (shakeIntensity > 0)
            {
                ShakeCamera(shakeIntensity);
                shakeIntensity -= 1.4f * Time.deltaTime;
            }
            else
            {
                transform.position = new Vector2(0.0f, 0.0f);
                shakeIntensity = 0.0f;
            }
        }
    }

    private void ShakeCamera(float intensity)
    {
        transform.position = Random.insideUnitCircle * intensity * shakeIntensity;
    }

    public void SmallShake()
    {
        shakeIntensity = smallShake;
    }

    public void LargeShake()
    {
        shakeIntensity = largeShake;
    }

    public void LargeShake(float mod)
    {
        shakeIntensity = largeShake + mod;
    }

    public void HugeShake()
    {
        shakeIntensity = largeShake + 1;
    }

    public void CustomShake(float shake)
    {
        shakeIntensity = shake;
    }
}
