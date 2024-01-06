using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchRandomizer : MonoBehaviour
{
    public float pitchRange;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (AudioSource audio in audioSources) 
        { 
            audio.pitch += Random.Range(-pitchRange, pitchRange); 
        }
    }
}
