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

    private void Update()
    {
        if (OmniController.omniController != null)
        {
            enemies.text = "Enemies Defeated: " + OmniController.omniController.enemiesKilled;
            powerups.text = "PowerUps Collected: " + OmniController.omniController.powerUpsCollected;
            deaths.text = "Times Died: " + OmniController.omniController.timesDied;
            score.text = "Overall Score: " + OmniController.omniController.totalScore;
        }
    }
}
