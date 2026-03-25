using Godot;

namespace GoldPenaltyMod.GoldPenaltyModCode.UI;

/// <summary>
/// Displays a floating gold change notification above a target node in the game world.
/// Shows animated text that floats upward and fades out (e.g., "-10 Gold" in red or "+10 Gold" in green).
/// Automatically removes itself from the scene tree after the animation completes.
/// </summary>
public partial class GoldFloatingText : Node2D
{
    private const float FloatDistance = 80.0f;
    private const float AnimationDuration = 1.5f;
    private const int FontSize = 28;

    private Label? _label;

    /// <summary>
    /// Creates and displays a floating gold notification above the specified target node.
    /// </summary>
    /// <param name="target">The game node (e.g., a player character) to display the text above.</param>
    /// <param name="goldAmount">The gold change amount. Positive = gain, negative = loss.</param>
    /// <param name="playerName">The name of the player for the notification.</param>
    public static void Show(Node target, int goldAmount, string playerName)
    {
        var floatingText = new GoldFloatingText();
        floatingText.Setup(goldAmount, playerName);

        target.AddChild(floatingText);
        floatingText.Position = new Vector2(0, -60);
        floatingText.StartAnimation();
    }

    private void Setup(int goldAmount, string playerName)
    {
        _label = new Label();

        bool isGain = goldAmount > 0;
        string sign = isGain ? "+" : "";
        _label.Text = $"{playerName}: {sign}{goldAmount} Gold";

        _label.AddThemeColorOverride("font_color", isGain
            ? new Color(1.0f, 0.84f, 0.0f)   // Gold/yellow for gain
            : new Color(1.0f, 0.2f, 0.2f));   // Red for loss

        _label.AddThemeFontSizeOverride("font_size", FontSize);
        _label.HorizontalAlignment = HorizontalAlignment.Center;

        AddChild(_label);
    }

    private void StartAnimation()
    {
        var tween = CreateTween();
        tween.SetParallel(true);

        // Float upward
        tween.TweenProperty(this, "position:y", Position.Y - FloatDistance, AnimationDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        // Fade out (start fading after 40% of animation)
        tween.TweenProperty(this, "modulate:a", 0.0f, AnimationDuration * 0.6f)
            .SetDelay(AnimationDuration * 0.4f)
            .SetEase(Tween.EaseType.In);

        tween.SetParallel(false);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
