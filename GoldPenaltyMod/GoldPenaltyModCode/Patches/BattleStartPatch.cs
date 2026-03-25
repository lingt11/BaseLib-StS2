using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that resets the damage tracker at the start of each new combat encounter.
/// This ensures damage from previous battles does not carry over.
/// Only active in multiplayer mode.
/// </summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.SetUpCombat))]
public static class BattleStartPatch
{
    /// <summary>
    /// Prefix patch that clears all tracked damage data before a new battle begins.
    /// Skips if not in multiplayer mode.
    /// </summary>
    public static void Prefix()
    {
        if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer) return;

        DamageTracker.Reset();
        MainFile.Logger.Info("New co-op battle started. Damage tracking reset.");
    }
}
