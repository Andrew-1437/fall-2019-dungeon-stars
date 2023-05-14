using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject[] Projectiles;

    public float BaseFireRate;
    public int BaseCapacity;
    public float BaseHeatGen;
    public int WeaponCost;

    public bool EnableSemiAuto;

    private float nextFire;
    public int Ammo;
    

    // Start is called before the first frame update
    public void Start()
    {
        if (BaseCapacity > 0)
        {
            Ammo = BaseCapacity;
        }
        nextFire = 0;
    }

    /// <summary>
    /// Attempts to fire the weapon
    /// </summary>
    /// <param name="spawner">Transform to spawn projectile in</param>
    /// <param name="player">Player firing the projectile</param>
    /// <param name="fireRateMod">Fire rate modifier based on player's buffs</param>
    /// <param name="heatFireRateMod">Fire rate modifier when there is excessive heat</param>
    /// <param name="fireButton">Name of the button used to shoot the weapon</param>
    public void Fire(Transform spawner, PlayerController player, float fireRateMod, float heatFireRateMod, string fireButton)
    {
        int powerLevel = Mathf.Clamp(player.level, 0, Projectiles.Length - 1);

        // Only shoot when key is pressed down if semi auto
        if(EnableSemiAuto && !Input.GetKeyDown(fireButton))
        {
            return;
        }

        // Only shoot when ready to fire again
        if (Time.time < nextFire)
        {
            return;
        }

        // Don't shoot when no ammo
        if (BaseCapacity > 0 && Ammo <= 0)
        {
            return;
        }

        // Set the next fire time based on the weapon's fire rate
        if (BaseFireRate > 0)
        {
            nextFire = Time.time + BaseFireRate *
                    fireRateMod * heatFireRateMod * OmniController.omniController.playerFireRateScale;
        }

        // Spawn projectile and delete it 5 seconds later if it is not already deleted
        Destroy(
            Instantiate(Projectiles[powerLevel], spawner.position, spawner.rotation),
            5);
        // Subtract score for every shot fired
        GM.gameController.AddRawScore(-Mathf.Max(WeaponCost - Mathf.FloorToInt(WeaponCost * (1f - fireRateMod)), 0));

        // Deduct one from ammo each time weapon is fired
        if (BaseCapacity > 0)
        {
            Ammo = Mathf.Clamp(Ammo - 1, 0, BaseCapacity);
        }

        // Add heat if the ship and weapon use heat
        if (BaseHeatGen > 0 && player.enableHeat)
        {
            player.AddHeat(BaseHeatGen);
        }
    }

    public void ReplenishAmmo()
    {
        Ammo = BaseCapacity;
    }
}


