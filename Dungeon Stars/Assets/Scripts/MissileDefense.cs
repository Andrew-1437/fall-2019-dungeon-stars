using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDefense : MonoBehaviour
{
    public float fireRate;  // Time between zaps
    float nextFire;

    LineRenderer lr;
    AudioSource shootSound;

    // Start is called before the first frame update
    void Start()
    {
        nextFire = Time.time;
        lr = GetComponent<LineRenderer>();
        shootSound = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Zap any enemy missile within range if we can fire
        if(collision.CompareTag("EnemyMissile") && Time.time >= nextFire)
        {
            // Destroy the missile and set the next time we can zap again
            collision.GetComponent<ProjectileBehavior>().DestroyProjectile();
            shootSound.Play();
            nextFire = Time.time + fireRate;

            // Get some points for killing a missile
            GM.gameController.AddScore(10);

            // Display a line renderer to see the missile get zapped
            StartCoroutine(FireAntiMissile(collision.transform.position));
        }
    }

    IEnumerator FireAntiMissile(Vector3 target)
    {
        // Brief period of time showing the line renderer
        lr.enabled = true;
        lr.SetPositions(new Vector3[] { transform.position, target });

        yield return new WaitForSeconds(.1f);

        // Disable the line renderer to make it look like a laser
        lr.enabled = false;
    }
}
