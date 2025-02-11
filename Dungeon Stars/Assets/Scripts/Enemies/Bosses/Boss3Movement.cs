﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Movement : MonoBehaviour
{
    #region References
    [Tooltip("Designate this turret as the second turret for the purpose of movement")]
    public bool altTurret;  // Designate this turret as the second turret
    public ObstacleBehavior otherTurret;   // Reference to other turret base's ObstacleBehavior
    public Boss3TurretBehavior thisTurret;   // Reference to this turret's TurretBehavior
    public ObstacleBehavior shield; // Reference to this turret's shield
    #endregion

    #region Random Move Stage
    // That moment when you spend hours to try to mimic a state machine only to find a Brackeys
    // tutorial that explains you can just use the Animator
    public short moveStage;     // State machine design for movement mode: 
                                // 0 = Original, 1 = Orbit, 2 = Spin Attack
    public float speed;
    public float timeBetweenMoves;

    Vector3 startPos;
    Vector3 endPos;

    float nextMoveTime;
    float journeyLength;
    float startTime;

    [Header("Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    #endregion

    #region Orbit Stage
    [Header("Orbit Mode")]
    public float orbitRadius;
    public float orbitDuration;
    float orbitStartTime = Mathf.Infinity;
    float orbitEndTime = Mathf.Infinity;
    public float orbitSpeed;
    int orbitMod = 1;
    #endregion

    #region Spining Stage
    [Header("Spin Mode")]
    public float spinAttackDuration;
    float spinStartTime = Mathf.Infinity;
    float spinEndTime = Mathf.Infinity;
    
    public bool awake;
    bool orbiting = false;
    bool beginSpinAttack = false;
    #endregion

    LineRenderer lr;

    // Start is called before the first frame update
    void Awake()
    {
        nextMoveTime = Mathf.Infinity;
        startPos = transform.position;
        endPos = startPos;
        lr = GetComponent<LineRenderer>();

        if (altTurret) orbitMod = -1;

        otherTurret.OnObstacleDeath += OtherTurret_OnObstacleDeath;
        shield.OnObstacleDeath += Shield_OnObstacleDeath;
        GM.OnLevelEnd += GM_OnLevelEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (awake)
        {
            // Randomly move across the screen in sync
            if (moveStage == 0)
            {
                // Transition to Spin Attack
                if (Time.time >= spinStartTime && Time.time >= nextMoveTime)
                {
                    if (beginSpinAttack)
                    {
                        thisTurret.doSpinAttack = true;
                        moveStage = 2;
                        spinStartTime = Time.time;
                        spinEndTime = spinStartTime + spinAttackDuration;
                        beginSpinAttack = false;
                        return;
                    }
                    StartMove(0f, 0f);
                    nextMoveTime = Time.time + timeBetweenMoves;
                    spinStartTime = nextMoveTime;
                    beginSpinAttack = true;
                }
                // Transition to Orbit Attack
                else if(Time.time >= orbitStartTime && Time.time >= nextMoveTime)
                {
                    if (orbiting)
                    {
                        moveStage = 1;
                        orbitStartTime = Time.time;
                        orbitEndTime = orbitStartTime + orbitDuration;
                        return;
                    }
                    StartMove(orbitRadius * orbitMod, 0f);
                    nextMoveTime = Time.time + timeBetweenMoves;
                    orbitStartTime = nextMoveTime;
                    orbiting = true;
                }
                // Randomly move across the screen
                else if (Time.time >= nextMoveTime)
                {
                    StartMove();
                    nextMoveTime = Time.time + timeBetweenMoves;
                }

                // Distance moved equals elapsed time times speed..
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.
                transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney - .5f);
                Vector3[] positions = { transform.position, endPos };
                lr.SetPositions(positions);
            }

            // Orbit around the center of the screen, going faster then slower until it stops
            if (moveStage == 1)
            {
                float t = (Time.time - orbitStartTime) * orbitSpeed;
                transform.position = new Vector3(
                    Mathf.Cos(t) * orbitRadius * orbitMod,
                    Mathf.Sin(t) * orbitRadius * orbitMod,
                    0f);

                if(Time.time >= orbitEndTime)
                {
                    orbiting = false;
                    moveStage = 0;
                    nextMoveTime = Time.time + 1.5f;
                    orbitStartTime = Mathf.Infinity;
                    if (otherTurret != null)
                        orbitStartTime = Time.time + orbitDuration * 2f;
                    else
                        spinStartTime = Time.time + orbitDuration * 2f;
                    startPos = transform.position;
                    endPos = transform.position;
                }
            }

            if (moveStage == 2)
            {
                if (Time.time >= spinEndTime)
                {
                    thisTurret.doSpinAttack = false;
                    moveStage = 0;
                    nextMoveTime = Time.time + 1.5f;
                    spinStartTime = Mathf.Infinity;
                    orbitStartTime = Time.time + orbitDuration * 2f;
                    startPos = transform.position;
                    endPos = transform.position;
                }
            }
        }
    }

    public void StartMove()
    {
        startTime = Time.time;
        float nextX = Random.Range(minX, maxX);
        float nextY = Random.Range(minY, maxY);
        startPos = transform.position;
        endPos = new Vector3(nextX, nextY, 0f);
        journeyLength = Vector3.Distance(startPos, endPos);
    }

    public void StartMove(float x, float y)
    {
        startTime = Time.time;
        float nextX = x;
        float nextY = y;
        startPos = transform.position;
        endPos = new Vector3(nextX, nextY, 0f);
        journeyLength = Vector3.Distance(startPos, endPos);
    }

    public void InitialMove(Vector3 dest)
    {
        startTime = Time.time;
        startPos = transform.position;
        endPos = dest;
        journeyLength = Vector3.Distance(startPos, endPos);
        nextMoveTime = Time.time + timeBetweenMoves * 2.5f;
        orbitStartTime = Time.time + orbitDuration * 2f;
        awake = true;
    }

    // When the other turret dies, activate our shield
    private void OtherTurret_OnObstacleDeath(ObstacleBehavior thisObstacle)
    {
        shield.gameObject.SetActive(true);
        shield.SetAwake(true);
        GetComponent<ObstacleBehavior>().invincible = true;
        speed *= 1.5f;
        timeBetweenMoves /= 1.5f;
        thisTurret.shootDelay /= 1.5f;
        orbitSpeed *= 1.5f;
        orbitEndTime = Time.time;
        spinStartTime = Time.time + spinAttackDuration * 2;
    }

    private void Shield_OnObstacleDeath(ObstacleBehavior thisObstacle)
    {
        GetComponent<ObstacleBehavior>().invincible = false;
        speed *= 1.5f;
        timeBetweenMoves /= 1.5f;
        thisTurret.shootDelay /= 1.5f;
        orbitSpeed *= 1.5f;
    }

    private void OnDestroy()
    {
        otherTurret.OnObstacleDeath -= OtherTurret_OnObstacleDeath;
        shield.OnObstacleDeath -= Shield_OnObstacleDeath;
        GM.OnLevelEnd -= GM_OnLevelEnd;
    }

    private void GM_OnLevelEnd()
    {
        otherTurret.OnObstacleDeath -= OtherTurret_OnObstacleDeath;
        shield.OnObstacleDeath -= Shield_OnObstacleDeath;
        GM.OnLevelEnd -= GM_OnLevelEnd;
    }
}
