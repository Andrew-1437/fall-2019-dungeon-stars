using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisBehavior : MonoBehaviour {

    public float speed;
    public float rotate;

    private float angVel;

    private bool awake;

    private GameObject gm;

    private void Start()
    {
        angVel = Random.value * rotate;

        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by debris!");
        }
    }

    private void FixedUpdate()
    {
        if (gm.GetComponent<GM>().gameStart)
        {
            if (awake)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -speed);
                GetComponent<Rigidbody2D>().angularVelocity = angVel;
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -1.0f);
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
