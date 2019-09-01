using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerditionBeamFX : MonoBehaviour {

    public GameObject beam;
    private float opacity;

    private void Start()
    {
        opacity = 4.0f;
        SetOpacity(opacity);
    }

    private void Update()
    {
        opacity -= 0.01f;
        SetOpacity(opacity);
        if(opacity <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void SetOpacity(float opacity)
    {
        beam.GetComponent<MeshRenderer>().material.color = new Color(160.0f / 255.0f, 80.0f / 255.0f, 0.0f, opacity);
    }
}
