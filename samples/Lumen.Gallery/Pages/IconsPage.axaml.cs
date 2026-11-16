using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Lumen.UI.Controls;
using Lumen.UI.Icons;

namespace Lumen.Gallery.Pages;

public partial class IconsPage : UserControl
{
    public IconsPage()
    {
        InitializeComponent();
        foreach (var kind in Enum.GetValues<IconKind>().Where(k => k != IconKind.None))
        {
            var label = new TextBlock
            {
                Text = kind.ToString(),
                TextTrimming = TextTrimming.CharacterEllipsis,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            label.Classes.Add("caption");
            label.Classes.Add("secondary");

            var tile = new Border
            {
                Width = 112,
                Height = 84,
                Margin = new Thickness(0, 0, 8, 8),
                CornerRadius = new CornerRadius(10),
                Child = new StackPanel
                {
                    Spacing = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children = { new Icon { Kind = kind, Size = 22 }, label },
                },
            };
            tile.Bind(Border.BackgroundProperty, tile.GetResourceObservable("Lumen.Brush.SurfaceSubtle"));
            ToolTip.SetTip(tile, $"IconKind.{kind}");
            IconGrid.Children.Add(tile);
        }
    }
}
