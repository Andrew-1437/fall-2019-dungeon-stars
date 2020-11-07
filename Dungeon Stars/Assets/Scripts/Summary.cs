using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Summary : MonoBehaviour
{
    public TextMeshProUGUI enemies;
    public TextMeshProUGUI powerups;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI score;

    string bestEnemiesStr;
    string bestPowerUpsStr;
    string bestDeathsStr;
    string highScoreStr;

    private void Start()
    {
        int bestEnemies = PlayerPrefs.GetInt("MostEnemies", 0);
        int bestPowerUps = PlayerPrefs.GetInt("MostPowerUp", 0);
        int bestDeaths = PlayerPrefs.GetInt("MostDeaths", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

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

        if (OmniController.omniController.totalScore > highScore)
        {
            highScoreStr = "   Best: " + OmniController.omniController.totalScore.ToString() + " **New Best**";
            PlayerPrefs.SetInt("HighScore", OmniController.omniController.totalScore);
        }
        else
            highScoreStr = "   Best: " + highScore.ToString();

    }

    private void Update()
    {
        if (OmniController.omniController != null)
        {
            enemies.text = "Enemies Defeated: " + OmniController.omniController.enemiesKilled + bestEnemiesStr;
            powerups.text = "PowerUps Collected: " + OmniController.omniController.powerUpsCollected + bestPowerUpsStr;
            deaths.text = "Times Died: " + OmniController.omniController.timesDied + bestDeathsStr;
            score.text = "Overall Score: " + OmniController.omniController.totalScore + highScoreStr;
        }
    }
}
