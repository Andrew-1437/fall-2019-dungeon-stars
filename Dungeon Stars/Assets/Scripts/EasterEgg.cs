using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public float initialTime;
    public float spinnySpeed;
    private float activeTime = 60;
    private float endTime = 1000;
    private float resetTime;
    private Rigidbody2D rb;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private int set;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        set = 0;
        activeTime = Time.time + initialTime;
        resetTime = initialTime + 300f;
        initialPos = transform.position;
        initialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= resetTime) 
        {
            Reset();
        }

        // Fly into screen
        if(set == 0 && Time.time >= activeTime)
        {
            if (transform.position.y < 0.0f)
            {
                rb.velocity = transform.up;
            }
            else
            {
                rb.velocity = Vector2.zero;
                activeTime = Time.time + Random.Range(1f, 4f);
                endTime = activeTime + Random.Range(6f, 10f);
                set++;
            }
        }
        // Rotate really fast for a bit
        else if(set == 1 && Time.time >= activeTime)
        {
            rb.angularVelocity = spinnySpeed;
            if (Time.time >= endTime)
            {
                rb.angularVelocity = 0f;
                activeTime = Time.time + Random.Range(1f, 4f);
                set++;
            }
        }
        // Fly off of screen
        else if(set == 2 && Time.time >= activeTime)
        {
            rb.velocity = transform.up;
        }
    }

    /// <summary>
    /// Resets the sprite to the original position & rotation and resets state
    /// </summary>
    private void Reset()
    {
        resetTime = Time.time + 300f;
        set = 0;
        activeTime = Time.time + initialTime;
        transform.SetPositionAndRotation(initialPos, initialRot);
        rb.velocity = Vector2.zero;
    }
}
