using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWeapon : MonoBehaviour
{
    public float sync;

    public float aimTime;       // Time before beam "charges up" where it can aim
    public float targetTime;    // Time beam "charges up" before it shoots where it stops aiming
    public float sleepTime;     // Time while beam is doing firing animation where it doesnt aim or do anything

    float aimEndTime;
    float targetEndTime;
    float sleepEndTime;

    public float turn;
    float turnSpeedMod = 1f;
    GameObject target;

    public GameObject damager;
    float damageTime = .2f;
    float damageEndTime;
    bool firing = false;

    public AudioSource chargeSFX;
    public AudioSource fireSFX;

    bool aim = true;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        aimEndTime = Time.time + aimTime + sync;
        targetEndTime = aimEndTime + targetTime;
        sleepEndTime = targetEndTime + sleepTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > aimEndTime)
        {
            anim.SetTrigger("LockTarget");
            chargeSFX.Play();
            aimEndTime = sleepEndTime + aimTime;
            aim = false;
        }
        else if (Time.time > targetEndTime)
        {
            anim.SetTrigger("Fire");
            fireSFX.Play();
            targetEndTime = aimEndTime + targetTime;
            damageEndTime = Time.time + damageTime;
            firing = true;
        }
        else if (Time.time > sleepEndTime)
        {
            sleepEndTime = targetEndTime + sleepTime;
            aim = true;
        }

        if (aim) RotateTowards("Player");

        if (firing && Time.time > damageEndTime) firing = false;

        damager.SetActive(firing);
    }

    void RotateTowards(string tag)
    {
        target = FindClosestByTag("Player");
        // If the target is the player and they are playing the "Vector Hunter" stealth ship, the turret's turn speed is reduced
        if (target.GetComponent<PlayerController>().id.Equals(ShipsEnum.ShipID.VECTOR))
            turnSpeedMod = .35f;
        else
            turnSpeedMod = 1f;
        if (target != null)
        {
            Vector3 targetDir = target.transform.position - transform.position;

            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turn * Time.deltaTime * turnSpeedMod);
        }
    }

    GameObject FindClosestByTag(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        //print(gos.Length);
        foreach (GameObject go in gos)
        {
            if (tag == "Player")
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
            else
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
        }
        return closest;
    }
}
