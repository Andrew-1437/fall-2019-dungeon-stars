using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDelete : MonoBehaviour {

    //Cleans up stray projectiles
    void OnTriggerExit2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}
