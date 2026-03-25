using HarmonyLib;
using MegaCrit.Sts2.Core.Systems;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that resets the damage tracker at the start of each new combat encounter.
/// This ensures damage from previous battles does not carry over.
/// </summary>
[HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.OnBattleStart))]
public static class BattleStartPatch
{
    /// <summary>
    /// Prefix patch that clears all tracked damage data before a new battle begins.
    /// </summary>
    public static void Prefix()
    {
        DamageTracker.Reset();
        MainFile.Logger.Log("New battle started. Damage tracking reset.");
    }
}
