using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resDropdown;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeDisplay;

    Resolution[] resolutions;

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
        volumeDisplay.text = ((int)(AudioListener.volume * 100)).ToString();
    }

    // Toggles if all extra ships are available
    public void ToggleShips(bool toggle)
    {
        OmniController.omniController.enableAllShips = toggle;
    }

    // Toggles use of debugging tools in game
    public void ToggleDebug(bool toggle)
    {
        OmniController.omniController.enableDebug = toggle;
    }

    // Toggle infinite lives
    public void ToggleLives(bool toggle)
    {
        OmniController.omniController.infiniteLives = toggle;
    }

    // Toggles Camera Shaking
    public void ToggleCameraShake(bool toggle)
    {
        OmniController.omniController.enableCameraShake = toggle;
    }

    // Resets all unlocked ships
    public void ResetUnlockedShips()
    {
        OmniController.omniController.ResetUnlockedShips();
    }

    // Resets level progress in the main story
    public void ResetLevelProgress()
    {
        PlayerPrefs.SetInt("highestLevelCompleted", 0);
    }

    // Sets resolution of the screen to the resolution at index
    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width,
            resolutions[resolutionIndex].height,
            Screen.fullScreen);
    }

    // Toggles if game is in fullscreen
    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OnVolumeChange(float newVolume)
    {
        AudioListener.volume = newVolume;
    }
}
