using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : ProjectileBehavior
{
    [Header("Bomb Mechanics")]
    public GameObject payload;

    [Header("Triggers")]
    //public bool impact;
    public bool proxy;
    public bool keyUp;
    public bool keyDown;

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
        if(keyUp && Input.GetButtonUp("Fire3") ||
            keyDown && Input.GetButtonDown("Fire3"))
        {
            Detonate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(proxy && other.tag == "Enemy")
        {
            Detonate();
        }
    }

    public void Detonate()
    {
        Instantiate(payload, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
