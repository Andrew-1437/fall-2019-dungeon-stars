using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDelete : MonoBehaviour {

    //Cleans up stray projectiles & enemies that wander out of bounds
    void OnTriggerExit2D(Collider2D other)
    {
        // Delete projectiles the proper way
        if (other.tag == "Projectile" || other.tag == "EnemyProjectile")
        {
            other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile();
        }
        // Bosses should be able to move out of the screen freely
        else if (other.tag == "Dps" || other.tag == "Boss")
        {
            return;
        }
        // Some obstacles should not be deleted when they exit the screen
        else if (other.tag == "Obstacle" && other.gameObject.GetComponent<ObstacleBehavior>().dontDieOnScreenExit)
        {
            other.gameObject.GetComponent<ObstacleBehavior>().SleepOnScreenExit();
        }
        // Missiles should be deleted by the SuperBounds so that they get a chance to turn around off screen
        else if(other.tag == "Missile" || other.tag == "EnemyMissile")
        {
            if (tag == "SuperBounds") { other.gameObject.GetComponent<ProjectileBehavior>().DestroyProjectile(); }
            else { return; }
        }
        // Everything else should be deleted
        else
        {
            Destroy(other.gameObject);
        }
    }
}
