using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemyBehavior : MonoBehaviour {

    public short turrets;
    public GameObject explosion;

    //Camera Shake
    private GameObject camera;

    private void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");
        if (camera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
    }

    private void Update()
    {
        if (turrets <= 0)
        {
            Die();
        }
        
    }

    private void Die()
    {
        Destroy(gameObject);
        Instantiate(explosion, gameObject.GetComponent<Transform>().position, gameObject.GetComponent<Transform>().rotation);
        camera.GetComponent<CameraShaker>().LargeShake(0.2f);
    }
}
