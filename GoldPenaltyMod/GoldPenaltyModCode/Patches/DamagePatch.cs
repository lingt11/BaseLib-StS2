using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Characters;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that intercepts damage dealt to enemies during combat.
/// Records the damage each player character deals for end-of-combat gold redistribution.
///
/// Targets CharacterBattle.TakeDamage to capture all instances where an enemy takes damage
/// from a player character, including card attacks, power triggers, and relic effects.
/// </summary>
[HarmonyPatch(typeof(CharacterBattle), nameof(CharacterBattle.TakeDamage))]
public static class DamagePatch
{
    /// <summary>
    /// Postfix patch that records damage dealt after TakeDamage completes.
    /// Only records damage from player characters (not enemies or environment).
    /// </summary>
    /// <param name="__instance">The character that took the damage (the target).</param>
    /// <param name="amount">The damage amount that was dealt.</param>
    /// <param name="attacker">The character that dealt the damage (the source).</param>
    public static void Postfix(CharacterBattle __instance, int amount, CharacterBattle attacker)
    {
        if (attacker == null || amount <= 0) return;

        // Only track damage dealt by player characters, not by enemies
        if (!attacker.IsPlayer) return;

        DamageTracker.RecordDamage(attacker, amount);
    }
}
