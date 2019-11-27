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

    [Header("Options")]
    public GameObject[] allShips;
    public bool enableAllShips;
    public bool enableDebug;
    public bool infiniteLives;

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
    }
    
    public void ResetGameplayVariables()
    {
        totalScore = 0;
        enemiesKilled = 0;
        timesDied = 0;
        powerUpsCollected = 0;
    }

    
}
