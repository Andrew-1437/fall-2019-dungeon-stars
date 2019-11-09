using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockBeam : ProjectileBehavior
{
    [Header("Shock Beam")]
    public float arcRange;
    public float shieldRecharge;
    PlayerController player;
    //public Transform lightningEnd;
    public Transform endpoint;  // Default endpoint if no targets in range
    public GameObject root;
    GameObject target;

    private void Awake()
    {
        player = (PlayerController)FindObjectOfType(typeof(PlayerController));
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        //arcRange = Vector3.Distance(transform.position, endpoint.position);
        //print(arcRange);

        target = FindClosestByTag("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (!target)
        {
            //print("No target");
            transform.position = endpoint.position + Random.insideUnitSphere;
        }
        else
        {
            //print("Target");
            transform.position = target.transform.position + Random.insideUnitSphere;
            player.shield += shieldRecharge;
            player.shieldDown = false;
        }
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
                if (curDistance < distance && curDistance < arcRange)
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
                    if (curDistance < distance && curDistance < arcRange)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }
        return closest;
    }

    public override void DestroyProjectile()
    {
        base.DestroyProjectile();

        Destroy(root);
    }
}
