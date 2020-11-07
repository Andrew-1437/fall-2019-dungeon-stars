using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GM.gameController.player)
        {
            Vector3 playerPos = GM.gameController.player.transform.position;
            Vector3 direction = playerPos - transform.position;
            rb.AddForce(direction * 10f);
        }
    }
}
