using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OmniController : MonoBehaviour
{
    // Singleton instance
    public static OmniController omniController;

    #region Score Tracking
    [Header("Game Summary")]
    public int totalScore;
    public int enemiesKilled;
    public int timesDied;
    public int powerUpsCollected;
    public bool completedGame;
    #endregion

    #region Ship Selection
    public string loadIntoLevel = "";
    [HideInInspector]
    public GameObject selectedShip = null;
    [HideInInspector]
    public GameObject selectedShip2 = null;  // Two players
    #endregion

    #region Options
    [Header("Options")]
    public GameObject[] allShips;
    public bool enableAllShips;
    public bool enableDebug;
    public bool infiniteLives;
    public bool enableCameraShake;
    #endregion

    #region Global Modifiers
    [Header("Modifiers")]
    public float globalTimeScale = 1f;  // Time for Unity's Time.timeScale
    public int deathPenalty = 50000;    // Score penalty upon dying
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
    #endregion

    #region Additional Game Modes
    [Header("Two Player Mode")]
    public bool twoPlayerMode;

    [Header("Endless Mode")]
    public bool endlessMode;
    public int finalDifficultyLevel;
    public float timeTaken;
    #endregion

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
        // "Speedrun mode"
        if (enableDebug)
        {
            if (Input.GetKeyDown(KeyCode.I)) ChangeTimeScale(1f);
            if (Input.GetKeyDown(KeyCode.O)) ChangeTimeScale(2f);
            if (Input.GetKeyDown(KeyCode.U)) ChangeTimeScale(.5f);
        }

        //Time.timeScale = globalTimeScale;
    }

    // Resets all variables used to track player's gameplay
    public void ResetGameplayVariables()
    {
        totalScore = 0;
        enemiesKilled = 0;
        timesDied = 0;
        powerUpsCollected = 0;
        selectedShip = null;
        selectedShip2 = null;
        completedGame = false;
    }

    // Reset all global modifiers back to the default value of 1
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
        ChangeTimeScale(1f);
    }

    public void ChangeTimeScale(float newTimeScale)
    {
        globalTimeScale = newTimeScale;
        Time.timeScale = globalTimeScale;
    }

    public void SetTwoPlayers(bool setCoop)
    {
        twoPlayerMode = setCoop;
    }
    
}
