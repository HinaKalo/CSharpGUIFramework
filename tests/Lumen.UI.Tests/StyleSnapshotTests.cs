using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Lumen.UI.Services;
using Xunit;

namespace Lumen.UI.Tests;

public class StyleSnapshotTests
{
    private static Control BuiltInControls()
    {
        var buttons = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var cls in new[] { "", "primary", "soft", "outline", "ghost", "danger", "danger-soft", "link" })
        {
            var b = new Button { Content = cls == "" ? "Default" : cls, Margin = new(0, 0, 8, 8) };
            if (cls != "")
            {
                b.Classes.Add(cls);
            }

            buttons.Children.Add(b);
        }

        var sizes = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var sm = new Button { Content = "Small" };
        sm.Classes.Add("sm");
        var lg = new Button { Content = "Large" };
        lg.Classes.Add("lg");
        lg.Classes.Add("primary");
        var pill = new Button { Content = "Pill" };
        pill.Classes.Add("pill");
        sizes.Children.Add(sm);
        sizes.Children.Add(new Button { Content = "Medium" });
        sizes.Children.Add(lg);
        sizes.Children.Add(pill);
        sizes.Children.Add(new ToggleButton { Content = "Toggle on", IsChecked = true });
        sizes.Children.Add(new Button { Content = "Disabled", IsEnabled = false });

        var filled = new TextBox { PlaceholderText = "Filled input", Width = 220 };
        filled.Classes.Add("filled");

        var inputs = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };
        inputs.Children.Add(new TextBox { PlaceholderText = "Search anything...", Width = 220 });
        inputs.Children.Add(filled);
        inputs.Children.Add(new ComboBox { Width = 180, PlaceholderText = "Choose...", ItemsSource = new[] { "One", "Two" } });

        var toggles = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 20 };
        toggles.Children.Add(new CheckBox { Content = "Checked", IsChecked = true });
        toggles.Children.Add(new CheckBox { Content = "Unchecked" });
        toggles.Children.Add(new RadioButton { Content = "Radio", IsChecked = true });
        toggles.Children.Add(new ToggleSwitch { IsChecked = true });
        toggles.Children.Add(new ToggleSwitch { IsChecked = false });

        var progress = new StackPanel { Spacing = 12 };
        progress.Children.Add(new Slider { Value = 40, Width = 360, HorizontalAlignment = HorizontalAlignment.Left });
        progress.Children.Add(new ProgressBar { Value = 65, Width = 360, HorizontalAlignment = HorizontalAlignment.Left });

        var list = new ListBox { ItemsSource = new[] { "Inbox", "Drafts", "Archive" }, SelectedIndex = 1, Width = 240, HorizontalAlignment = HorizontalAlignment.Left };

        var tabs = new TabControl
        {
            ItemsSource = new[]
            {
                new TabItem { Header = "Overview" },
                new TabItem { Header = "Activity" },
                new TabItem { Header = "Settings" },
            },
        };

        var root = new StackPanel { Margin = new(28), Spacing = 20 };
        var title = new TextBlock { Text = "Built-in controls" };
        title.Classes.Add("headline");
        root.Children.Add(title);
        root.Children.Add(buttons);
        root.Children.Add(sizes);
        root.Children.Add(inputs);
        root.Children.Add(toggles);
        root.Children.Add(progress);
        root.Children.Add(list);
        root.Children.Add(tabs);
        return root;
    }

    [AvaloniaTheory]
    [InlineData(ThemeMode.Light)]
    [InlineData(ThemeMode.Dark)]
    public void Built_in_controls_render(ThemeMode mode)
    {
        var path = Snapshot.Render(BuiltInControls(), "builtin", mode, 900, 760);
        Assert.True(File.Exists(path));
    }
}
