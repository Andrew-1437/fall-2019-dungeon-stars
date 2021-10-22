using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;

    public TextMeshProUGUI connectionStatus;

    // Scene names
    const string shipSelect = "ShipSelect";
    const string level1 = "Level1";
    const string level2 = "Level2";
    const string level3 = "Level3";
    const string level4 = "Level4";
    const string endless = "Endless Mode";
    const string tutorial = "Tutorial";

    private void Start()
    {
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.ResetModifiers();

        NetworkManager.OnConnectToMaster += NetworkManager_OnConnectToMaster;
        connectionStatus.text = "Not connected...";
    }

    private void NetworkManager_OnConnectToMaster()
    {
        connectionStatus.text = "Connected to Photon Services!";
        NetworkManager.OnConnectToMaster -= NetworkManager_OnConnectToMaster;
    }

    public void PlayGame(bool twoPlayer)
    {
        NetworkManager.OnConnectToMaster -= NetworkManager_OnConnectToMaster;
        sceneLoader.LoadScene(shipSelect);
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(twoPlayer);
        OmniController.omniController.loadIntoLevel = "LevelSelect";
        OmniController.omniController.endlessMode = false;
        gameObject.SetActive(false);
    }

    public void PlayEndless(bool twoPlayer)
    {
        NetworkManager.OnConnectToMaster -= NetworkManager_OnConnectToMaster;
        sceneLoader.LoadScene(shipSelect);
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(twoPlayer);
        OmniController.omniController.loadIntoLevel = endless;
        OmniController.omniController.endlessMode = true;
        gameObject.SetActive(false);
    }

    public void Tutorial()
    {
        NetworkManager.OnConnectToMaster -= NetworkManager_OnConnectToMaster;
        sceneLoader.LoadScene(tutorial);
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(false);
        OmniController.omniController.endlessMode = false;
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        NetworkManager.OnConnectToMaster -= NetworkManager_OnConnectToMaster;
        print("*Windows shut down sound effect*");
        Application.Quit();
    }

}
