using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace Lumen.UI;

/// <summary>
/// Lumen design system entry point. Add <c>&lt;lumen:LumenTheme /&gt;</c> to <c>Application.Styles</c>
/// instead of <c>FluentTheme</c>; it contains FluentTheme plus the Lumen tokens and control styles.
/// </summary>
public class LumenTheme : Styles
{
    /// <summary>The default accent (indigo-600).</summary>
    public static readonly Color DefaultAccent = Color.Parse("#4F46E5");

    public LumenTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
        Fluent = this.OfType<FluentTheme>().First();
    }

    /// <summary>The underlying Fluent theme instance.</summary>
    public FluentTheme Fluent { get; }

    /// <summary>
    /// Accent color used by both light and dark variants. All Lumen accent brushes and Fluent
    /// accent resources derive from it, so changing it recolors the whole UI at runtime.
    /// </summary>
    public Color Accent
    {
        get => Fluent.Palettes.TryGetValue(ThemeVariant.Light, out var p) ? p.Accent : DefaultAccent;
        set
        {
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            {
                if (!Fluent.Palettes.TryGetValue(variant, out var palette))
                {
                    palette = new ColorPaletteResources();
                    Fluent.Palettes[variant] = palette;
                }

                palette.Accent = value;
            }
        }
    }

    /// <summary>Control density inherited from Fluent (Normal / Compact).</summary>
    public DensityStyle Density
    {
        get => Fluent.DensityStyle;
        set => Fluent.DensityStyle = value;
    }
}
