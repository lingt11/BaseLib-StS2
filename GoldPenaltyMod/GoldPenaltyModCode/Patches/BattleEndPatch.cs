using Godot;
using HarmonyLib;
using GoldPenaltyMod.GoldPenaltyModCode.UI;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that triggers gold redistribution when a combat encounter ends in victory.
///
/// Only active in multiplayer mode. After a battle is won, this patch:
/// 1. Identifies the player who dealt the least total damage
/// 2. Identifies the player who dealt the most total damage
/// 3. Deducts gold from the lowest-damage player (amount based on current act)
/// 4. Awards the same gold to the highest-damage player
/// 5. Stores transfer data and shows a banner notification
///
/// Gold penalty scales with act progression: Act 1 = 5, Act 2 = 10, Act 3 = 15.
/// If only one player participated or if all players dealt equal damage, no gold is transferred.
/// If the lowest-damage player has fewer gold than the penalty, only their available gold is transferred.
/// </summary>
[HarmonyPatch(typeof(CombatManager), nameof(CombatManager.EndCombatInternal))]
public static class BattleEndPatch
{
    /// <summary>
    /// Prefix patch that executes before EndCombatInternal to redistribute gold.
    /// Runs as a Prefix so that damage data is still available (history is cleared during EndCombatInternal).
    /// Skips entirely if not in multiplayer mode.
    /// </summary>
    public static void Prefix()
    {
        if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
        {
            return;
        }

        var damageData = DamageTracker.GetAllDamage();

        // Need at least 2 players to compare
        if (damageData.Count < 2)
        {
            MainFile.Logger.Info("Skipping gold transfer: fewer than 2 players tracked.");
            DamageTracker.Reset();
            return;
        }

        var lowestPlayer = DamageTracker.GetLowestDamagePlayer();
        var highestPlayer = DamageTracker.GetHighestDamagePlayer();

        // If the same player or either is null, skip
        if (lowestPlayer == null || highestPlayer == null || lowestPlayer == highestPlayer)
        {
            MainFile.Logger.Info("Skipping gold transfer: same player or no valid players found.");
            DamageTracker.Reset();
            return;
        }

        int lowestDamage = damageData[lowestPlayer];
        int highestDamage = damageData[highestPlayer];

        // If everyone dealt the same damage, no penalty
        if (lowestDamage == highestDamage)
        {
            MainFile.Logger.Info("Skipping gold transfer: all players dealt equal damage.");
            DamageTracker.Reset();
            return;
        }

        // Gold penalty scales with current act: Act 1 = 5, Act 2 = 10, Act 3 = 15
        int goldPenalty = DamageTracker.GetGoldPenaltyForCurrentAct();

        // Calculate actual transfer amount (cannot take more gold than the player has)
        int currentGold = lowestPlayer.Gold;
        int transferAmount = Math.Min(goldPenalty, currentGold);

        if (transferAmount > 0)
        {
            string lowestName = lowestPlayer.Creature?.Name ?? "Unknown";
            string highestName = highestPlayer.Creature?.Name ?? "Unknown";

            // Apply gold changes immediately
            lowestPlayer.Gold -= transferAmount;
            highestPlayer.Gold += transferAmount;

            int actNumber = DamageTracker.GetCurrentActNumber();
            MainFile.Logger.Info(
                $"Gold transfer (Act {actNumber}, penalty: {goldPenalty}): " +
                $"{lowestName} (damage: {lowestDamage}) lost {transferAmount} gold. " +
                $"{highestName} (damage: {highestDamage}) gained {transferAmount} gold.");

            // Store transfer data for display
            GoldTransferInfo.Pending = new GoldTransferInfo.TransferResult
            {
                Loser = lowestPlayer,
                Winner = highestPlayer,
                LoserName = lowestName,
                WinnerName = highestName,
                TransferAmount = transferAmount,
                LoserDamage = lowestDamage,
                WinnerDamage = highestDamage
            };

            // Show banner notification via the scene tree
            var sceneTree = Engine.GetMainLoop() as SceneTree;
            if (sceneTree?.Root != null)
            {
                GoldTransferBanner.Show(sceneTree.Root, lowestName, highestName,
                    transferAmount, lowestDamage, highestDamage);
            }
        }
        else
        {
            MainFile.Logger.Info(
                $"No gold transferred: {lowestPlayer.Creature?.Name ?? "Unknown"} has no gold to lose.");
        }

        DamageTracker.Reset();
    }
}
