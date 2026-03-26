using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace GoldPenaltyMod.GoldPenaltyModCode;

/// <summary>
/// Tracks total damage dealt by each player during a single combat encounter.
/// Used in multiplayer to determine which player dealt the most/least damage.
/// Also provides act-based gold penalty amounts (Act 1: 5, Act 2: 10, Act 3: 15).
/// </summary>
public static class DamageTracker
{
    /// <summary>
    /// Reflection accessor for the private RunManager.State property.
    /// </summary>
    private static readonly MethodInfo? StateGetter =
        AccessTools.PropertyGetter(typeof(RunManager), "State");

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

    /// <summary>
    /// Determines the current act number (1, 2, or 3) using reflection on RunManager.State.
    /// Returns 1 as the default if the act cannot be determined.
    /// </summary>
    public static int GetCurrentActNumber()
    {
        try
        {
            if (StateGetter == null) return 1;

            var state = StateGetter.Invoke(RunManager.Instance, null);
            if (state == null) return 1;

            var stateType = state.GetType();

            // Try CurrentActIndex property (0-based index)
            var actIndexProp = AccessTools.Property(stateType, "CurrentActIndex");
            if (actIndexProp != null)
            {
                var indexObj = actIndexProp.GetValue(state);
                if (indexObj is int index)
                {
                    return index + 1; // Convert 0-based to 1-based
                }
            }

            // Fallback: try CurrentAct property and determine act number by type name
            var currentActProp = AccessTools.Property(stateType, "CurrentAct");
            if (currentActProp != null)
            {
                var actModel = currentActProp.GetValue(state);
                if (actModel != null)
                {
                    return actModel.GetType().Name switch
                    {
                        "Overgrowth" or "Underdocks" => 1,
                        "Hive" => 2,
                        "Glory" => 3,
                        _ => 1
                    };
                }
            }

            return 1;
        }
        catch
        {
            return 1;
        }
    }

    /// <summary>
    /// Returns the gold penalty amount based on the current act/map:
    /// Act 1 = 5 gold, Act 2 = 10 gold, Act 3 = 15 gold.
    /// </summary>
    public static int GetGoldPenaltyForCurrentAct()
    {
        int actNumber = GetCurrentActNumber();
        return actNumber switch
        {
            1 => 5,
            2 => 10,
            3 => 15,
            _ => 5
        };
    }
}
