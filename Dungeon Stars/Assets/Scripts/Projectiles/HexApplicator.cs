using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexApplicator : ProjectileBehavior
{
    public int stacksToApply;

    /// <summary>
    /// Applies the projectile's effects and applies a number of stacks of hex
    /// If at max stacks of hex, applies "overflow" which doubles the damage of this projectile
    /// </summary>
    /// <param name="target">Target this projectile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(ObstacleBehavior target)
    {
        bool overflow;
        target.hex.ApplyStacks(stacksToApply, out overflow);

        if (overflow) { dmgValue *= 2; }

        base.ApplyProjectile(target);
    }

    /// <summary>
    /// Applies the projectile's effects and applies a number of stacks of hex
    /// If at max stacks of hex, applies "overflow" which doubles the damage of this projectile
    /// </summary>
    /// <param name="target">Target this projectile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(BossBehavior target)
    {
        bool overflow;
        target.hex.ApplyStacks(stacksToApply, out overflow);

        if (overflow) { dmgValue *= 2; }

        base.ApplyProjectile(target);
    }

    /// <summary>
    /// Applies the projectile's effects and applies a number of stacks of hex
    /// If at max stacks of hex, applies "overflow" which doubles the damage of this projectile
    /// </summary>
    /// <param name="target">Target this projectile hit. Pass in "this" from the target object</param>
    public override void ApplyProjectile(PlayerController target)
    {
        bool overflow;
        target.hex.ApplyStacks(stacksToApply, out overflow);

        if (overflow) { dmgValue *= 2; }

        base.ApplyProjectile(target);
    }
}
