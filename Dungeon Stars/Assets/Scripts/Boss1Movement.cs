using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Movement : MonoBehaviour {

	public float moveYToHere;
    public float rotateTo;
    public float moveSpeed;
    public float rotateSpeed;

    private void Update()
    {
        if (GetComponent<BossBehavior>().awake)
        {
            if (transform.position.y < moveYToHere)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.up * moveSpeed;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            }
            if (transform.position.y >= 0 && GetComponent<Transform>().rotation.eulerAngles.z < 180.0f)
            {
                GetComponent<Rigidbody2D>().angularVelocity = rotateSpeed;
            }
            else if (transform.position.y >= 0 && GetComponent<Transform>().rotation.eulerAngles.z >= 180.0f)
            {
                GetComponent<Rigidbody2D>().angularVelocity = 0;
                GetComponent<BossBehavior>().activeAllTriggers(true);
            }
        }
    }

}
