using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resDropdown;
    public TextMeshProUGUI ships;
    public TextMeshProUGUI debug;
    public TextMeshProUGUI lives;
    public TextMeshProUGUI camShake;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeDisplay;

    Resolution[] resolutions;

    string shipText = "Unlock all Ships:  ";
    string debugText = "Enable Debug Tools:  ";
    string livesText = "Enable Infinite Lives:  ";
    string camShakeText = "Enable Camera Shake:  ";

    private void Start()
    {
        volumeSlider.value = AudioListener.volume;

        // Thanks Brackeys! https://www.youtube.com/watch?v=YOaYQrN1oYQ&ab_channel=Brackeys
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        // Converts array of Resolutions to List of strings for the dropdown menu
        List<string> resToStringList = new List<string>();

        int currResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resToStringList.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].Equals(Screen.currentResolution)) currResIndex = i;
        }

        resDropdown.AddOptions(resToStringList);
        resDropdown.value = currResIndex;
        resDropdown.RefreshShownValue();
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

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width,
            resolutions[resolutionIndex].height,
            Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OnVolumeChange(float newVolume)
    {
        AudioListener.volume = newVolume;
    }
}
