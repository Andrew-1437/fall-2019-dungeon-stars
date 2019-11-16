using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDelete : MonoBehaviour {

    //Cleans up stray projectiles
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.tag == "EnemyProjectile")
        {
            other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
        else if (other.tag == "Dps")
        {
            return;
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
