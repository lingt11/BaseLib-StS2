using Godot;

namespace GoldPenaltyMod.GoldPenaltyModCode.UI;

/// <summary>
/// A custom reward entry that mimics the game's standard reward item appearance.
/// Displays a gold icon, amount text, and player info — styled to match other reward items
/// on the post-battle reward screen.
/// </summary>
public partial class GoldPenaltyRewardEntry : HBoxContainer
{
    private const int IconSize = 48;
    private const int FontSize = 24;
    private const int OutlineSize = 8;

    /// <summary>
    /// Creates a gold penalty reward entry (gold lost by the lowest-damage player).
    /// </summary>
    public static GoldPenaltyRewardEntry CreatePenalty(string playerName, int amount, int damage)
    {
        var entry = new GoldPenaltyRewardEntry();
        entry.Setup(
            isGain: false,
            playerName: playerName,
            amount: amount,
            damage: damage
        );
        return entry;
    }

    /// <summary>
    /// Creates a gold bonus reward entry (gold gained by the highest-damage player).
    /// </summary>
    public static GoldPenaltyRewardEntry CreateBonus(string playerName, int amount, int damage)
    {
        var entry = new GoldPenaltyRewardEntry();
        entry.Setup(
            isGain: true,
            playerName: playerName,
            amount: amount,
            damage: damage
        );
        return entry;
    }

    private void Setup(bool isGain, string playerName, int amount, int damage)
    {
        // Match standard reward item sizing and layout
        CustomMinimumSize = new Vector2(400, 56);
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        AddThemeConstantOverride("separation", 12);

        // --- Background panel for the row ---
        var panel = new PanelContainer();
        panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        var panelStyle = new StyleBoxFlat
        {
            BgColor = isGain
                ? new Color(0.15f, 0.22f, 0.12f, 0.90f)   // dark green tint for bonus
                : new Color(0.25f, 0.10f, 0.10f, 0.90f),  // dark red tint for penalty
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginLeft = 12,
            ContentMarginRight = 16,
            ContentMarginTop = 6,
            ContentMarginBottom = 6,
            BorderWidthBottom = 2,
            BorderWidthTop = 2,
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderColor = isGain
                ? new Color(0.55f, 0.78f, 0.35f, 0.70f)   // green border
                : new Color(0.78f, 0.35f, 0.35f, 0.70f)   // red border
        };
        panel.AddThemeStyleboxOverride("panel", panelStyle);

        var innerBox = new HBoxContainer();
        innerBox.AddThemeConstantOverride("separation", 10);

        // --- Gold coin icon (colored square placeholder matching reward style) ---
        var iconContainer = new PanelContainer();
        iconContainer.CustomMinimumSize = new Vector2(IconSize, IconSize);
        var iconStyle = new StyleBoxFlat
        {
            BgColor = isGain
                ? new Color(1.0f, 0.84f, 0.0f, 0.90f)     // gold/yellow for gain
                : new Color(0.85f, 0.25f, 0.25f, 0.90f),  // red for loss
            CornerRadiusTopLeft = 4,
            CornerRadiusTopRight = 4,
            CornerRadiusBottomLeft = 4,
            CornerRadiusBottomRight = 4
        };
        iconContainer.AddThemeStyleboxOverride("panel", iconStyle);

        var iconLabel = new Label();
        iconLabel.Text = isGain ? "▲" : "▼";
        iconLabel.HorizontalAlignment = HorizontalAlignment.Center;
        iconLabel.VerticalAlignment = VerticalAlignment.Center;
        iconLabel.AddThemeColorOverride("font_color", Colors.White);
        iconLabel.AddThemeFontSizeOverride("font_size", 20);
        iconLabel.SetAnchorsPreset(LayoutPreset.FullRect);
        iconContainer.AddChild(iconLabel);

        innerBox.AddChild(iconContainer);

        // --- Text column (amount + player info) ---
        var textBox = new VBoxContainer();
        textBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        textBox.AddThemeConstantOverride("separation", 2);

        // Main amount line
        string sign = isGain ? "+" : "-";
        var amountLabel = new Label();
        amountLabel.Text = $"{sign}{amount} Gold";
        amountLabel.AddThemeColorOverride("font_color", isGain
            ? new Color(1.0f, 0.84f, 0.0f)     // gold color for gain
            : new Color(1.0f, 0.3f, 0.3f));    // red for loss
        amountLabel.AddThemeFontSizeOverride("font_size", FontSize);
        amountLabel.LabelSettings = new LabelSettings
        {
            FontSize = FontSize,
            FontColor = isGain
                ? new Color(1.0f, 0.84f, 0.0f)
                : new Color(1.0f, 0.3f, 0.3f),
            OutlineSize = OutlineSize,
            OutlineColor = new Color(0.1f, 0.1f, 0.1f)
        };
        textBox.AddChild(amountLabel);

        // Detail line (player name + reason)
        var detailLabel = new Label();
        detailLabel.Text = isGain
            ? $"{playerName} — Highest damage ({damage})"
            : $"{playerName} — Lowest damage ({damage})";
        detailLabel.AddThemeColorOverride("font_color", new Color(0.75f, 0.75f, 0.75f));
        detailLabel.AddThemeFontSizeOverride("font_size", 16);
        textBox.AddChild(detailLabel);

        innerBox.AddChild(textBox);
        panel.AddChild(innerBox);
        AddChild(panel);

        // --- Tooltip ---
        TooltipText = isGain
            ? $"Battle Gold Bonus: +{amount} Gold\n{playerName} dealt the most damage ({damage}) this battle."
            : $"Battle Gold Penalty: -{amount} Gold\n{playerName} dealt the least damage ({damage}) this battle.";
    }
}
