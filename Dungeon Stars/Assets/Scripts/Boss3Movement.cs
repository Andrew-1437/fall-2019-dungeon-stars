using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Movement : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;

    public short moveStage;   // State machine design for movement mode: 0 = Original, 1 = Orbit

    [Tooltip("Designate this turret as the second turret for the purpose of movement")]
    public bool altTurret;  // Designate this turret as the second turret

    public float speed;
    public float timeBetweenMoves;

    float nextMoveTime;

    float journeyLength;

    float startTime;



    [Header("Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Orbit Mode")]
    public float orbitRadius;
    public float orbitDuration;
    //public float orbitMaxSpeed;
    float orbitStartTime = Mathf.Infinity;
    float orbitEndTime = Mathf.Infinity;
    public float orbitSpeed;
    int orbitMod = 1;
    
    public bool awake;
    bool orbiting = false;

    LineRenderer lr;


    // Start is called before the first frame update
    void Awake()
    {
        nextMoveTime = Mathf.Infinity;
        startPos = transform.position;
        endPos = startPos;
        lr = GetComponent<LineRenderer>();

        if (altTurret) orbitMod = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (awake)
        {
            // Randomly move across the screen in sync
            if (moveStage == 0)
            {
                // Transition to the next movement type
                if(Time.time >= orbitStartTime && Time.time >= nextMoveTime)
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
}
