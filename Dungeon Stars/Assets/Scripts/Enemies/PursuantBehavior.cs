using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuantBehavior : StdEnemyBehavior
{
    public float Distance;  // Represents how far ahead of the target the pursuant will set the target point
    public float Variance;  // Represents how far from the target point the pursuant will wander
    
    public GameObject DebugMarker;

    private Vector3 offset; // When created, will set a random offset from the target position that is always applied
    private Vector3 targetPosition;

    GameObject targetPlayer;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        shooter = GetComponent<Shooter>();
        speed += Random.Range(-1f, 1f);

        offset = Random.insideUnitSphere * (Distance / 2f);

        if (OmniController.omniController.enableDebug) 
        { 
            DebugMarker.SetActive(true); 
        }
    }

    protected new void FixedUpdate()
    {
        if (!gm.gameStart) { return; }

        if (!awake)
        {
            rb.velocity = Vector2.down;
            return;
        }
        
        // If player is null, find a new player to track
        if (targetPlayer == null)
        {
            targetPlayer = Functions.FindNearestPlayer(transform);
        }

        // If player is still null, stay at current position 
        if (targetPlayer != null) 
        {
            targetPosition = targetPlayer.transform.position +  
                offset +
                (Vector3.up * Distance) + 
                (Random.insideUnitSphere * Variance);

            Functions.RotateTowardsTarget(gameObject, targetPlayer, turn);
            DebugMarker.transform.position = targetPosition;
        }

        float force = speed * GetSpeedMod();
        rb.AddForce((targetPosition - transform.position) * force);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        rb.AddForce(Vector2.down * speed * 3f, ForceMode2D.Impulse);
    }
}
