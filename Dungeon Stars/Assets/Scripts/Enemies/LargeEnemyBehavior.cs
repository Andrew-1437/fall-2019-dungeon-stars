using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LargeEnemyBehavior : MonoBehaviour {

    #region References
    protected GM gm;
    protected GameObject gameCamera;
    #endregion

    public short turrets;

    #region Visual FX
    public GameObject explosion;
    public GameObject subExplosion;
    public float subExplodeRadius = 5f;
    #endregion

    #region Score
    public int score;
    public GameObject floatingScoreText;
    #endregion

    #region Dying Behavior
    protected bool dying = false;
    protected float dieTime = Mathf.Infinity;
    protected float subexplodeTime = Mathf.Infinity;
    #endregion

    protected void Start()
    {
        gameCamera = Camera.main.gameObject;
        if (gameCamera == null)
        {
            print("Ohshit! Obstacle cannot find camera!");
        }
        gm = GM.GameController;
    }

    protected void Update()
    {
        if (turrets <= 0 && !dying)
        {
            BeginDeathSequence();
        }
        if(dying)
        {
            // Randomly explode when dieing
            // TODO: This can be made into a coroutine
            float x = Random.Range(0f, .5f);
            if (Time.time > subexplodeTime)
            {
                Destroy(
                    Instantiate(subExplosion, 
                    transform.position + Random.insideUnitSphere * subExplodeRadius, 
                    transform.rotation), 3f);
                subexplodeTime = Time.time + Random.Range(.05f, .35f);
            }
            if (Time.time > dieTime) { Die(); }
        }
        
    }

    public virtual void Die()
    {
        OmniController.omniController.enemiesKilled++;
        Destroy(gameObject);
        Destroy(
            Instantiate(explosion, transform.position, transform.rotation), 5f);
        DisplayScore();
        if (gameCamera)
        {
            gameCamera.GetComponent<CameraShaker>().LargeShake(0.2f);
        }
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

    public void BeginDeathSequence()
    {
        if (!dying)
        {
            dying = true;
            dieTime = Time.time + 1.3f;
            subexplodeTime = Time.time;
        }
    }
}
