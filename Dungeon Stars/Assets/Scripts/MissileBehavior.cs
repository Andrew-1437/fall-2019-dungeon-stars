﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour {

    public float speed; //Speed of projectile
    public float turn; //Turning speed of projectile
    public string targetTag;
    private GameObject target;

    public GameObject explosion;
    
    public float lifeTime;
    private float deathTime;

    private void Start()
    {
        deathTime = Time.time + lifeTime;
        target = FindClosestByTag(targetTag);
    }

    void Update()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Transform>().up * speed;

        
        if (target != null)
        {
            Vector3 targetDir = target.GetComponent<Transform>().position - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime);
        }
        else { target = FindClosestByTag(targetTag); }

        if (Time.time >= deathTime)
        {
            Destroy(gameObject);
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == targetTag)
        {
            Destroy(gameObject);
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    GameObject FindClosestByTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (tag == "Player")
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            else
            {
                if (go.GetComponent<ObstacleBehavior>().awake)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < distance)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }
}
