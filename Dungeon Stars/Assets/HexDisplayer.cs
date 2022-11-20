using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexDisplayer : MonoBehaviour
{
    public GameObject[] hexMarkers;
    public ObstacleBehavior obstacle;

    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (obstacle == null) { return; }

        DisplayHexMarkers(obstacle.hex.Stacks);
        if (obstacle.hex.Stacks == 6 && particles.isStopped) 
        { 
            particles.Play(); 
        }
        else if (obstacle.hex.Stacks < 6 && particles.isPlaying) 
        { 
            particles.Stop(); 
        }
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
