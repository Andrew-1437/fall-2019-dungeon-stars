using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Behavior : MonoBehaviour
{
    public PeriodicBeam[] beams;
    private float shootTime = 10f;
    public GameObject root;
    public BossBehavior boss;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = Random.insideUnitCircle;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.awake && !root.activeSelf)
        {
            root.SetActive(true);
            shootTime = Time.time + 5f;
        }
        if (root.activeSelf && Time.time >= shootTime)
        {
            shootTime = Time.time + 10f;
            RandomBeams(5);
        }
        
    }

    public void RandomBeams(int numBeams)
    {
        for (int i = 0; i < numBeams; i++)
        {
            beams[Random.Range(0, beams.Length - 1)].StartNow();
        }
    }
}
