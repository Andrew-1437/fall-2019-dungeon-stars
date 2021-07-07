using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Summary : MonoBehaviour
{
    public TextMeshProUGUI enemies;
    public TextMeshProUGUI powerups;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI scoreMod;
    public TextMeshProUGUI score;

    string bestEnemiesStr;
    string bestPowerUpsStr;
    string bestDeathsStr;
    string highScoreStr;

    public int rank = 0;

    int trueScore = 0;  // True high score, after applying all modifiers
    float scoreModifier = 1f;

    private void Start()
    {
        int bestEnemies = PlayerPrefs.GetInt("MostEnemies", 0);
        int bestPowerUps = PlayerPrefs.GetInt("MostPowerUp", 0);
        int bestDeaths = PlayerPrefs.GetInt("MostDeaths", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        ApplyModifiers();
        trueScore = (int)((double)OmniController.omniController.totalScore * scoreModifier);

        if (OmniController.omniController.enemiesKilled > bestEnemies)
        {
            bestEnemiesStr = "   Best: " + OmniController.omniController.enemiesKilled.ToString() + " **New Best**";
            PlayerPrefs.SetInt("MostEnemies", OmniController.omniController.enemiesKilled);
        }
        else
            bestEnemiesStr = "   Best: " + bestEnemies.ToString();

        if (OmniController.omniController.powerUpsCollected > bestPowerUps)
        {
            bestPowerUpsStr = "   Best: " + OmniController.omniController.powerUpsCollected.ToString() + " **New Best**";
            PlayerPrefs.SetInt("MostPowerUp", OmniController.omniController.powerUpsCollected);
        }
        else
            bestPowerUpsStr = "   Best: " + bestPowerUps.ToString();

        if (OmniController.omniController.timesDied > bestDeaths)
        {
            bestDeathsStr = "   Worst: " + OmniController.omniController.timesDied.ToString() + " **New Worst**";
            PlayerPrefs.SetInt("MostDeaths", OmniController.omniController.timesDied);
        }
        else
            bestDeathsStr = "   Worst: " + bestDeaths.ToString();

        if (trueScore > highScore)
        {
            highScoreStr = "   Best: " + trueScore.ToString() + " **New Best**";
            PlayerPrefs.SetInt("HighScore", trueScore);
        }
        else
            highScoreStr = "   Best: " + highScore.ToString();

        GetRanking();
        // Show animation of rank appearing
        
    }

    private void Update()
    {
        if (OmniController.omniController != null)
        {
            enemies.text = "Enemies Defeated: " + OmniController.omniController.enemiesKilled + bestEnemiesStr;
            powerups.text = "PowerUps Collected: " + OmniController.omniController.powerUpsCollected + bestPowerUpsStr;
            deaths.text = "Times Died: " + OmniController.omniController.timesDied + bestDeathsStr;
            scoreMod.text = "Score Modifier: x" + scoreModifier;
            score.text = "Overall Score: " + trueScore + highScoreStr;
        }
    }

    private void ApplyModifiers()
    {
        // If infinite lives are enabled, apply x.5 mod to score
        if (OmniController.omniController.infiniteLives)
            scoreModifier *= .5f;

        // If either player has the meme ship, apply x.05 to score
        if (OmniController.omniController.selectedShip.GetComponent<PlayerController>().id == ShipsEnum.ShipID.MEME ||
            (OmniController.omniController.twoPlayerMode && 
            OmniController.omniController.selectedShip2.GetComponent<PlayerController>().id == ShipsEnum.ShipID.MEME))
            scoreModifier *= .05f;

        // If either player is using the quantum ship, apply x5 to score
        if (OmniController.omniController.selectedShip.GetComponent<PlayerController>().id == ShipsEnum.ShipID.QUANTUM ||
            (OmniController.omniController.twoPlayerMode && 
            OmniController.omniController.selectedShip2.GetComponent<PlayerController>().id == ShipsEnum.ShipID.QUANTUM))
            scoreModifier *= 5f;

        // If playing multiplayer, apply x1.1 to score
        if (OmniController.omniController.twoPlayerMode)
            scoreModifier *= 1.1f;
    }

    // Calculate the rank from 0-6 (F, D, C, B, A, S Tier)
    public void GetRanking()
    {
        /* ***********************
         * Criteria for S rank
         * Enemies Killed: >= 550
         * Power Ups Collected >= 45
         * Deaths <= 0
         * Final Score >= 1,500,000
         * 
         * ***********************
         * Criteria for A Rank
         * Enemies Killed: >= 500
         * Power Ups Collected >= 40
         * Deaths <= 1
         * Final Score >= 1,300,000
         * 
         * ***********************
         * Criteria for B rank
         * Enemies Killed: >= 450
         * Power Ups Collected >= 35
         * Deaths <= 3
         * Final Score >= 1,000,000
         * 
         * ***********************
         * Criteria for C rank
         * Enemies Killed: >= 370
         * Power Ups Collected >= 30
         * Deaths <= 4
         * Final Score >= 800,000
         * 
         * ***********************
         * Criteria for D rank
         * Enemies Killed: >= 300
         * Power Ups Collected >= 20
         * Deaths <= 5
         * Final Score >= 500,000
         */

        if(OmniController.omniController != null)
        {
            // D Rank
            if (OmniController.omniController.enemiesKilled >= 300 &&
                OmniController.omniController.powerUpsCollected >= 20 &&
                OmniController.omniController.timesDied <= 5 &&
                trueScore >= 500000)
                rank++;
            // C Rank
            if (OmniController.omniController.enemiesKilled >= 370 &&
                OmniController.omniController.powerUpsCollected >= 30 &&
                OmniController.omniController.timesDied <= 4 &&
                trueScore >= 800000)
                rank++;
            // B Rank
            if (OmniController.omniController.enemiesKilled >= 450 &&
                OmniController.omniController.powerUpsCollected >= 35 &&
                OmniController.omniController.timesDied <= 3 &&
                trueScore >= 1000000)
                rank++;
            // A Rank
            if (OmniController.omniController.enemiesKilled >= 500 &&
                OmniController.omniController.powerUpsCollected >= 40 &&
                OmniController.omniController.timesDied <= 1 &&
                trueScore >= 1300000)
                rank++;
            // S Rank
            if (OmniController.omniController.enemiesKilled >= 550 &&
                OmniController.omniController.powerUpsCollected >= 45 &&
                OmniController.omniController.timesDied <= 0 &&
                trueScore >= 1500000)
                rank++;
        }
    }
}
