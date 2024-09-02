using System.Collections;
using UnityEngine;

/// <summary>
/// Generalized class that handles shooting projectiles
/// </summary>
public class Shooter : MonoBehaviour
{
    public GameObject Projectile;
    public float BurstFireRate;
    public int ShotPerBurst;
    public Transform Hardpoint;
    public float Delay;

    /// <summary>
    /// Fires a single burst of the shooter
    /// </summary>
    public void Shoot()
    {
        StartCoroutine(FireBurst());
    }

    /// <summary>
    /// Creates a single projectile and removes it later
    /// </summary>
    private void ShootOnce()
    {
        Destroy(Instantiate(Projectile, Hardpoint.position, Hardpoint.rotation), 30f);
    }

    /// <summary>
    /// Coroutine that fires a single burst of the shooter over time
    /// </summary>
    private IEnumerator FireBurst()
    {
        if (Delay > 0) { yield return new WaitForSeconds(Delay); }

        int shots = 0;

        while (true)
        {
            ShootOnce();
            shots++;

            if (shots >= ShotPerBurst)
            {
                break;
            }

            yield return new WaitForSeconds(BurstFireRate);
        }
    }
}
