using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Movement : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;

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
    
    public bool awake;

    LineRenderer lr;


    // Start is called before the first frame update
    void Awake()
    {
        nextMoveTime = Mathf.Infinity;
        startPos = transform.position;
        endPos = startPos;
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (awake)
        {
            if (Time.time >= nextMoveTime)
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
    }

    void StartMove()
    {
        startTime = Time.time;
        float nextX = Random.Range(minX, maxX);
        float nextY = Random.Range(minY, maxY);
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
        awake = true;
    }
}
