using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour {
    //GM
    private GM gm;
    
    //Hp********************
    public float hp;    //Hp of the enemy
    public float collisionVal;  //Base damage done on a collision with the player

    //Camera Shake
    private GameObject camera;

    //Visual FX
    public GameObject explosion;

    //Score
    public int score;

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
        gm = GM.gameController;
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
            ProjectileBehavior hit = other.gameObject.GetComponent<ProjectileBehavior>();
            hp -= hit.dmgValue;
            if (!hit.perist)
            {
                hit.DestroyProjectile();
            }
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
        Instantiate(explosion, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 12.0f);
        gm.score+=score;
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
    }

    private void DieByProjectile()
    {
        Destroy(gameObject);
        Instantiate(explosion, transform.position, transform.rotation);
        camera.GetComponent<CameraShaker>().CustomShake(collisionVal / 12.0f);  //Camera shake is proportional to collisionVal (heavier objects should shake camera more)
        if (isATurret)
        {
            GetComponentInParent<LargeEnemyBehavior>().turrets--;
        }
    }


}
