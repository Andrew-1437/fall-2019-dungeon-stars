using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Boss1Behavior : MonoBehaviour {

    [Header("Begining Movement Variables")]
	public float moveYToHere;
    public float rotateTo;
    public float moveSpeed;
    public float rotateSpeed;

    [Header("References")]
    public GameObject bossFlowchart;
    public GameObject weakSpot;

    BossBehavior boss;

    bool stage2 = false;

    private void Start()
    {
        boss = GetComponent<BossBehavior>();
    }

    // Wow who wrote this. Oh wait it was me
    private void Update()
    {
        // Begining Movement (Old code)
        if (GetComponent<BossBehavior>().awake)
        {
            if (transform.position.y < moveYToHere)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.up * moveSpeed;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                bossFlowchart.SetActive(true);
                //GetComponent<Collider2D>().enabled = true;
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

        // If all four turrets are destroyed, go to stage 2 (reveal fifth weak point and change moveset)
        if (boss.turrets <= 1 && !stage2)
        {
            stage2 = true;
            bossFlowchart.GetComponent<Fungus.Flowchart>().SendFungusMessage("stage2");
        }
    }

    public void RevealWeakSpot(bool active)
    {
        weakSpot.GetComponent<Collider2D>().enabled = active;
        weakSpot.GetComponent<Light2D>().enabled = active;
    }

}
