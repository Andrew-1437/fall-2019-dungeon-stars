using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public string sceneToLoad; 


    public void PlayGame()
    {
        //SceneManager.LoadScene(sceneToLoad);
        sceneLoader.LoadScene(sceneToLoad);
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(false);
        gameObject.SetActive(false);
    }

    public void Play2Player()
    {
        sceneLoader.LoadScene(sceneToLoad);
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(true);
        gameObject.SetActive(false);
    }

    public void Tutorial()
    {
        //SceneManager.LoadScene("Tutorial");
        sceneLoader.LoadScene("Tutorial");
        OmniController.omniController.ResetGameplayVariables();
        OmniController.omniController.SetTwoPlayers(false);
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        print("*Windows shut down sound effect*");
        Application.Quit();
    }

}
