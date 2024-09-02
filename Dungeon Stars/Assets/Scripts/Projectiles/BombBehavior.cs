using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : ProjectileBehavior
{
    [Header("Bomb Mechanics")]
    public GameObject payload;
    public bool enemyBomb;

    [Header("Triggers")]
    //public bool impact;
    public bool proxy;
    public bool keyUp;
    public bool keyDown;
    public bool timer;

    private float startTime;

    // Start is called before the first frame update
    protected void Start()
    {
        base.Start();
        startTime = Time.time;
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
        if(keyUp && Input.GetButtonUp("Fire3") ||
            keyDown && Input.GetButtonDown("Fire3"))
        {
            Detonate();
        }
        if(timer &&  Time.time >= (startTime + lifeTime)) 
        {
            Detonate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (proxy && !enemyBomb && other.tag == "Obstacle")
        {
            Detonate();
        }
        if (proxy && enemyBomb && other.tag == "Target")
        {
            Detonate();
        }
    }

    public void Detonate()
    {
        Instantiate(payload, transform.position, transform.rotation);
        DestroyProjectile();
    }

}
