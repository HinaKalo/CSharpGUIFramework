using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Lumen.UI.Icons;

namespace Lumen.UI.Controls;

public enum Severity
{
    Neutral,
    Info,
    Success,
    Warning,
    Danger,
}

internal static class SeverityExtensions
{
    public static IconKind ToIcon(this Severity severity) => severity switch
    {
        Severity.Success => IconKind.CircleCheck,
        Severity.Warning => IconKind.AlertTriangle,
        Severity.Danger => IconKind.AlertCircle,
        Severity.Info => IconKind.Info,
        _ => IconKind.Bell,
    };

    public static void ApplyPseudoClasses(this Severity severity, IPseudoClasses classes)
    {
        classes.Set(":info", severity == Severity.Info);
        classes.Set(":success", severity == Severity.Success);
        classes.Set(":warning", severity == Severity.Warning);
        classes.Set(":danger", severity == Severity.Danger);
        classes.Set(":neutral", severity == Severity.Neutral);
    }
}

/// <summary>Base for controls that render a severity-colored message with an icon.</summary>
public abstract class SeverityControl : TemplatedControl
{
    public static readonly StyledProperty<Severity> SeverityProperty =
        AvaloniaProperty.Register<SeverityControl, Severity>(nameof(Severity), Severity.Info);

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<SeverityControl, string?>(nameof(Title));

    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<SeverityControl, string?>(nameof(Message));

    public static readonly DirectProperty<SeverityControl, IconKind> IconProperty =
        AvaloniaProperty.RegisterDirect<SeverityControl, IconKind>(nameof(Icon), o => o.Icon);

    private IconKind _icon = IconKind.Info;

    protected SeverityControl()
    {
        Severity.ApplyPseudoClasses(PseudoClasses);
    }

    public Severity Severity
    {
        get => GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public IconKind Icon
    {
        get => _icon;
        private set => SetAndRaise(IconProperty, ref _icon, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SeverityProperty)
        {
            Icon = Severity.ToIcon();
            Severity.ApplyPseudoClasses(PseudoClasses);
        }
    }
}

/// <summary>Inline alert / banner. Set <see cref="IsClosable"/> to show a close button.</summary>
public class InfoBar : SeverityControl
{
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsClosable));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<InfoBar, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<InfoBar, object?>(nameof(Actions));

    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<InfoBar, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    private Button? _closeButton;

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_closeButton is not null)
        {
            _closeButton.Click -= OnCloseClick;
        }

        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        if (_closeButton is not null)
        {
            _closeButton.Click += OnCloseClick;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsOpenProperty)
        {
            IsVisible = IsOpen;
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        IsOpen = false;
        RaiseEvent(new RoutedEventArgs(ClosedEvent));
    }
}
