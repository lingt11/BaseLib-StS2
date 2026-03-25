using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Runs;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that intercepts damage dealt to enemies during combat.
/// Records the damage each player character deals for end-of-combat gold redistribution.
///
/// Targets Hook.AfterDamageGiven to capture all instances where damage is dealt,
/// including card attacks, power triggers, and relic effects.
/// Only active in multiplayer mode.
/// </summary>
[HarmonyPatch(typeof(Hook), nameof(Hook.AfterDamageGiven))]
public static class DamagePatch
{
    /// <summary>
    /// Postfix patch that records damage dealt after AfterDamageGiven completes.
    /// Only records damage from player characters in multiplayer mode.
    /// </summary>
    /// <param name="dealer">The creature that dealt the damage (the source).</param>
    /// <param name="results">The damage result containing actual damage amounts.</param>
    public static void Postfix(Creature? dealer, DamageResult results)
    {
        if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer) return;
        if (dealer == null || !dealer.IsPlayer) return;

        var player = dealer.Player;
        if (player == null) return;

        int damage = results.UnblockedDamage;
        if (damage <= 0) return;

        DamageTracker.RecordDamage(player, damage);
    }
}
