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

namespace MegaCrit.Sts2.Core.Nodes.Screens.Rewards
{
    /// <summary>
    /// The post-battle reward screen that displays earned rewards.
    /// </summary>
    public partial class RewardScreen : Godot.Control
    {
        /// <summary>
        /// Container that holds the list of reward items.
        /// </summary>
        public Godot.VBoxContainer RewardListContainer { get; }

        /// <summary>
        /// Opens the reward screen and populates rewards.
        /// </summary>
        public void Open() { }
    }

    /// <summary>
    /// Base class for individual reward items displayed in the reward screen.
    /// </summary>
    public partial class RewardItem : Godot.HBoxContainer
    {
        /// <summary>
        /// Icon displayed next to the reward text.
        /// </summary>
        public Godot.TextureRect IconRect { get; }

        /// <summary>
        /// Label showing the reward description.
        /// </summary>
        public Godot.Label RewardLabel { get; }

        /// <summary>
        /// Tooltip text displayed on hover.
        /// </summary>
        public new string TooltipText { get; set; } = string.Empty;
    }

    /// <summary>
    /// A reward item representing a gold amount.
    /// </summary>
    public class GoldRewardItem : RewardItem
    {
        /// <summary>
        /// The amount of gold this reward gives.
        /// </summary>
        public int GoldAmount { get; set; }

        public GoldRewardItem() { }
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
        public void Info(string message) { }
    }
}
