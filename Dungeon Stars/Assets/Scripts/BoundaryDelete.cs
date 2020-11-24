using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDelete : MonoBehaviour {

    //Cleans up stray projectiles & enemies that wander out of bounds
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Projectile" || other.tag == "EnemyProjectile")
        {
            other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
        else if (other.tag == "Dps" || other.tag == "Boss")
        {
            return;
        }
        else if (other.tag == "Obstacle" && other.gameObject.GetComponent<ObstacleBehavior>().dontDieOnScreenExit)
        {
            other.gameObject.GetComponent<ObstacleBehavior>().SleepOnScreenExit();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
