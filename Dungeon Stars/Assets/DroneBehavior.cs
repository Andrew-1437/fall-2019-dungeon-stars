using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBehavior : MonoBehaviour
{
    public float orbitSpeed;
    public float orbitRadius;
    public float rotationSpeed;
    public float lifetime;

    float timeSpawned;

    // Start is called before the first frame update
    void Start()
    {
        timeSpawned = Time.time;
    }

    private void FixedUpdate()
    {
        Transform playerTransform;
        if (GM.gameController.player != null)
            playerTransform = GM.gameController.player.transform;
        else
            playerTransform = GM.gameController.transform;

        transform.position = new Vector3(
            playerTransform.position.x + Mathf.Cos((Time.time - timeSpawned) * orbitSpeed) * orbitRadius,
            playerTransform.position.y + Mathf.Sin((Time.time - timeSpawned) * orbitSpeed) * orbitRadius,
            0f);
        transform.Rotate(0f, 0f, rotationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
