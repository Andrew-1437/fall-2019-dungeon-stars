using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMenuCamera : MonoBehaviour {

    public float speed;
    public float xLim;
    public float yLim;
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontal, vertical) * speed;
        Vector2 pos = gameObject.GetComponent<Rigidbody2D>().position;
        pos.x = Mathf.Clamp(GetComponent<Rigidbody2D>().position.x, 0, xLim);
        pos.y = Mathf.Clamp(GetComponent<Rigidbody2D>().position.y, yLim, 0);
        gameObject.GetComponent<Rigidbody2D>().position = pos;
    }
}
