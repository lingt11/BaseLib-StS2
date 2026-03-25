using Godot;
using HarmonyLib;
using GoldPenaltyMod.GoldPenaltyModCode.UI;
using MegaCrit.Sts2.Core.Systems;

namespace GoldPenaltyMod.GoldPenaltyModCode.Patches;

/// <summary>
/// Harmony patch that triggers gold redistribution when a combat encounter ends in victory.
///
/// After a battle is won in multiplayer, this patch:
/// 1. Identifies the player who dealt the least total damage
/// 2. Identifies the player who dealt the most total damage
/// 3. Deducts 10 gold from the lowest-damage player
/// 4. Awards 10 gold to the highest-damage player
/// 5. Displays floating text and a summary banner notification
///
/// If only one player participated or if all players dealt equal damage, no gold is transferred.
/// If the lowest-damage player has fewer than 10 gold, only their available gold is transferred.
/// </summary>
[HarmonyPatch(typeof(BattleSystem), nameof(BattleSystem.OnBattleVictory))]
public static class BattleEndPatch
{
    /// <summary>
    /// Postfix patch that executes after a battle victory to redistribute gold
    /// and display UI notifications.
    /// </summary>
    public static void Postfix()
    {
        var damageData = DamageTracker.GetAllDamage();

        // Need at least 2 players to compare
        if (damageData.Count < 2)
        {
            MainFile.Logger.Log("Skipping gold transfer: fewer than 2 players tracked.");
            DamageTracker.Reset();
            return;
        }

        var lowestPlayer = DamageTracker.GetLowestDamagePlayer();
        var highestPlayer = DamageTracker.GetHighestDamagePlayer();

        // If the same player or either is null, skip
        if (lowestPlayer == null || highestPlayer == null || lowestPlayer == highestPlayer)
        {
            MainFile.Logger.Log("Skipping gold transfer: same player or no valid players found.");
            DamageTracker.Reset();
            return;
        }

        int lowestDamage = damageData[lowestPlayer];
        int highestDamage = damageData[highestPlayer];

        // If everyone dealt the same damage, no penalty
        if (lowestDamage == highestDamage)
        {
            MainFile.Logger.Log("Skipping gold transfer: all players dealt equal damage.");
            DamageTracker.Reset();
            return;
        }

        // Calculate actual transfer amount (cannot take more gold than the player has)
        int currentGold = lowestPlayer.Gold;
        int transferAmount = Math.Min(DamageTracker.GoldPenalty, Math.Max(0, currentGold));

        if (transferAmount > 0)
        {
            lowestPlayer.Gold -= transferAmount;
            highestPlayer.Gold += transferAmount;

            MainFile.Logger.Log(
                $"Gold transfer: {lowestPlayer.Name} (damage: {lowestDamage}) lost {transferAmount} gold. " +
                $"{highestPlayer.Name} (damage: {highestDamage}) gained {transferAmount} gold.");

            // Show floating text above each player character
            GoldFloatingText.Show(lowestPlayer, -transferAmount, lowestPlayer.Name);
            GoldFloatingText.Show(highestPlayer, transferAmount, highestPlayer.Name);

            // Show a summary banner at the top of the screen
            Node? sceneRoot = ((Node)lowestPlayer).GetTree()?.CurrentScene;
            if (sceneRoot != null)
            {
                GoldTransferBanner.Show(sceneRoot, lowestPlayer.Name, highestPlayer.Name,
                    transferAmount, lowestDamage, highestDamage);
            }
        }
        else
        {
            MainFile.Logger.Log(
                $"No gold transferred: {lowestPlayer.Name} has no gold to lose.");
        }

        DamageTracker.Reset();
    }
}
