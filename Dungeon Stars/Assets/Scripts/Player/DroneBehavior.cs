using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehavior : MonoBehaviour
{
    public float orbitSpeed;
    public float orbitRadius;
    public float rotationSpeed;
    public float lifetime;

    [HideInInspector]
    public bool followPlayer2 = false;  // Should the drone lock on to Player 2 instead?

    float timeSpawned;

    public ParticleSystem particles;
    public AudioSource despawnSound;
    public ParticleSystem despawnParticles;

    Transform playerTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        timeSpawned = Time.time;

        // Set to follow player 1's poition
        if (!followPlayer2 && GM.gameController.player != null)
            playerTransform = GM.gameController.player.transform;
        // Set to follow player 2's position
        else if (followPlayer2 && GM.gameController.player2 != null)
            playerTransform = GM.gameController.player2.transform;
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            DespawnDrone(); // If player is dead (or nothing to follow), despawn
            return;
        }
        
        transform.position = new Vector3(
            playerTransform.position.x + Mathf.Cos((Time.time - timeSpawned) * orbitSpeed) * orbitRadius,
            playerTransform.position.y + Mathf.Sin((Time.time - timeSpawned) * orbitSpeed) * orbitRadius,
            0f);
        transform.Rotate(0f, 0f, rotationSpeed);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSpawned > lifetime)
            DespawnDrone();
    }

    public void DespawnDrone()
    {
        particles.Stop();
        despawnSound.Play();
        despawnParticles.Play();
        particles.gameObject.transform.SetParent(null);
        despawnParticles.gameObject.transform.SetParent(null);
        Destroy(gameObject);
        Destroy(particles.gameObject, 3f);
        Destroy(despawnParticles.gameObject, 3f);
    }
}
