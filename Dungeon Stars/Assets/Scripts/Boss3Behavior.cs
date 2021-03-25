using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Behavior : MonoBehaviour
{
    short stage;

    public BossBehavior boss;

    [Header("Stage 1")]
    public GameObject stage1;
    public BeamWeapon[] beams;
    private float shootTime = 10f;
    

    [Header("Stage 2")]
    public GameObject stage2;
    public GameObject multiCannon1;
    public GameObject multiCannon2;

    public Transform cannon1pos;
    public Transform cannon2pos;

    Rigidbody2D rb;

    ObstacleBehavior cannon1;
    ObstacleBehavior cannon2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stage = 1;

        cannon1 = multiCannon1.GetComponent<ObstacleBehavior>();
        cannon2 = multiCannon2.GetComponent<ObstacleBehavior>();
    }

    private void FixedUpdate()
    {
        //rb.velocity = Random.insideUnitCircle;
    }

    // Update is called once per frame
    void Update()
    {
        if (stage == 1)
        {
            if (boss.awake && !stage1.activeSelf)
            {
                stage1.SetActive(true);
                shootTime = Time.time + 5f;
            }
            if (stage1.activeSelf && Time.time >= shootTime)
            {
                shootTime = Time.time + 8.5f;
                RandomBeams(4);
            }
        }
        float cann1hp = 0f;
        float cann2hp = 0f;
        if (multiCannon1 != null) cann1hp = cannon1.hp;
        if (multiCannon2 != null) cann2hp = cannon2.hp;
        boss.hp = cann1hp + cann2hp;
    }

    public void RandomBeams(int numBeams)
    {
        for (int i = 0; i < numBeams - 1; i++)
        {
            beams[Random.Range(0, beams.Length)].FireCycleOnce();
        }
    }

    public void NextStage()
    {
        stage = 2;
        stage2.SetActive(true);
        cannon1.awake = true;
        cannon2.awake = true;
    }

    public void IntoPosition()
    {
        multiCannon1.GetComponent<Boss3Movement>().InitialMove(cannon1pos.position);
        multiCannon2.GetComponent<Boss3Movement>().InitialMove(cannon2pos.position);
    }
}
