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
    public GameObject selectedShip = null;
    [HideInInspector]
    public GameObject selectedShip2 = null;  // Two players

    [Header("Options")]
    public GameObject[] allShips;
    public bool enableAllShips;
    public bool enableDebug;
    public bool infiniteLives;
    public bool enableCameraShake;

    [Header("Modifiers")]
    public float globalTimeScale = 1;

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

    public void SetTwoPlayers(bool setCoop)
    {
        twoPlayerMode = setCoop;
    }
    
}
