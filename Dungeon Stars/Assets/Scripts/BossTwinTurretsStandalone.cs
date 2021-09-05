using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTwinTurretsStandalone : MonoBehaviour
{
    public GameObject multiCannon1;
    public GameObject multiCannon2;

    public Transform pos1;
    public Transform pos2;

    Boss3Movement cannonMovement1;
    Boss3Movement cannonMovement2;

    ObstacleBehavior cannon1;
    ObstacleBehavior cannon2;

    BossBehavior boss;

    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponent<BossBehavior>();

        cannonMovement1 = multiCannon1.GetComponent<Boss3Movement>();
        cannon1 = multiCannon1.GetComponent<ObstacleBehavior>();
        cannonMovement2 = multiCannon2.GetComponent<Boss3Movement>();
        cannon2 = multiCannon2.GetComponent<ObstacleBehavior>();

        boss.hp = cannon1.hp + cannon2.hp;

        cannonMovement1.InitialMove(pos1.position);
        cannonMovement2.InitialMove(pos2.position);
    }

    // Update is called once per frame
    void Update()
    {
        float cann1hp = 0f;
        float cann2hp = 0f;
        if (multiCannon1 != null) cann1hp = cannon1.hp;
        if (multiCannon2 != null) cann2hp = cannon2.hp;
        boss.hp = cann1hp + cann2hp;
        GM.gameController.UpdateBossHpBar(boss.hp);
        if (boss.hp <= 0) boss.BeginDeathSequence();
    }
}
