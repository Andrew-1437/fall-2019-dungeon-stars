using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterFlyIn : MonoBehaviour
{
    public ParticleSystem shootingVFX;
    public AudioSource shootingSFX;

    public void BeginShooting()
    {
        shootingVFX.Play();
        shootingSFX.Play();
    }

    public void StopShooting()
    {
        shootingVFX.Stop();
        shootingSFX.Stop();
    }


}
