using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexDisplayer : MonoBehaviour
{
    public GameObject[] hexMarkers;
    public ObstacleBehavior obstacle;
    public BossBehavior boss;
    public PlayerController player;

    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        int currentStacks = GetCurrentStacks();

        DisplayHexMarkers(currentStacks);
        if (currentStacks == 6 && particles.isStopped) 
        { 
            particles.Play(); 
        }
        else if (currentStacks < 6 && particles.isPlaying) 
        { 
            particles.Stop(); 
        }
    }

    private int GetCurrentStacks()
    {
        if (obstacle) { return obstacle.hex.Stacks; }
        if (boss) { return boss.hex.Stacks; }
        if (player) { return player.hex.Stacks; }
        return 0;
    }

    public void DisplayHexMarkers(int stacks)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i < stacks) hexMarkers[i].SetActive(true);
            else hexMarkers[i].SetActive(false);
        }
    }
}
