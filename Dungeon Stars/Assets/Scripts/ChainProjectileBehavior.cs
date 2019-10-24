using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainProjectileBehavior : BombBehavior
{

    //[Header("Chain Behavior")]
    //public float dpf;  // Damage per frame/update loop 

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
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
            ObstacleBehavior entity = other.gameObject.GetComponent<ObstacleBehavior>();
            //Chain lightning

            Vector3[] chain = { transform.position, entity.gameObject.transform.position };

            lineRenderer.SetPositions(chain);
            entity.hp -= dmgValue;
        }
    }
}
