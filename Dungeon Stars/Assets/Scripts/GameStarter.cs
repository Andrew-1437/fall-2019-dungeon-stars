using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    public float delay;

    private float startTime;
    private bool triggered;

    private GameObject gm;
    //public GameObject player;
    //public GameObject fx;
    public GameObject textDisplay;

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by BackgroundMove!");
        }

        triggered = false;
    }

    // Update i should only be called once
    void Update ()
    {
        if(!triggered)
        {
            if(textDisplay != null)
            {
                textDisplay.GetComponent<FlashUI>().awake = true;
            }
            triggered = true;
            startTime = Time.time + delay;
        }
        if (Time.time >= startTime)
        {
            gm.GetComponent<GM>().gameStart = true;
            gm.GetComponent<GM>().SpawnPlayer();
            gm.GetComponent<GM>().FindPlayer();
            

            gameObject.SetActive(false);
        }
	}
}
