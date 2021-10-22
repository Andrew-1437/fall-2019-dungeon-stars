using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityBehavior : ProjectileBehavior
{
    [Header("Singularity")]
    public float force;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            StdEnemyBehavior enemy = other.gameObject.GetComponent<StdEnemyBehavior>();
            if(enemy)
            {
                enemy.Stun(1f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            if(rb)
            {
                Vector3 forceVector = Vector3.Normalize(transform.position - rb.gameObject.transform.position);
                rb.AddForce(forceVector * force);
            }
        }
    }
}
