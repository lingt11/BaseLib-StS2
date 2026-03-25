using MegaCrit.Sts2.Core.Entities.Players;

namespace GoldPenaltyMod.GoldPenaltyModCode;

/// <summary>
/// Tracks total damage dealt by each player during a single combat encounter.
/// Used in multiplayer to determine which player dealt the most/least damage.
/// </summary>
public static class DamageTracker
{
    /// <summary>
    /// Gold amount to transfer from the lowest-damage player to the highest-damage player.
    /// </summary>
    public const int GoldPenalty = 10;

    /// <summary>
    /// Maps each player instance to their cumulative damage dealt in the current combat.
    /// </summary>
    private static readonly Dictionary<Player, int> DamageDealtByPlayer = new();

    /// <summary>
    /// Resets all tracked damage. Should be called at the start of each combat.
    /// </summary>
    public static void Reset()
    {
        DamageDealtByPlayer.Clear();
    }

    /// <summary>
    /// Records damage dealt by a specific player.
    /// </summary>
    /// <param name="player">The player who dealt the damage.</param>
    /// <param name="damage">The amount of damage dealt.</param>
    public static void RecordDamage(Player player, int damage)
    {
        if (damage <= 0) return;

        if (!DamageDealtByPlayer.TryAdd(player, damage))
        {
            DamageDealtByPlayer[player] += damage;
        }
    }

    /// <summary>
    /// Gets a snapshot of all tracked player damage data.
    /// </summary>
    /// <returns>A read-only dictionary mapping players to their total damage.</returns>
    public static IReadOnlyDictionary<Player, int> GetAllDamage()
    {
        return DamageDealtByPlayer;
    }

    /// <summary>
    /// Gets the player who dealt the most damage in the current combat, or null if no data.
    /// </summary>
    public static Player? GetHighestDamagePlayer()
    {
        Player? best = null;
        int maxDamage = -1;

        foreach (var (player, damage) in DamageDealtByPlayer)
        {
            if (damage > maxDamage)
            {
                maxDamage = damage;
                best = player;
            }
        }

        return best;
    }

    /// <summary>
    /// Gets the player who dealt the least damage in the current combat, or null if no data.
    /// </summary>
    public static Player? GetLowestDamagePlayer()
    {
        Player? worst = null;
        int minDamage = int.MaxValue;

        foreach (var (player, damage) in DamageDealtByPlayer)
        {
            if (damage < minDamage)
            {
                minDamage = damage;
                worst = player;
            }
        }

        return worst;
    }
}
