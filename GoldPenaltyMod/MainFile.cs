using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace GoldPenaltyMod;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "GoldPenaltyMod";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Logger.Info("GoldPenaltyMod initialized: gold penalty scales by act (Act1: 5, Act2: 10, Act3: 15). Damage tracking includes all sources and block damage, excludes overkill.");
    }
}
