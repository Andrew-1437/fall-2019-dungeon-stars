using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the Hexxed Status effect
/// 
/// Hex is a stacking debuff that stacks up to six times, each stack increases the 
/// amount of damage taken and reduces movement speed. At max stacks, the amount
/// per stack is doubled and any attacks that apply Hex will have their damage doubled,
/// in addition to the damage bonus provided by existing Hex stacks.
/// 
/// Targets with any stacks of Hex will "Hexplode" on death, applying their current stacks
/// of Hex to nearby enemies and causing damage proportional to the amount of Hex stacks they
/// currently have, potentially causing a chain reaction if the new enemy dies by the blast.
/// 
/// Stacks are applied by the HexApplicator class which inherits from ProjectileBehavior.
/// This class should be a property on anything capable of taking damage, such as
/// the ObstacleBehavior, PlayerController, and BossBehavior classes
/// </summary>
public class HexStatus
{
    private const float DamageBuffPerStack = .05f;
    private const float SpeedDebuffPerStack = .06f;
    private const float StackDecayRate = 3f;
    private const float DamagePerStackOnDeath = 2f;

    public int Stacks { get; set; }
    public float TimeForNextDecay { get; set; }

    private float immunityTime = 0;

    /// <summary>
    /// Constructor. Sets current stacks to 0
    /// </summary>
    public HexStatus()
    {
        Stacks = 0;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Stacks == 0) { return; }

        if (Time.time >= TimeForNextDecay)
        {
            ApplyStacks(-1);
        }
    }

    /// <summary>
    /// Apples stacks of Hex
    /// </summary>
    /// <param name="stacks">Number of stacks to apply. Default is one</param>
    public void ApplyStacks(int stacks = 1)
    {
        if(Time.time <= immunityTime) { return; }

        Stacks += stacks;
        Stacks = Mathf.Clamp(Stacks, 0, 6);

        TimeForNextDecay = Time.time + StackDecayRate;
    }

    /// <summary>
    /// Apples stacks of Hex
    /// </summary>
    /// <param name="stacks">Number of stacks to apply</param>
    /// <param name="overflow">Output param. True if already at max stacks when applying this stack</param>
    public void ApplyStacks(int stacks, out bool overflow)
    {
        overflow = Stacks >= 6;
        ApplyStacks(stacks);
    }

    /// <summary>
    /// Begins a brief timer to cause a hexplosion at the target location.
    /// This didn't work how I expected it to so WIP
    /// </summary>
    /// <param name="transform">The transform of the GameObject to spawn the hexplosion</param>
    public IEnumerator PreHexplode(Transform transform)
    {
        yield return new WaitForSeconds(.1f);

        Hexplosion(transform);
    }

    /// <summary>
    /// Causes a hexplosion when the target object has stacks of Hex and dies.
    /// The hexplosion applies the amount of Stacks that the object currently has to 
    /// nearby objects and deals damage based on how many stacks they had.
    /// </summary>
    /// <param name="transform">The transform of the GameObject to spawn the hexplosion</param>
    public void Hexplosion(Transform transform)
    {
        HexApplicator hexChain = Object.Instantiate(OmniController.omniController.Hexplosion, 
            transform.position, transform.rotation).GetComponent<HexApplicator>();
        hexChain.stacksToApply = Stacks;
        hexChain.dmgValue = GetHexDamageOnDeath();
    }

    /// <summary>
    /// Current damage modifier that should be applied by stacks of Hex
    /// If at max stacks, double the amount of bonus damage per stack
    /// </summary>
    public float GetHexDmgMod()
    {
        if (IsAtMaxStacks())
        {
            return 1f + (DamageBuffPerStack * 2f * Stacks);
        }
        return 1f + (DamageBuffPerStack * Stacks);
    }

    /// <summary>
    /// Current speed modifier that should be applied by stacks of Hex
    /// If at max stacks, double the amount of bonus slow per stack
    /// </summary>
    public float GetHexSpeedMod()
    {
        if (IsAtMaxStacks())
        {
            return Mathf.Clamp(1f - (SpeedDebuffPerStack * 2f * Stacks), .1f, 1f);
        }
        return Mathf.Clamp(1f - (SpeedDebuffPerStack * Stacks), .1f, 1f);
    }

    /// <summary>
    /// Amount of damage to deal in the hexplosion when a target affected by Hex dies
    /// </summary>
    public float GetHexDamageOnDeath()
    {
        return DamagePerStackOnDeath * Stacks;
    }

    /// <summary>
    /// Whether the Hex status is aat max stacks
    /// </summary>
    /// <returns>True if Stacks == 6</returns>
    public bool IsAtMaxStacks()
    {
        return Stacks == 6;
    }

    /// <summary>
    /// Sets the current stacks to 0 and prevents new stacks from being applied for 3 seconds
    /// </summary>
    /// <param name="immuneTime">Amount of time to make immune to new stacks of Hex. Defaults to 3 seconds</param>
    public void CleanseHex(float immuneTime = 3f)
    {
        Stacks = 0;
        immunityTime = Time.time + immuneTime;
    }
}
