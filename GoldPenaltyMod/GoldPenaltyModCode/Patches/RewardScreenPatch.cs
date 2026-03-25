using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Rewards;
using MegaCrit.Sts2.Core.Systems;
using GoldPenaltyMod.GoldPenaltyModCode.UI;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that injects gold penalty/bonus entries into the post-battle reward screen.
///
/// When the reward screen opens after a co-op battle, this patch checks for pending
/// gold transfer data and adds styled reward entries showing:
/// - A penalty entry for the lowest-damage player (red, showing gold lost)
/// - A bonus entry for the highest-damage player (green, showing gold gained)
///
/// These entries are displayed alongside other standard battle rewards with matching UI style.
/// </summary>
[HarmonyPatch(typeof(RewardScreen), nameof(RewardScreen.Open))]
public static class RewardScreenPatch
{
    /// <summary>
    /// Postfix patch that adds gold transfer reward entries after the reward screen opens.
    /// Only runs in co-op mode and only when a gold transfer occurred in the previous battle.
    /// </summary>
    /// <param name="__instance">The reward screen instance being opened.</param>
    public static void Postfix(RewardScreen __instance)
    {
        if (!RunManager.IsCoop) return;

        var transfer = GoldTransferInfo.Pending;
        if (transfer == null) return;

        var rewardContainer = __instance.RewardListContainer;
        if (rewardContainer == null)
        {
            MainFile.Logger.Info("RewardScreen: RewardListContainer not found, cannot add gold transfer entries.");
            GoldTransferInfo.Clear();
            return;
        }

        MainFile.Logger.Info(
            $"Adding gold transfer reward entries: {transfer.LoserName} -{transfer.TransferAmount}G, " +
            $"{transfer.WinnerName} +{transfer.TransferAmount}G");

        // Add the penalty entry (gold lost by lowest-damage player)
        var penaltyEntry = GoldPenaltyRewardEntry.CreatePenalty(
            transfer.LoserName, transfer.TransferAmount, transfer.LoserDamage);
        rewardContainer.AddChild(penaltyEntry);

        // Add the bonus entry (gold gained by highest-damage player)
        var bonusEntry = GoldPenaltyRewardEntry.CreateBonus(
            transfer.WinnerName, transfer.TransferAmount, transfer.WinnerDamage);
        rewardContainer.AddChild(bonusEntry);

        // Consume the pending transfer data so it doesn't repeat
        GoldTransferInfo.Clear();
    }
}
