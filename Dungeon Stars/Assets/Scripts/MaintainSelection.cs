using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MaintainSelection : MonoBehaviour
{
    public GameObject selectedShip;
    public GameObject selectedShip2;  // Two players

    private void Update()
    {
        // If we return to main menu, delete this
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }
}
