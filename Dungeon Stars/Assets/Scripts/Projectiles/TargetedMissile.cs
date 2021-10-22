using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedMissile : BombBehavior
{
    [Header("Targeted Missile Target")]
    public Transform targetPos;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        // The targeted missile comes from the top of the screen down onto the target
        // If the missile is at or below the target's y position, detonate
        if (transform.position.y <= targetPos.position.y + .1f)
            Detonate();
    }
}
