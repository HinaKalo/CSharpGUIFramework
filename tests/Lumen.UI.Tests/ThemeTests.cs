using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Lumen.UI.Services;
using Xunit;

namespace Lumen.UI.Tests;

public class ThemeTests
{
    private static ISolidColorBrush Brush(StyledElement host, string key)
    {
        Assert.True(host.TryFindResource(key, host.ActualThemeVariant, out var value), $"Missing resource {key}");
        return Assert.IsAssignableFrom<ISolidColorBrush>(value);
    }

    [AvaloniaFact]
    public void Semantic_tokens_resolve_in_both_variants()
    {
        var window = new Window { Content = new TextBlock { Text = "x" } };
        window.Show();

        ThemeManager.SetMode(ThemeMode.Light);
        var lightBg = Brush(window, "Lumen.Brush.Background").Color;
        var lightText = Brush(window, "Lumen.Brush.TextPrimary").Color;

        ThemeManager.SetMode(ThemeMode.Dark);
        var darkBg = Brush(window, "Lumen.Brush.Background").Color;
        var darkText = Brush(window, "Lumen.Brush.TextPrimary").Color;

        Assert.NotEqual(lightBg, darkBg);
        Assert.NotEqual(lightText, darkText);
        Assert.True(ThemeManager.IsDark);

        ThemeManager.SetMode(ThemeMode.Light);
        window.Close();
    }

    [AvaloniaFact]
    public void Accent_can_be_changed_at_runtime()
    {
        var border = new Border();
        border.Bind(Border.BackgroundProperty, border.GetResourceObservable("Lumen.Brush.Accent"));
        var window = new Window { Content = border };
        window.Show();

        var teal = Color.Parse("#0D9488");
        ThemeManager.SetAccent(teal);

        Assert.Equal(teal, ThemeManager.Accent);
        var bg = Assert.IsAssignableFrom<ISolidColorBrush>(border.Background);
        Assert.Equal(teal, bg.Color);

        ThemeManager.SetAccent(LumenTheme.DefaultAccent);
        Assert.Equal(LumenTheme.DefaultAccent, ((ISolidColorBrush)border.Background!).Color);
        window.Close();
    }

    [AvaloniaFact]
    public void Fluent_keys_are_bridged_to_lumen_tokens()
    {
        var window = new Window { RequestedThemeVariant = ThemeVariant.Light };
        window.Show();

        Assert.Equal(Brush(window, "Lumen.Brush.BorderStrong").Color, Brush(window, "TextControlBorderBrush").Color);
        Assert.True(window.TryFindResource("ControlCornerRadius", out var radius));
        Assert.Equal(new CornerRadius(8), radius);
        window.Close();
    }
}
