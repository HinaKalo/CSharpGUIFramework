using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace Lumen.UI.Services;

/// <summary>Requested theme mode.</summary>
public enum ThemeMode
{
    System,
    Light,
    Dark,
}

/// <summary>A named accent preset.</summary>
public sealed record AccentPreset(string Name, Color Color);

/// <summary>
/// Runtime theme helpers: switch light / dark / system and recolor the accent.
/// Works against <see cref="Application.Current"/> and the <see cref="LumenTheme"/> it contains.
/// </summary>
public static class ThemeManager
{
    /// <summary>Curated accent presets that look good in both light and dark variants.</summary>
    public static IReadOnlyList<AccentPreset> AccentPresets { get; } =
    [
        new("Indigo", Color.Parse("#4F46E5")),
        new("Blue", Color.Parse("#2563EB")),
        new("Sky", Color.Parse("#0284C7")),
        new("Teal", Color.Parse("#0D9488")),
        new("Emerald", Color.Parse("#059669")),
        new("Amber", Color.Parse("#D97706")),
        new("Rose", Color.Parse("#E11D48")),
        new("Violet", Color.Parse("#7C3AED")),
        new("Slate", Color.Parse("#475569")),
    ];

    /// <summary>Raised after the theme mode or accent changes.</summary>
    public static event EventHandler? Changed;

    /// <summary>The currently requested mode.</summary>
    public static ThemeMode Mode
    {
        get
        {
            var requested = Application.Current?.RequestedThemeVariant;
            if (requested == ThemeVariant.Light)
            {
                return ThemeMode.Light;
            }

            return requested == ThemeVariant.Dark ? ThemeMode.Dark : ThemeMode.System;
        }
    }

    /// <summary>True when the effective (resolved) theme is dark.</summary>
    public static bool IsDark => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    /// <summary>The <see cref="LumenTheme"/> registered in the application styles, if any.</summary>
    public static LumenTheme? Theme => Application.Current?.Styles.OfType<LumenTheme>().FirstOrDefault();

    /// <summary>Current accent color.</summary>
    public static Color Accent => Theme?.Accent ?? LumenTheme.DefaultAccent;

    /// <summary>Switch between light, dark or following the OS.</summary>
    public static void SetMode(ThemeMode mode)
    {
        if (Application.Current is not { } app)
        {
            return;
        }

        app.RequestedThemeVariant = mode switch
        {
            ThemeMode.Light => ThemeVariant.Light,
            ThemeMode.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
        Changed?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>Flip between light and dark based on the effective theme.</summary>
    public static void Toggle() => SetMode(IsDark ? ThemeMode.Light : ThemeMode.Dark);

    /// <summary>Recolor the accent at runtime.</summary>
    public static void SetAccent(Color color)
    {
        if (Theme is not { } theme)
        {
            return;
        }

        theme.Accent = color;
        Changed?.Invoke(null, EventArgs.Empty);
    }
}
