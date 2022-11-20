using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StdEnemyBehavior : MonoBehaviour {

    //Movement**************
    [Header("Movement")]
    public float speed;
    public float turn;
    public Vector2 additionalMovementVector;
    public bool lookAtPlayer;
    private float hexSpeedMod;

    //Weapons*********************
    [Header("Attack")]
    public GameObject projectile;
    public Transform hardpoint;
    public float shootDelay;
    public float fireRate;
    public float burstTime;
    private float burstEnd;
    private float nextBurst;
    private float nextFire;

    protected bool awake;

    private bool stunned = false;
    private float stunTimer;

    GM gm;
    Rigidbody2D rb;
    ObstacleBehavior ob;


    protected void Start()
    {
        nextBurst = 0.0f;
        nextFire = 0.0f;
        awake = false;

        gm = GM.gameController;
        rb = GetComponent<Rigidbody2D>();
        ob = GetComponent<ObstacleBehavior>();
        
    }

    private void FixedUpdate()
    {
        if (gm.gameStart)
        {
            if (!stunned)
            {
                if (awake)
                {
                    if (ob == null) { hexSpeedMod = 1f; }
                    else { hexSpeedMod = ob.hex.GetHexSpeedMod(); }

                    rb.velocity = ((transform.up + (Vector3)additionalMovementVector).normalized * 
                        speed * OmniController.omniController.obstacleSpeedScale * hexSpeedMod) + Vector3.down;
                }
                else
                {
                    rb.velocity = Vector2.down;
                }
            }
            else
            {
                if (Time.time >= stunTimer)
                {
                    stunned = false;
                }
            }
        }
        else
            rb.velocity = Vector2.zero;

        if (lookAtPlayer)
        {
            GameObject target = GetNearestPlayer();
            if (target != null)
            {
                /*
                // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
                if (target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .35f;
                else
                    turnSpeedMod = 1f;*/

                Vector3 targetDir = target.transform.position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.fixedDeltaTime);
            }
        }
    }
    

    // Update is called once per frame
    protected void Update () {
        if (awake && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if(Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Destroy(
                    Instantiate(projectile, hardpoint.position, hardpoint.rotation), 5f);
                nextFire = Time.time + fireRate * OmniController.omniController.enemyFireRateScale;
            }
            
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Bounds")
        {
            awake = true;
        }
    }

    public void Stun(float stunTime)
    {
        stunned = true;
        stunTimer = Time.time + stunTime;
    }

    // Returns the game object of the nearest player, or null if there are none
    private GameObject GetNearestPlayer()
    {
        // If player 2 does not exist but player 1 does, target player 1
        if (GM.gameController.player2 == null && GM.gameController.player != null)
        {
            return GM.gameController.player;
        }
        // If player 1 does not exist but player 2 does, target player 2
        else if (GM.gameController.player == null && GM.gameController.player2 != null)
        {
            return GM.gameController.player2;
        }
        // If both exist, compare the distance between us and both players and return closer one
        else if (GM.gameController.player != null && GM.gameController.player2 != null)
        {
            if (Vector3.Distance(transform.position, GM.gameController.player.transform.position) <=
            Vector3.Distance(transform.position, GM.gameController.player2.transform.position))
                return GM.gameController.player;
            else
                return GM.gameController.player2;
        }
        // If none exist, return null
        else
            return null;
    }


}
