using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    public float delay;

    private float startTime;
    private bool triggered;

    private GM gm;
    //public GameObject player;
    //public GameObject fx;
    public GameObject textDisplay;

    private void Start()
    {
        gm = GM.gameController;
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
                textDisplay.GetComponent<FlashUI>().Flash();
            }
            triggered = true;
            startTime = Time.time + delay;
        }
        if (Time.time >= startTime)
        {
            gm.gameStart = true;
            gm.SpawnPlayer();
            if (gm.twoPlayerMode)
                gm.SpawnPlayer2();
            //gm.FindPlayer();
            

            gameObject.SetActive(false);
        }
	}
}
