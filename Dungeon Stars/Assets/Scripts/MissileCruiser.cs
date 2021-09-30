using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCruiser : MonoBehaviour
{
    public TargetedMissileSpawner missileSpawner;
    public ParticleSystem missileFX;
    public ParticleSystem missileGroupFX;
    public AudioSource missileSFX;

    
    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        coroutine = MissileBurst();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            StartCoroutine(coroutine);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            StopCoroutine(coroutine);
            Destroy(gameObject, 8f);
        }
    }

    IEnumerator MissileBurst()
    {
        int cycle = 0;

        for (; ; )
        {
            if(cycle % 6 == 0) yield return new WaitForSeconds(2f);

            if (cycle % 6 < 3)
            {
                float startTime = Time.time;
                while (Time.time - startTime < missileSpawner.burstTime)
                {
                    missileFX.Play();
                    missileSFX.Play();
                    yield return new WaitForSeconds(missileSpawner.fireRate);
                }

                yield return new WaitForSeconds(1f);

                missileSpawner.FireBurst();

                yield return new WaitForSeconds(missileSpawner.burstOffTime);
            }
            else
            {
                missileGroupFX.Play();
                missileSFX.Play();

                yield return new WaitForSeconds(1f);

                missileSpawner.FireSimultanious(6);

                yield return new WaitForSeconds(.9f);
            }
            cycle++;
        }
    }
}
