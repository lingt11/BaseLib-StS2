using MegaCrit.Sts2.Core.Entities.Characters;

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
    /// Maps each player's CharacterBattle instance to their cumulative damage dealt in the current combat.
    /// </summary>
    private static readonly Dictionary<CharacterBattle, int> DamageDealtByPlayer = new();

    /// <summary>
    /// Resets all tracked damage. Should be called at the start of each combat.
    /// </summary>
    public static void Reset()
    {
        DamageDealtByPlayer.Clear();
    }

    /// <summary>
    /// Records damage dealt by a specific player character.
    /// </summary>
    /// <param name="attacker">The player character who dealt the damage.</param>
    /// <param name="damage">The amount of damage dealt.</param>
    public static void RecordDamage(CharacterBattle attacker, int damage)
    {
        if (damage <= 0) return;

        if (!DamageDealtByPlayer.TryAdd(attacker, damage))
        {
            DamageDealtByPlayer[attacker] += damage;
        }
    }

    /// <summary>
    /// Gets a snapshot of all tracked player damage data.
    /// </summary>
    /// <returns>A read-only dictionary mapping players to their total damage.</returns>
    public static IReadOnlyDictionary<CharacterBattle, int> GetAllDamage()
    {
        return DamageDealtByPlayer;
    }

    /// <summary>
    /// Gets the player who dealt the most damage in the current combat, or null if no data.
    /// </summary>
    public static CharacterBattle? GetHighestDamagePlayer()
    {
        CharacterBattle? best = null;
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
    public static CharacterBattle? GetLowestDamagePlayer()
    {
        CharacterBattle? worst = null;
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
