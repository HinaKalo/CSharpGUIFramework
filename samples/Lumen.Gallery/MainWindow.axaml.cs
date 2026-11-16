using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Lumen.UI.Services;

namespace Lumen.Gallery;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Shell.NavItems = GalleryPages.Create();
        BuildAccentSwatches();
    }

    private void BuildAccentSwatches()
    {
        foreach (var preset in ThemeManager.AccentPresets)
        {
            var swatch = new Border
            {
                Width = 26,
                Height = 26,
                Margin = new Thickness(0, 0, 8, 8),
                CornerRadius = new CornerRadius(13),
                Background = new SolidColorBrush(preset.Color),
                Cursor = new Cursor(StandardCursorType.Hand),
            };
            ToolTip.SetTip(swatch, preset.Name);
            swatch.PointerPressed += (_, _) => ThemeManager.SetAccent(preset.Color);
            AccentSwatches.Children.Add(swatch);
        }
    }
}
