using Godot;

namespace GoldPenaltyMod.GoldPenaltyModCode.UI;

/// <summary>
/// Displays a banner notification at the top-center of the screen summarizing the gold transfer
/// result after a battle. The banner slides in from the top, stays visible briefly, then fades out.
/// Automatically removes itself from the scene tree after the animation completes.
/// </summary>
public partial class GoldTransferBanner : CanvasLayer
{
    private const float DisplayDuration = 3.0f;
    private const float FadeInDuration = 0.3f;
    private const float FadeOutDuration = 0.5f;
    private const int TitleFontSize = 22;
    private const int DetailFontSize = 18;

    /// <summary>
    /// Creates and displays a gold transfer summary banner on the screen.
    /// </summary>
    /// <param name="sceneRoot">The root node to attach the banner to.</param>
    /// <param name="loserName">Name of the player who lost gold.</param>
    /// <param name="winnerName">Name of the player who gained gold.</param>
    /// <param name="amount">The gold amount transferred.</param>
    /// <param name="loserDamage">Total damage dealt by the losing player.</param>
    /// <param name="winnerDamage">Total damage dealt by the winning player.</param>
    public static void Show(Node sceneRoot, string loserName, string winnerName, int amount,
        int loserDamage, int winnerDamage)
    {
        var banner = new GoldTransferBanner();
        banner.Setup(loserName, winnerName, amount, loserDamage, winnerDamage);
        sceneRoot.AddChild(banner);
        banner.StartAnimation();
    }

    private PanelContainer? _panel;

    private void Setup(string loserName, string winnerName, int amount,
        int loserDamage, int winnerDamage)
    {
        Layer = 100;

        _panel = new PanelContainer();

        var styleBox = new StyleBoxFlat
        {
            BgColor = new Color(0.1f, 0.1f, 0.15f, 0.85f),
            CornerRadiusBottomLeft = 8,
            CornerRadiusBottomRight = 8,
            ContentMarginLeft = 20,
            ContentMarginRight = 20,
            ContentMarginTop = 12,
            ContentMarginBottom = 12
        };
        _panel.AddThemeStyleboxOverride("panel", styleBox);

        var vbox = new VBoxContainer();
        vbox.Alignment = BoxContainer.AlignmentMode.Center;

        // Title line
        var titleLabel = new Label();
        titleLabel.Text = "⚔ Battle Gold Penalty ⚔";
        titleLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.84f, 0.0f));
        titleLabel.AddThemeFontSizeOverride("font_size", TitleFontSize);
        titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(titleLabel);

        // Loser line (red)
        var loserLabel = new Label();
        loserLabel.Text = $"▼ {loserName} (Damage: {loserDamage})  -{amount} Gold";
        loserLabel.AddThemeColorOverride("font_color", new Color(1.0f, 0.3f, 0.3f));
        loserLabel.AddThemeFontSizeOverride("font_size", DetailFontSize);
        loserLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(loserLabel);

        // Winner line (green)
        var winnerLabel = new Label();
        winnerLabel.Text = $"▲ {winnerName} (Damage: {winnerDamage})  +{amount} Gold";
        winnerLabel.AddThemeColorOverride("font_color", new Color(0.3f, 1.0f, 0.3f));
        winnerLabel.AddThemeFontSizeOverride("font_size", DetailFontSize);
        winnerLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(winnerLabel);

        _panel.AddChild(vbox);

        // Position at top-center of screen
        _panel.AnchorLeft = 0.5f;
        _panel.AnchorRight = 0.5f;
        _panel.AnchorTop = 0;
        _panel.AnchorBottom = 0;
        _panel.GrowHorizontal = Control.GrowDirection.Both;
        _panel.OffsetTop = -100; // Start off-screen

        AddChild(_panel);
    }

    private void StartAnimation()
    {
        if (_panel == null) return;

        var tween = CreateTween();

        // Slide in from top
        tween.TweenProperty(_panel, "offset_top", 20.0f, FadeInDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);

        // Hold
        tween.TweenInterval(DisplayDuration);

        // Fade out
        tween.TweenProperty(_panel, "modulate:a", 0.0f, FadeOutDuration)
            .SetEase(Tween.EaseType.In);

        // Remove from scene
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
