using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserCrashFX : MonoBehaviour
{
    public GameObject particleContainer;
    public ParticleSystem initialExplosion;
    public ParticleSystem smokeFX;
    public ParticleSystem finalExplosion;

    public void InitialFX()
    {
        initialExplosion.Play();
        smokeFX.Play();
    }

    public void FinalFX()
    {
        finalExplosion.Play();
        smokeFX.Stop();
        particleContainer.transform.parent = null;
        Destroy(gameObject);
        Destroy(particleContainer, 5f);
    }
}
