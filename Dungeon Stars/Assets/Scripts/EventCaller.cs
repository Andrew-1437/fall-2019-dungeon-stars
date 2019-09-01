using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCaller : MonoBehaviour {

    public string eventName;

    //GM
    private GameObject gm;

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by event caller!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Events
        if (other.tag == "Bounds")
        {
            print("Calling event: " + eventName);
            
            //gm.GetComponent<GM>().CallEvent(eventName);
        }
    }
}
