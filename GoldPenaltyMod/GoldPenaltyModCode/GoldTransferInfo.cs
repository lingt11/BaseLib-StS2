using MegaCrit.Sts2.Core.Entities.Characters;

namespace GoldPenaltyMod.GoldPenaltyModCode;

/// <summary>
/// Stores the result of a gold transfer so the reward screen patch can display it.
/// Populated by <see cref="Patches.BattleEndPatch"/> and consumed by <see cref="Patches.RewardScreenPatch"/>.
/// </summary>
public static class GoldTransferInfo
{
    /// <summary>
    /// Represents a single gold transfer event between two players.
    /// </summary>
    public sealed class TransferResult
    {
        public required CharacterBattle Loser { get; init; }
        public required CharacterBattle Winner { get; init; }
        public required string LoserName { get; init; }
        public required string WinnerName { get; init; }
        public required int TransferAmount { get; init; }
        public required int LoserDamage { get; init; }
        public required int WinnerDamage { get; init; }
    }

    /// <summary>
    /// The pending transfer result to display on the reward screen, or null if none.
    /// </summary>
    public static TransferResult? Pending { get; set; }

    /// <summary>
    /// Clears the pending transfer result after it has been consumed.
    /// </summary>
    public static void Clear()
    {
        Pending = null;
    }
}
