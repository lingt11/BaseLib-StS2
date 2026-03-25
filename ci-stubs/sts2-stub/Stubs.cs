// Minimal stub types from sts2.dll required to compile STS2 mods.
// These stubs are used only for CI compilation and do not contain game logic.

namespace MegaCrit.Sts2.Core.Entities.Characters
{
    public partial class CharacterBattle : Godot.Node
    {
        public int Gold { get; set; }
        public new string Name { get; } = string.Empty;
        public bool IsPlayer { get; }

        public void TakeDamage(int amount, CharacterBattle attacker) { }
    }
}

namespace MegaCrit.Sts2.Core.Systems
{
    public class BattleSystem
    {
        public void OnBattleVictory() { }
        public void OnBattleStart() { }
    }

    public static class RunManager
    {
        public static bool IsCoop { get; }
    }
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
    public enum LogType
    {
        Generic
    }

    public class Logger
    {
        public Logger(string id, LogType type) { }
        public void Log(string message) { }
    }
}
