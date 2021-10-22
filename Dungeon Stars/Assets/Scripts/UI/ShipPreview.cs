using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPreview : MonoBehaviour
{
    [Tooltip("Setting this will auto fill the values below in runtime")]
    public PlayerController referencedShip;

    public bool unlocked;   // If false, the ship will be displayed as a silouhette
    public string howToUnlock;  // Text on how to unlock ship, displayed instead of description
    public string shipLore;
    
    // Descriptive info - Filled in by referencedShip
    [Header("Descriptive Info - This gets auto filled in")]
    public string shipName;
    public string shipDesc;
    public string primaryWeap;
    public string secondaryWeap;
    public string tertiaryWeap;
    public ShipsEnum.ShipID id;

    // Numbers info - Filled in by referencedShip
    [Header("Numbers Info - This gets auto filled in")]
    public float maxHP;
    public float maxShield;
    public float shieldRecharge;
    public float speed;
    public float primaryFireRate;
    public float secondaryFireRate;

    // Weapons info - Filled in by referencedShip
    GameObject[] primary;
    GameObject[] secondary;

    [Header("References")]
    public Transform hardpoints;
    public Animator animator;
    public SpriteRenderer sprite;

    float nextPrimaryFire = 0f;
    float nextSecondaryFire = 0f;

    // Power level to display - ship should cycle through its weapon's power levels
    int displayedPower = 0;
    float timeBetweenPowerUp = 2;
    float nextPowerUp = Mathf.Infinity;

    // Start is called before the first frame update
    void Start()
    {
        shipName = referencedShip.shipName;
        shipDesc = referencedShip.desc;
        primaryWeap = referencedShip.primaryWeap;
        secondaryWeap = referencedShip.secondaryWeap;
        tertiaryWeap = referencedShip.tertiaryWeap;
        id = referencedShip.id;

        maxHP = referencedShip.maxHp;
        maxShield = referencedShip.maxShield;
        shieldRecharge = referencedShip.shieldRecharge;
        speed = referencedShip.speed;
        primaryFireRate = referencedShip.fireRate;
        secondaryFireRate = referencedShip.secondaryFireRate;

        primary = referencedShip.primary;
        secondary = referencedShip.secondary;

        animator.SetTrigger("FlyIntoScreen");
        nextPrimaryFire = Time.time + 1.5f;
        nextSecondaryFire = Time.time + 1.5f;

        nextPowerUp = Time.time + timeBetweenPowerUp + 1.5f;

        if (!unlocked)
        {
            sprite.color = Color.black;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (unlocked)
        {
            if (Time.time >= nextPowerUp)
            {
                displayedPower++;
                nextPowerUp = Time.time + timeBetweenPowerUp;
            }

            int pow = displayedPower % primary.Length;

            if (Time.time > nextPrimaryFire)
            {
                Shoot(primary[pow]);
                nextPrimaryFire = Time.time + primaryFireRate;
            }
            if (Time.time > nextSecondaryFire)
            {
                Shoot(secondary[pow]);
                nextSecondaryFire = Time.time + secondaryFireRate;
            }
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
