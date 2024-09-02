using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossDivineBombs : MonoBehaviour
{
    public GameObject Bomb;
    public float FireRate;

    private float nextFire;

    // Start is called before the first frame update
    void Start()
    {
        nextFire = Time.time + FireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextFire) 
        {
            nextFire = Time.time + FireRate;

            Transform target = Functions.FindNearestPlayer(transform)?.transform;

            if (target == null) { return; }

            Destroy(Instantiate(Bomb, target.position, Quaternion.Euler(Vector3.zero)), 10f);
        }
    }
}
