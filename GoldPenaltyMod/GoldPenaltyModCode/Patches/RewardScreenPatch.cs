using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that logs gold transfer information after combat ends.
///
/// When combat ends in multiplayer, this patch checks for pending gold transfer data
/// and logs the results. Visual feedback is handled by the GoldTransferBanner shown
/// in BattleEndPatch.
/// </summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.EndCombatInternal))]
public static class RewardScreenPatch
{
    /// <summary>
    /// Postfix patch that logs gold transfer information after combat ends.
    /// Only runs in multiplayer mode and only when a gold transfer occurred.
    /// </summary>
    public static void Postfix()
    {
        if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer) return;

        var transfer = GoldTransferInfo.Pending;
        if (transfer == null) return;

        MainFile.Logger.Info(
            $"Gold transfer summary: {transfer.LoserName} -{transfer.TransferAmount}G, " +
            $"{transfer.WinnerName} +{transfer.TransferAmount}G");

        // Consume the pending transfer data so it doesn't repeat
        GoldTransferInfo.Clear();
    }
}
