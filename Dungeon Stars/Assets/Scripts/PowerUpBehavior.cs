using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour {

    public float speed;
    public float duration;

    private bool awake;

    private GameObject gm;

    private void Start()
    {
        awake = false;

        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by PowerUp!");
        }
    }

    private void FixedUpdate()
    {
        if (gm.GetComponent<GM>().gameStart)
        {
            if (awake)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = (gameObject.GetComponent<Transform>().up * speed) + new Vector3(0.0f, -1.0f, 0.0f);
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
