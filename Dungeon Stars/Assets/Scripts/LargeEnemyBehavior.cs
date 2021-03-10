using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LargeEnemyBehavior : MonoBehaviour {
    //GM
    protected GM gm;

    public short turrets;
    public GameObject explosion;
    public GameObject subExplosion;
    public float subExplodeRadius = 5f;

    //Camera Shake
    protected GameObject gameCamera;

    //Score
    public int score;
    public GameObject floatingScoreText;

    bool dieing = false;
    float dietime = Mathf.Infinity;
    float subexplodeTime = Mathf.Infinity;

    protected void Start()
    {
        gameCamera = Camera.main.gameObject;
        if (gameCamera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        //GameObject gmobject = GameObject.FindWithTag("GameController");
        //gm = gmobject.GetComponent<GM>(); 
        gm = GM.gameController;
    }

    protected void Update()
    {
        if (turrets <= 0 && !dieing)
        {
            dieing = true;
            dietime = Time.time + 1f;
            subexplodeTime = Time.time;
        }
        if(dieing)
        {
            // Randomly explode when dieing
            float x = Random.Range(0f, 1f);
            if (Time.time > subexplodeTime)
            {
                Destroy(
                    Instantiate(subExplosion, transform.position + Random.insideUnitSphere * subExplodeRadius, transform.rotation), 3f);
                subexplodeTime = Time.time + Random.Range(.05f, .35f);
            }
            if (Time.time > dietime)
                Die();
        }
        
    }

    private void Die()
    {
        OmniController.omniController.enemiesKilled++;
        Destroy(gameObject);
        Destroy(
            Instantiate(explosion, transform.position, transform.rotation), 5f);
        DisplayScore();
        if(gameCamera)
            gameCamera.GetComponent<CameraShaker>().LargeShake(0.2f);
        gm.AddScore(score);
    }

    private void DisplayScore()
    {
        if (score != 0 && floatingScoreText)
        {
            GameObject scoreText = Instantiate(floatingScoreText,
                transform.position,
                Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f))
                ) as GameObject;
            scoreText.GetComponent<TextMeshPro>().text = (score * gm.scoreMultiplier).ToString();
            scoreText.GetComponent<Rigidbody2D>().AddForce(Random.onUnitSphere, ForceMode2D.Impulse);
            Destroy(scoreText, 1);
        }
    }
}
