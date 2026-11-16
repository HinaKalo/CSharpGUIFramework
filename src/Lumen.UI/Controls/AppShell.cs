using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Lumen.UI.Icons;

namespace Lumen.UI.Controls;

/// <summary>A navigation entry for <see cref="AppShell"/>.</summary>
public class NavItem
{
    public NavItem()
    {
    }

    public NavItem(string title, IconKind icon, Func<object>? pageFactory = null)
    {
        Title = title;
        Icon = icon;
        PageFactory = pageFactory;
    }

    public string Title { get; set; } = string.Empty;

    public IconKind Icon { get; set; } = IconKind.Circle;

    /// <summary>Optional count / label shown on the right (e.g. "3", "New").</summary>
    public string? Badge { get; set; }

    /// <summary>Creates the page shown when this item is selected (created once, then cached).</summary>
    public Func<object>? PageFactory { get; set; }

    /// <summary>Arbitrary user data (route key, view-model...).</summary>
    public object? Tag { get; set; }

    /// <summary>When true the item renders as a non-selectable group label.</summary>
    public bool IsHeader { get; set; }

    public bool HasBadge => !string.IsNullOrEmpty(Badge);

    internal object? CachedPage { get; set; }

    /// <summary>Creates a group label.</summary>
    public static NavItem Header(string title) => new() { Title = title, IsHeader = true, Icon = IconKind.None };

    public object? GetOrCreatePage() => CachedPage ??= PageFactory?.Invoke();

    public override string ToString() => Title;
}

/// <summary>Sidebar navigation list used by <see cref="AppShell"/>; header items are not selectable.</summary>
public class NavList : ListBox
{
    /// <summary>Icon-only mode used when the shell pane is collapsed.</summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<NavList, bool>(nameof(IsCompact));

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(NavList);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        var isHeader = item is NavItem { IsHeader: true };
        container.Classes.Set("header", isHeader);
        container.Classes.Set("compact", IsCompact);
        container.IsEnabled = !isHeader;
        container.Focusable = !isHeader;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsCompactProperty)
        {
            foreach (var container in GetRealizedContainers())
            {
                container.Classes.Set("compact", IsCompact);
            }
        }
    }
}

/// <summary>
/// Application chrome: collapsible sidebar navigation, top bar with page title and actions,
/// animated page host, plus built-in <see cref="DialogHost"/> and <see cref="ToastHost"/>.
/// When <see cref="ContentControl.Content"/> is not set, the selected <see cref="NavItem"/>'s page is shown.
/// </summary>
public class AppShell : ContentControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<AppShell, string?>(nameof(Title));

    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<AppShell, string?>(nameof(Subtitle));

    public static readonly StyledProperty<object?> LogoProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(Logo));

    public static readonly StyledProperty<IEnumerable?> NavItemsProperty =
        AvaloniaProperty.Register<AppShell, IEnumerable?>(nameof(NavItems));

    public static readonly StyledProperty<NavItem?> SelectedNavItemProperty =
        AvaloniaProperty.Register<AppShell, NavItem?>(nameof(SelectedNavItem), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<object?> PaneFooterProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(PaneFooter));

    public static readonly StyledProperty<object?> HeaderActionsProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(HeaderActions));

    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(IsPaneOpen), true);

    public static readonly StyledProperty<double> PaneWidthProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(PaneWidth), 248d);

    public static readonly StyledProperty<bool> ShowPageHeaderProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(ShowPageHeader), true);

    public static readonly DirectProperty<AppShell, object?> CurrentPageProperty =
        AvaloniaProperty.RegisterDirect<AppShell, object?>(nameof(CurrentPage), o => o.CurrentPage);

    public static readonly DirectProperty<AppShell, string?> PageTitleProperty =
        AvaloniaProperty.RegisterDirect<AppShell, string?>(nameof(PageTitle), o => o.PageTitle);

    private object? _currentPage;
    private string? _pageTitle;
    private Button? _paneToggle;

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>Content shown left of the title (an Icon, Image, initials...).</summary>
    public object? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }

    public IEnumerable? NavItems
    {
        get => GetValue(NavItemsProperty);
        set => SetValue(NavItemsProperty, value);
    }

    public NavItem? SelectedNavItem
    {
        get => GetValue(SelectedNavItemProperty);
        set => SetValue(SelectedNavItemProperty, value);
    }

    public object? PaneFooter
    {
        get => GetValue(PaneFooterProperty);
        set => SetValue(PaneFooterProperty, value);
    }

    /// <summary>Content on the right of the top bar (search, theme toggle, avatar...).</summary>
    public object? HeaderActions
    {
        get => GetValue(HeaderActionsProperty);
        set => SetValue(HeaderActionsProperty, value);
    }

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public double PaneWidth
    {
        get => GetValue(PaneWidthProperty);
        set => SetValue(PaneWidthProperty, value);
    }

    public bool ShowPageHeader
    {
        get => GetValue(ShowPageHeaderProperty);
        set => SetValue(ShowPageHeaderProperty, value);
    }

    /// <summary>The page currently displayed (explicit Content, or the selected item's page).</summary>
    public object? CurrentPage
    {
        get => _currentPage;
        private set => SetAndRaise(CurrentPageProperty, ref _currentPage, value);
    }

    public string? PageTitle
    {
        get => _pageTitle;
        private set => SetAndRaise(PageTitleProperty, ref _pageTitle, value);
    }

    /// <summary>Selects the first nav item whose <see cref="NavItem.Title"/> or <see cref="NavItem.Tag"/> matches.</summary>
    public bool Navigate(object key)
    {
        var item = NavItems?.OfType<NavItem>().FirstOrDefault(n =>
            !n.IsHeader && (Equals(n.Tag, key) || string.Equals(n.Title, key as string, StringComparison.OrdinalIgnoreCase)));
        if (item is null)
        {
            return false;
        }

        SelectedNavItem = item;
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_paneToggle is not null)
        {
            _paneToggle.Click -= OnPaneToggleClick;
        }

        _paneToggle = e.NameScope.Find<Button>("PART_PaneToggle");
        if (_paneToggle is not null)
        {
            _paneToggle.Click += OnPaneToggleClick;
        }

        if (SelectedNavItem is null)
        {
            SelectedNavItem = NavItems?.OfType<NavItem>().FirstOrDefault(n => !n.IsHeader);
        }

        UpdateCurrentPage();
        PseudoClasses.Set(":collapsed", !IsPaneOpen);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedNavItemProperty)
        {
            if (SelectedNavItem is { IsHeader: true })
            {
                SelectedNavItem = change.GetOldValue<NavItem?>();
                return;
            }

            UpdateCurrentPage();
        }
        else if (change.Property == ContentProperty)
        {
            UpdateCurrentPage();
        }
        else if (change.Property == IsPaneOpenProperty)
        {
            PseudoClasses.Set(":collapsed", !IsPaneOpen);
        }
        else if (change.Property == NavItemsProperty && SelectedNavItem is null && IsInitialized)
        {
            SelectedNavItem = NavItems?.OfType<NavItem>().FirstOrDefault(n => !n.IsHeader);
        }
    }

    private void UpdateCurrentPage()
    {
        PageTitle = SelectedNavItem?.Title;
        CurrentPage = Content ?? SelectedNavItem?.GetOrCreatePage();
    }

    private void OnPaneToggleClick(object? sender, RoutedEventArgs e) => IsPaneOpen = !IsPaneOpen;
}
