using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TextMeshProUGUI ships;
    public TextMeshProUGUI debug;
    public TextMeshProUGUI lives;

    string shipText = "Unlock all Ships:  ";
    string debugText = "Enable Debug Tools:  ";
    string livesText = "Enable Infinite Lives:  ";

    // Update is called once per frame
    void Update()
    {
        ships.text = shipText + OmniController.omniController.enableAllShips.ToString();
        debug.text = debugText + OmniController.omniController.enableDebug.ToString();
        lives.text = livesText + OmniController.omniController.infiniteLives.ToString();
    }

    public void ToggleShips()
    {
        OmniController.omniController.enableAllShips = !OmniController.omniController.enableAllShips;
    }

    public void ToggleDebug()
    {
        OmniController.omniController.enableDebug = !OmniController.omniController.enableDebug;
    }

    public void ToggleLives()
    {
        OmniController.omniController.infiniteLives = !OmniController.omniController.infiniteLives;
    }
}
