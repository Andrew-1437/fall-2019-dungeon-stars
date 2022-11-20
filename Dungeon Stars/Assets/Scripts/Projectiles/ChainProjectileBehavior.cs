using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainProjectileBehavior : BombBehavior
{

    [Header("Chain Behavior")]
    //public float dpf;  // Damage per frame/update loop 

    //private LineRenderer lineRenderer;
    public GameObject lightning;
    public Transform lightningEnd;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        lightning.SetActive(false);
        //lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            lightning.SetActive(true);
            ObstacleBehavior entity = other.gameObject.GetComponent<ObstacleBehavior>();
            //Chain lightning

            lightningEnd.position = entity.gameObject.transform.position;

            //Vector3[] chain = { transform.position, entity.gameObject.transform.position };


            //lineRenderer.SetPositions(chain);
            entity.Damage(dmgValue);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            lightning.SetActive(false);
        }
    }
}
