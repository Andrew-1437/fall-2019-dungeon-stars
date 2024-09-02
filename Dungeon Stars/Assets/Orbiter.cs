using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public Transform Origin;
    public float XRadius;
    public float YRadius;
    public float Frequency;
    public float TimeOffset;


    void FixedUpdate()
    {
        Vector3 orbit = new Vector3(
            XRadius * Mathf.Cos((Time.time * Frequency) + TimeOffset),
            YRadius * Mathf.Sin((Time.time * Frequency) + TimeOffset),
            0f);

        transform.position = orbit + Origin.position;
    }
}
