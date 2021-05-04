using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OmniController : MonoBehaviour
{
    public static OmniController omniController;

    [Header("Game Summary")]
    public int totalScore;
    public int enemiesKilled;
    public int timesDied;
    public int powerUpsCollected;

    [HideInInspector]
    public GameObject selectedShip;
    [HideInInspector]
    public GameObject selectedShip2;  // Two players

    [Header("Options")]
    public GameObject[] allShips;
    public bool enableAllShips;
    public bool enableDebug;
    public bool infiniteLives;
    public bool enableCameraShake;
    
    [Header("Modifiers")]
    public float globalTimeScale = 1f;  // Time for Unity's Time.timeScale
    public float playerHpScale = 1f;    // Multiply player's starting max hp by this amount
    public float playerShieldScale = 1f;    // Multiply player's starting max shield by this amount
    public float hpPerLevelScale = 1f;  // Multiply the increase of max hp by this amount each level
    public float shieldPerLevelScale = 1f;  // Same as above but for shields
    public float playerIncommingDamageScale = 1f;   // Multiply player incomming damage by this amount
    public float obstacleIncommingDamageScale = 1f; // Multiply obstacle incomming damage by this amount
    public float collisionDamageScale = 1f; // Multiply collision damage by this amount (stacks with incomming damage scale)
    public float projectileDamageScale = 1f;    // Multiply damage by all projectiles by this amount
    public float projectileSpeedScale = 1f;     // Multiply all projectiles' speed by this amount
    public float playerSpeedScale = 1f; // Multiply player's move speed by this amount
    public float obstacleSpeedScale = 1f;   // Multiply obstacle/enemy move speed by this amount
    public float playerFireRateScale = 1f;  // Multiply player's fire rate (primary and secondary) by this amount
    public float enemyFireRateScale = 1f;   // Multiply enemy fire rate by this amount
    public float obstacleHpScale = 1f;  // Multiply obstacle/enemy max hp by this amount
    public float additionalScoreMultiplier = 1f;    // Multiply all point gains by this amount
    public float powerUpDurationScale = 1f;  // Multiply the duration of all power ups by this amount


    [Header("Two Player Co-op")]
    public bool twoPlayerMode;

    private void Awake()
    {
        
        if(omniController != null && omniController != this)
        {
            Destroy(gameObject);
        }
        else
        {
            omniController = this;
        }
        DontDestroyOnLoad(gameObject);
        globalTimeScale = 1;
    }

    private void Update()
    {
        if (enableDebug)
        {
            if (Input.GetKeyDown(KeyCode.I)) globalTimeScale = 1f;
            if (Input.GetKeyDown(KeyCode.O)) globalTimeScale = 2f;
            if (Input.GetKeyDown(KeyCode.U)) globalTimeScale = .5f;
        }

        Time.timeScale = globalTimeScale;
    }

    public void ResetGameplayVariables()
    {
        totalScore = 0;
        enemiesKilled = 0;
        timesDied = 0;
        powerUpsCollected = 0;
        selectedShip = null;
        selectedShip2 = null;
        globalTimeScale = 1f;
    }

    public void ResetModifiers()
    {
        playerHpScale = 1f;
        playerShieldScale = 1f; 
        hpPerLevelScale = 1f; 
        shieldPerLevelScale = 1f; 
        playerIncommingDamageScale = 1f;  
        obstacleIncommingDamageScale = 1f; 
        projectileDamageScale = 1f;  
        playerSpeedScale = 1f; 
        obstacleSpeedScale = 1f;  
        playerFireRateScale = 1f;  
        enemyFireRateScale = 1f;   
        obstacleHpScale = 1f;  
        additionalScoreMultiplier = 1f;
        powerUpDurationScale = 1f;
    }

    public void SetTwoPlayers(bool setCoop)
    {
        twoPlayerMode = setCoop;
    }
    
}
