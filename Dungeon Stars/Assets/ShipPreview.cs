using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPreview : MonoBehaviour
{
    [Header("Descriptive Info")]
    public string shipName;
    public string shipDesc;
    public string shipLore;
    public string primaryWeap;
    public string secondaryWeap;
    public string tertiaryWeap;
    public ShipsEnum.ShipID id;

    [Header("Numbers Info")]
    public float maxHP;
    public float maxShield;
    public float shieldRecharge;
    public float speed;
    public float primaryFireRate;
    public float secondaryFireRate;

    [Header("References")]
    public GameObject primary;
    public GameObject secondary;

    public Transform hardpoints;
    public Animator animator;

    float nextPrimaryFire = 0f;
    float nextSecondaryFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetTrigger("FlyIntoScreen");
        nextPrimaryFire = Time.time + 1.5f;
        nextSecondaryFire = Time.time + 1.5f;

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextPrimaryFire)
        {
            Shoot(primary);
            nextPrimaryFire = Time.time + primaryFireRate;
        }
        if (Time.time > nextSecondaryFire)
        {
            Shoot(secondary);
            nextSecondaryFire = Time.time + secondaryFireRate;
        }
    }

    void Shoot(GameObject projectile)
    {
        Destroy(
        Instantiate(projectile, hardpoints.position, hardpoints.rotation), 5f
        );
    }

    public void LeaveScreen()
    {
        animator.SetTrigger("FlyOutOfScreen");
        Destroy(this.gameObject, 2);
        nextPrimaryFire = Mathf.Infinity;
        nextSecondaryFire = Mathf.Infinity;
    }
}
