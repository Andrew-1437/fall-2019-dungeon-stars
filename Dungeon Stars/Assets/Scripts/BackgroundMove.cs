using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour {

    public float speed;

    public GM gm;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (gm.gameStart)
            rb.velocity = Vector2.down * speed;
        else
            rb.velocity = Vector2.zero;
    }

}
