using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvergenceBolt : ProjectileBehavior
{
    public float initForce;
    public float convergeStrength;
    float convergeForce = 0f;

    Rigidbody2D rb;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.up * initForce, ForceMode2D.Impulse);
        GameObject t = FindClosestByTag("Obstacle");
        if (t)
            target = t.transform;
    }

    private void FixedUpdate()
    {
        Vector3 targetDir = transform.up;
        if (target)
        {
            targetDir = target.position - transform.position;
        }
        else
        {
            GameObject t = FindClosestByTag("Obstacle");
            if (t)
                target = t.transform;
        }
        rb.AddForce(targetDir.normalized * convergeForce);
        convergeForce += convergeStrength;
    }

    GameObject FindClosestByTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (tag == "Player")
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            else
            {
                if (go.GetComponent<ObstacleBehavior>().awake)
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < distance)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }
}
