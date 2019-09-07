using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour {

    public float speed;

    private GameObject gm;

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController");
        if (gm == null)
        {
            print("Ohshit! Game Controller not found by BackgroundMove!");
        }
    }

    private void Update()
    {
        if (gm.GetComponent<GM>().gameStart)
        {
            transform.position = transform.position + Vector3.up * -speed;
        }
    }

}
