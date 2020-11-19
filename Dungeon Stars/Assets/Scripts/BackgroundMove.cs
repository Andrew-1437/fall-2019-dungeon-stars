using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour {

    public float speed;

    GM gm;

    private Rigidbody2D rb;

    private void Start()
    {
        gm = GM.gameController;
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
