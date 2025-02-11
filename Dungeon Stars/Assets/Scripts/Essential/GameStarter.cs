﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    public delegate void GameStart();
    public static event GameStart OnGameStart;

    public float delay;

    private float startTime;
    private bool triggered;

    private GM gm;
    public GameObject textDisplay;

    private void Start()
    {
        gm = GM.GameController;

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
            OnGameStart?.Invoke();
            gameObject.SetActive(false);
        }
	}
}
