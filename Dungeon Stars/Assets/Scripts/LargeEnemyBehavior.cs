using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemyBehavior : MonoBehaviour {
    //GM
    private GM gm;

    public short turrets;
    public GameObject explosion;

    //Camera Shake
    protected GameObject gameCamera;

    //Score
    public int score;

    protected void Start()
    {
        gameCamera = GameObject.FindWithTag("MainCamera");
        if (gameCamera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        GameObject gmobject = GameObject.FindWithTag("GameController");
        gm = gmobject.GetComponent<GM>();
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
        gameCamera.GetComponent<CameraShaker>().LargeShake(0.2f);
        gm.score += score;
    }
}
