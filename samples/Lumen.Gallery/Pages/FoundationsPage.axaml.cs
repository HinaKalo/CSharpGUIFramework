using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Lumen.Gallery.Pages;

public partial class FoundationsPage : UserControl
{
    private static readonly string[] s_neutral =
    [
        "Background", "Surface", "SurfaceSubtle", "SurfaceHover", "Sidebar",
        "Border", "BorderStrong", "TextPrimary", "TextSecondary", "TextTertiary",
    ];

    private static readonly string[] s_status =
    [
        "Accent", "AccentHover", "AccentSubtle", "AccentText",
        "Success", "SuccessSubtle", "Warning", "WarningSubtle",
        "Danger", "DangerSubtle", "Info", "InfoSubtle",
    ];

    public FoundationsPage()
    {
        InitializeComponent();
        Fill(NeutralSwatches, s_neutral);
        Fill(StatusSwatches, s_status);
    }

    private static void Fill(Panel panel, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            var key = "Lumen.Brush." + name;
            var chip = new Border
            {
                Height = 52,
                CornerRadius = new CornerRadius(8),
                BorderThickness = new Thickness(1),
            };
            chip.Bind(Border.BackgroundProperty, chip.GetResourceObservable(key));
            chip.Bind(Border.BorderBrushProperty, chip.GetResourceObservable("Lumen.Brush.Border"));

            var label = new TextBlock { Text = name, Margin = new Thickness(2, 6, 0, 0) };
            label.Classes.Add("caption");
            label.Classes.Add("medium");

            var item = new StackPanel
            {
                Width = 116,
                Margin = new Thickness(0, 0, 12, 12),
                Orientation = Orientation.Vertical,
            };
            item.Children.Add(chip);
            item.Children.Add(label);
            panel.Children.Add(item);
        }
    }
}
