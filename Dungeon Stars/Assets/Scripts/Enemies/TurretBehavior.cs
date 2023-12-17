using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    #region Targeting
    [Tooltip("Leave empty or specify \"Player\" to target player")]
    public string targetTag;    // Default target is player
    public GameObject projectile;
    protected GameObject target;
    #endregion

    #region Shooting & Aiming
    public float shootDelay;
    public float fireRate;
    public float burstTime;
    protected float burstEnd = 0f;
    protected float nextBurst = Mathf.Infinity;
    protected float nextFire = Mathf.Infinity;

    public float turn;
    float turnSpeedMod = 1f;

    public Transform hardpoint;
    #endregion

    #region Boolean Flags
    public bool awake;
    bool wasSleeping;
    [Tooltip("If true, will not shoot, even if awake")]
    public bool holdFire;   
    [Tooltip("If true, the turret will not care about an attached ObstacleBehavior script. " +
        "Mainly used for player's turrets or invulnerable turrets that should not be targeted.")]
    public bool ignoreObstacle;     
    #endregion

    ObstacleBehavior thisObstacle;  // Reference to this gameObject's ObstacleBehavior script
    GM gm;

    public delegate void TurretDelegate();
    public event TurretDelegate OnBurstEnd;

    // Use this for initialization
    protected void Start () {
        nextBurst = Time.time + shootDelay;
        nextFire = 0f;
        bool wasSleeping = !awake;

        if (targetTag == "") targetTag = Tags.Player;  // If targetTag is empty, target player

        if (!ignoreObstacle)
            thisObstacle = GetComponent<ObstacleBehavior>();

        if (hardpoint == null)
        {
            hardpoint = transform;
        }

        gm = GM.GameController;

        StartCoroutine(FindTargetsAsync());
    }
	
	// Update is called once per frame
	protected void Update ()
    {
        if (!ignoreObstacle)
            awake = thisObstacle.awake;

        if(awake && wasSleeping)
        {
            wasSleeping = false;
            Awaken();
        }
        else if (!awake && !wasSleeping)
        {
            wasSleeping = true;
        }

        if (awake)
        {
            // Target is found by FindTargetsAsync() every .2 seconds, not in the update method
            
            if (target != null)
            {
                // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
                if (targetTag == Tags.Player && target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
                    turnSpeedMod = .35f;
                else
                    turnSpeedMod = 1f;

                Vector3 targetDir = target.transform.position - transform.position;

                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime * turnSpeedMod);
            }
        }

        if (awake && target != null && !holdFire && (Time.time > nextBurst || Time.time < burstEnd))
        {
            if (Time.time >= burstEnd)
            {
                nextBurst = Time.time + shootDelay;
                burstEnd = Time.time + burstTime;
                OnBurstEnd?.Invoke();
            }
            if (Time.time > nextFire && Time.time < burstEnd)
            {
                Fire();
            }

        }
    }

    public void Fire()
    {
        Destroy(
            Instantiate(projectile, hardpoint.position, hardpoint.rotation), 
            5f);
        nextFire = Time.time + fireRate * OmniController.omniController.enemyFireRateScale;
    }

    public void HoldFire(bool state)
    {
        holdFire = state;
    }

    public void Awaken()
    {
        awake = true;
        nextBurst = Time.time + burstTime;
        nextFire = 0.0f;
    }

    // Do not search for a new target every frame (bad for performance)
    // But instead every .2 seconds
    IEnumerator FindTargetsAsync()
    {
        for (; ; )
        {
            target = FindClosestByTag(targetTag);
            yield return new WaitForSeconds(.2f);
        }
    }

    GameObject FindClosestByTag(string tag)
    {
        // For turrets targeting the player, we dont need to use FindGameObjectByTag()
        // This is uglier but theoretically faster
        if(targetTag == Tags.Player)
        {
            // If player 2 does not exist but player 1 does, target player 1
            if (gm.player2 == null && gm.player != null)
            {
                return gm.player;
            }
            // If player 1 does not exist but player 2 does, target player 2
            else if (gm.player == null && gm.player2 != null)
            {
                return gm.player2;
            }
            // If both exist, compare the distance between us and both players and return closer one
            else if (gm.player != null && gm.player2 != null)
            {
                if (Vector3.Distance(transform.position, gm.player.transform.position) <=
                Vector3.Distance(transform.position, gm.player2.transform.position))
                    return gm.player;
                else
                    return gm.player2;
            }
            // If none exist, return null
            else
                return null;
        }

        // If this turret wants to target something not the player, we have to find it (performance heavy)
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (go.GetComponent<ObstacleBehavior>().awake)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }


}
