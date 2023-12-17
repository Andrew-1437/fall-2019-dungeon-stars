using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSix : MonoBehaviour
{
    private const float BackgroundSpeed = 1.1f;

    public BackgroundMove Background;
    public GameObject Hexplosion;


    public void StopBackground()
    {
        Background.speed = 0f;
    }

    public void StartBackground()
    {
        Background.speed = BackgroundSpeed;
    }

    public void HexPlayers()
    {
        Transform playerPos = GM.GameController.playerController.transform;
        Instantiate(Hexplosion, playerPos.position, playerPos.rotation);

        if (GM.GameController.twoPlayerMode)
        {
            playerPos = GM.GameController.playerController2.transform; 
            Instantiate(Hexplosion, playerPos.position, playerPos.rotation);
        }
    }

    public void HexTurret(ObstacleBehavior turret)
    {
        turret.SetAwake(true);
        turret.hex.ApplyStacks(6);
        Transform turretTransform = turret.transform;
        Instantiate(Hexplosion, turretTransform.position, turretTransform.rotation);
    }
}
