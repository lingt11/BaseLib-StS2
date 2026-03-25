// Minimal stub types from sts2.dll required to compile STS2 mods.
// These stubs are used only for CI compilation and do not contain game logic.

namespace MegaCrit.Sts2.Core.Entities.Creatures
{
    public class Creature
    {
        public bool IsPlayer { get; }
        public bool IsMonster { get; }
        public MegaCrit.Sts2.Core.Entities.Players.Player? Player { get; }
        public string Name { get; } = string.Empty;
        public MegaCrit.Sts2.Core.Combat.CombatState? CombatState { get; set; }

        public DamageResult LoseHpInternal(decimal amount, MegaCrit.Sts2.Core.ValueProps.ValueProp props)
            => new DamageResult();
    }

    public class DamageResult
    {
        public int UnblockedDamage { get; set; }
        public int BlockedDamage { get; set; }
        public int OverkillDamage { get; set; }
        public int TotalDamage => UnblockedDamage + BlockedDamage;
    }
}

namespace MegaCrit.Sts2.Core.Entities.Players
{
    public class Player
    {
        public int Gold { get; set; }
        public MegaCrit.Sts2.Core.Entities.Creatures.Creature Creature { get; } = new();
    }
}

namespace MegaCrit.Sts2.Core.Combat
{
    public class CombatManager
    {
        public static CombatManager Instance { get; } = new();
        public bool IsInProgress { get; }
        public bool IsEnding { get; }

        public void SetUpCombat(CombatState state) { }
        public System.Threading.Tasks.Task EndCombatInternal()
            => System.Threading.Tasks.Task.CompletedTask;
    }

    public class CombatState
    {
        public System.Collections.Generic.IReadOnlyList<MegaCrit.Sts2.Core.Entities.Players.Player> Players { get; }
            = System.Array.Empty<MegaCrit.Sts2.Core.Entities.Players.Player>();
    }

    public enum CombatSide
    {
        Player,
        Enemy
    }
}

namespace MegaCrit.Sts2.Core.Runs
{
    public class RunManager
    {
        public static RunManager Instance { get; } = new();
        public bool IsSinglePlayerOrFakeMultiplayer { get; }
    }
}

namespace MegaCrit.Sts2.Core.Hooks
{
    public static class Hook
    {
        public static System.Threading.Tasks.Task AfterDamageGiven(
            MegaCrit.Sts2.Core.Context.PlayerChoiceContext choiceContext,
            MegaCrit.Sts2.Core.Combat.CombatState combatState,
            MegaCrit.Sts2.Core.Entities.Creatures.Creature? dealer,
            MegaCrit.Sts2.Core.Entities.Creatures.DamageResult results,
            MegaCrit.Sts2.Core.ValueProps.ValueProp props,
            MegaCrit.Sts2.Core.Entities.Creatures.Creature target,
            MegaCrit.Sts2.Core.Entities.Cards.CardModel? cardSource)
            => System.Threading.Tasks.Task.CompletedTask;
    }
}

namespace MegaCrit.Sts2.Core.ValueProps
{
    [System.Flags]
    public enum ValueProp
    {
        None = 0,
        Unblockable = 1,
    }
}

namespace MegaCrit.Sts2.Core.Context
{
    public class PlayerChoiceContext { }
}

namespace MegaCrit.Sts2.Core.Entities.Cards
{
    public class CardModel { }
}

namespace MegaCrit.Sts2.Core.Modding
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class ModInitializerAttribute : System.Attribute
    {
        public ModInitializerAttribute(string methodName) { }
    }
}

namespace MegaCrit.Sts2.Core.Logging
{
    public enum LogLevel
    {
        VeryDebug,
        Load,
        Debug,
        Info,
        Warn,
        Error
    }

    public enum LogType
    {
        Generic,
        Network,
        Actions,
        GameSync,
        VisualSync
    }

    public class Logger
    {
        public string? Context { get; set; }
        public Logger(string? context, LogType logType) { }
        public void LogMessage(LogLevel level, string text, int skipFrames) { }
        public void LogMessage(LogLevel level, LogType type, string text, int skipFrames) { }
        public void Load(string text, int skipFrames = 1) { }
        public void Debug(string text, int skipFrames = 1) { }
        public void VeryDebug(string text, int skipFrames = 1) { }
        public void Info(string text, int skipFrames = 1) { }
        public void Warn(string text, int skipFrames = 1) { }
        public void Error(string text, int skipFrames = 1) { }
    }
}
