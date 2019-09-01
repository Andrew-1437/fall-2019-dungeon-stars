using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour {

    //Hp********************
    public float hp;    //Hp of the enemy
    public float collisionVal;  //Base damage done on a collision with the player

    //Camera Shake
    private GameObject camera;

    //Visual FX
    public GameObject explosion;

    public bool awake;
    public bool isATurret;

    private void Start()
    {
        awake = false;
        camera = GameObject.FindWithTag("MainCamera");
        if(camera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
    }

    private void Update()
    {
        if (hp <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            hp -= other.gameObject.GetComponent<ProjectileBehavior>().dmgValue;
            Destroy(other.gameObject);
            if(hp <= 0)
            {
                //DieByProjectile();
            }
        }
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Instantiate(explosion, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 12.0f);
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
    }

    private void DieByProjectile()
    {
        Destroy(gameObject);
        Instantiate(explosion, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 12.0f);  //Camera shake is proportional to collisionVal (heavier objects should shake camera more)
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
    }


}
