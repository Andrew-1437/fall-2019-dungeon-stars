using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    public float pushbackDistance;
    public float collisionDamage;
    public float speed;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GM.gameController.gameStart)
            rb.velocity = Vector2.down;
    }
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            // Stun the player
            pc.Stun(.05f);
            pc.HullDamage(collisionDamage * Mathf.Max(1f, collision.relativeVelocity.magnitude));
            // Set the player's velocity away from the wall
            // The player should jerk away from the wall but not teleport away
            collision.rigidbody.velocity = 
                (Vector3)collision.GetContact(0).normal * 
                Mathf.Min(-4f, -collision.relativeVelocity.magnitude) * pushbackDistance;

            
        }
    }
}
