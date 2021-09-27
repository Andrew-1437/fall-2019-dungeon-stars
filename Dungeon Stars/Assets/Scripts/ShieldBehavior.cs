using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : ObstacleBehavior
{
    Animator anim;

    protected new void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.tag == "Projectile")
            anim.SetTrigger("ShieldHit");
        base.OnTriggerEnter2D(collision);
    }
}
