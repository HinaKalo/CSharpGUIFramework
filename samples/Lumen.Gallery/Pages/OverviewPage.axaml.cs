using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Lumen.UI.Controls;

namespace Lumen.Gallery.Pages;

public partial class OverviewPage : UserControl
{
    public OverviewPage() => InitializeComponent();

    private void OnGetStarted(object? sender, RoutedEventArgs e) =>
        this.FindAncestorOfType<AppShell>()?.Navigate("buttons");

    private void OnToast(object? sender, RoutedEventArgs e) =>
        Toast.Success("Welcome to Lumen", "Toasts stack in the bottom-right corner and dismiss themselves.");
}
