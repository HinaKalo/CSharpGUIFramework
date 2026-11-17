using Lumen.App.Views;
using Lumen.UI.Controls;
using Lumen.UI.Icons;

namespace Lumen.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>Sidebar navigation. Each page is created lazily and cached by the AppShell.</summary>
    public IReadOnlyList<NavItem> NavItems { get; } =
    [
        NavItem.Header("WORKSPACE"),
        new("Home", IconKind.Home, () => new HomePage { DataContext = new HomePageViewModel() }),
        NavItem.Header("SYSTEM"),
        new("Settings", IconKind.Settings, () => new SettingsPage { DataContext = new SettingsPageViewModel() }),
    ];
}
