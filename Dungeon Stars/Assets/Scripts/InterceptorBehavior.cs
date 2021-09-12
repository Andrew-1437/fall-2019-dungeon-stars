using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptorBehavior : MonoBehaviour
{
    public float speed;
    public float sideAcceleration;
    float sideSpeed;

    public GameObject missile;

    public float yPosToFireMissiles;    // Y value we fly to before we fire missiles and fly away

    bool awake = false;
    bool missilesFired = false;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /* When the interceptor enters the scene, it will fly forward before firing two missiles.
         * Then it will fly offscreen at an angle, accelerating rapidly towards the sides of the screen.
         */
        if (GM.gameController.gameStart)
        {
            if (awake)
            {
                rb.velocity = (Vector3.down * speed * OmniController.omniController.obstacleSpeedScale) + Vector3.down;
                if(transform.position.y < yPosToFireMissiles)
                {
                    if (!missilesFired)
                        FireTwoMissiles();
                    sideSpeed += sideAcceleration * Time.deltaTime;
                    rb.velocity = new Vector2(
                        sideSpeed,
                        rb.velocity.y);
                }
                transform.up = rb.velocity;
            }
            else
            {
                rb.velocity = Vector2.down;
            }
        }
        else
            rb.velocity = Vector2.zero;
    }

    public void FireTwoMissiles()
    {
        missilesFired = true;
        
        Destroy(
            Instantiate(missile, transform.position + Vector3.right, Quaternion.Euler(0, 0, 180)), 10f);
        Destroy(
            Instantiate(missile, transform.position + Vector3.left, Quaternion.Euler(0, 0, 180)), 10f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bounds")
        {
            awake = true;
        }
    }
}
