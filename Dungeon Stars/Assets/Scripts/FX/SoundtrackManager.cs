using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    public AudioSource levelSoundtrack;
    public AudioSource bossSoundtrack;
    AudioSource currentlyPlaying;

    // Start is called before the first frame update
    void Start()
    {
        currentlyPlaying = levelSoundtrack;
    }

    private void Update()
    {
        currentlyPlaying.pitch = OmniController.omniController.globalTimeScale;
    }

    // Helper methods for use by a Fungus Flowchart
    public void PlayLevelMusic()
    {
        levelSoundtrack.Play();
        currentlyPlaying = levelSoundtrack;
    }

    // "Why do I hear boss music?"
    public void PlayBossMusic()
    {
        bossSoundtrack.Play();
        currentlyPlaying = bossSoundtrack;
    }

    // Helper method that should be called from a Fungus Flowchart
    public void StartFadeOut(float fadeTime)
    {
        StartCoroutine(FadeOut(currentlyPlaying, fadeTime));
    }

    // Taken from https://forum.unity.com/threads/fade-out-audio-source.335031/
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
