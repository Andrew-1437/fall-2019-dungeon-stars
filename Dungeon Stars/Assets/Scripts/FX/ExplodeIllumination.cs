using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ExplodeIllumination : MonoBehaviour
{
    public float startIntensity;
    public float endIntensity;
    public float duration;
    float startingTime;
    float currIntensity = 0;

    Light2D illumination;

    // Start is called before the first frame update
    void Start()
    {
        illumination = GetComponent<Light2D>();
        currIntensity = startIntensity;
        startingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        illumination.intensity = Mathf.Max(0f,
            startIntensity - ((Time.time - startingTime) / duration) * startIntensity
            );
    }
}
