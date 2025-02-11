﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBehavior : MonoBehaviour {

    public float speed;
    public float rotate;

    private float angVel;

    private bool awake;

    private GM gm;
    private Rigidbody2D rb;

    private void Start()
    {
        angVel = Random.Range(-1*rotate, rotate);

        gm = GM.GameController;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (gm.gameStart)
        {
            if (awake)
            {
                rb.velocity = new Vector2(0.0f, -speed);
                rb.angularVelocity = angVel;
            }
            else
            {
                rb.velocity = new Vector2(0.0f, -1.0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }
}
