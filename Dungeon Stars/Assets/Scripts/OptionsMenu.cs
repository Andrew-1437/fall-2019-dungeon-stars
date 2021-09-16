using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TextMeshProUGUI ships;
    public TextMeshProUGUI debug;
    public TextMeshProUGUI lives;
    public TextMeshProUGUI camShake;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeDisplay;

    string shipText = "Unlock all Ships:  ";
    string debugText = "Enable Debug Tools:  ";
    string livesText = "Enable Infinite Lives:  ";
    string camShakeText = "Enable Camera Shake:  ";

    private void Start()
    {
        volumeSlider.value = AudioListener.volume;
    }

    // Update is called once per frame
    void Update()
    {
        ships.text = shipText + OmniController.omniController.enableAllShips.ToString();
        debug.text = debugText + OmniController.omniController.enableDebug.ToString();
        lives.text = livesText + OmniController.omniController.infiniteLives.ToString();
        camShake.text = camShakeText + OmniController.omniController.enableCameraShake.ToString();
        volumeDisplay.text = ((int)(AudioListener.volume * 100)).ToString();
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

    public void ToggleCameraShake()
    {
        OmniController.omniController.enableCameraShake = !OmniController.omniController.enableCameraShake;
    }

    public void OnVolumeChange(float newVolume)
    {
        AudioListener.volume = newVolume;
    }
}
