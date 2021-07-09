﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fungus;

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

    int displayedEnemies = 0;
    int displayedPowerUps = 0;
    int displayedDeaths = 0;
    int displayedScore = 0;

    public AudioSource accumulateScoreFx;
    public AudioSource accumulateDeathsFx;

    public Flowchart rankFlowchart;
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
        
    }

    private void Update()
    {
        if (OmniController.omniController != null)
        {
            enemies.text = "Enemies Defeated: " + displayedEnemies + bestEnemiesStr;
            powerups.text = "PowerUps Collected: " + displayedPowerUps + bestPowerUpsStr;
            deaths.text = "Times Died: " + displayedDeaths + bestDeathsStr;
            scoreMod.text = "Score Modifier: x" + scoreModifier;
            score.text = "Overall Score: " + displayedScore + highScoreStr;
        }
    }

    private void ApplyModifiers()
    {
        if (OmniController.omniController != null && OmniController.omniController.selectedShip != null)
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
    }

    // Calculate the rank from 0-5 (F, D, C, B, A, S Tier)
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
        rankFlowchart.SetIntegerVariable("Rank", rank);
    }

    // Fungus helper method to 
    public void StartAccumulatingScore(int scoreType)
    {
        rankFlowchart.SetBooleanVariable("Accumulating", true);
        StartCoroutine("AccumulateScoreTxt", scoreType);
    }

    IEnumerator AccumulateScoreTxt(int scoreType)
    {
        switch (scoreType)
        {
            case 0:
                while (displayedEnemies < OmniController.omniController.enemiesKilled)
                {
                    if (OmniController.omniController.enemiesKilled - displayedEnemies > 100) displayedEnemies += 2;
                    else displayedEnemies++;
                    accumulateScoreFx.Play();
                    yield return new WaitForSecondsRealtime(.01f);
                }
                break;
            case 1:
                while (displayedPowerUps < OmniController.omniController.powerUpsCollected)
                {
                    displayedPowerUps++;
                    accumulateScoreFx.Play();
                    yield return new WaitForSecondsRealtime(.04f);
                }
                break;
            case 2:
                while (displayedDeaths < OmniController.omniController.timesDied)
                {
                    displayedDeaths++;
                    accumulateDeathsFx.Play();
                    yield return new WaitForSecondsRealtime(.5f);
                }
                break;
            case 3:
                while (displayedScore < trueScore)
                {
                    //if (trueScore - displayedScore > 100000) displayedScore += 100000;
                    if (trueScore - displayedScore > 10000) displayedScore += 10000;
                    else if (trueScore - displayedScore > 1000) displayedScore += 1000;
                    else if (trueScore - displayedScore > 100) displayedScore += 100;
                    else displayedScore++;
                    accumulateScoreFx.Play();
                    yield return new WaitForSecondsRealtime(.02f);
                }
                break;
            default:
                break;
        }
        rankFlowchart.SetBooleanVariable("Accumulating", false);
        
    }
}
