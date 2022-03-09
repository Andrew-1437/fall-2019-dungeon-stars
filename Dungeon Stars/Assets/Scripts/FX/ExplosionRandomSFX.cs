using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionRandomSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] explosions = GetComponents<AudioSource>();

        int i = Random.Range(0, explosions.Length);
        explosions[i].enabled = true;
        explosions[i].pitch += Random.Range(-.2f, .2f);
    }
}
