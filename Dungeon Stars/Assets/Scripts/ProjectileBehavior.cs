using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    public float speed; //Speed of projectile
    public float dmgValue; //Damage projectile will do
    public float lifeTime;
    private float deathTime;
    public bool perist;

    private void Start()
    {
        deathTime = Time.time + lifeTime;
    }

    void FixedUpdate () {
        transform.position = transform.position + transform.up * speed;
	}

    private void Update()
    {
        if(Time.time >= deathTime)
        {
            Destroy(gameObject);
        }
    }



}
