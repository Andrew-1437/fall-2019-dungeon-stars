using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexStatus
{
    private const float DamageBuffPerStack = .05f;
    private const float SpeedDebuffPerStack = .1f;
    private const float StackDecayRate = 3f;
    private const float DamagePerStackOnDeath = 2f;

    public int Stacks { get; set; }
    public float TimeForNextDecay { get; set; }

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
        Stacks += stacks;
        Stacks = Mathf.Clamp(Stacks, 0, 6);

        TimeForNextDecay = Time.time + StackDecayRate;
    }

    /// <summary>
    /// Apples stacks of Hex
    /// </summary>
    /// <param name="stacks">Number of stacks to apply. Default is one</param>
    /// <param name="overflow">Output param. True if already at max stacks when applying this stack</param>
    public void ApplyStacks(int stacks, out bool overflow)
    {
        overflow = Stacks >= 6;
        ApplyStacks(stacks);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hexplosionObj"></param>
    /// <param name="transform"></param>
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
}
