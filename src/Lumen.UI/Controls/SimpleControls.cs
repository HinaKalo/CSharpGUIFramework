using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Lumen.UI.Icons;

namespace Lumen.UI.Controls;

/// <summary>Page / section title with optional description and right-aligned actions.</summary>
public class SectionHeader : TemplatedControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<SectionHeader, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<SectionHeader, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<SectionHeader, object?>(nameof(Actions));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }
}

/// <summary>Placeholder for "nothing here yet" states.</summary>
public class EmptyState : TemplatedControl
{
    public static readonly StyledProperty<IconKind> IconProperty =
        AvaloniaProperty.Register<EmptyState, IconKind>(nameof(Icon), IconKind.Inbox);

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<EmptyState, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<EmptyState, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionProperty =
        AvaloniaProperty.Register<EmptyState, object?>(nameof(Action));

    public IconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }
}

/// <summary>Pulsing loading placeholder. Classes: <c>circle</c>, <c>text</c>.</summary>
public class Skeleton : TemplatedControl
{
}

/// <summary>Indeterminate circular loading indicator.</summary>
public class Spinner : TemplatedControl
{
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<Spinner, double>(nameof(Size), 20d);

    public static readonly StyledProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<Spinner, double>(nameof(Thickness), 2.5d);

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public double Thickness
    {
        get => GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }
}

public enum Trend
{
    Neutral,
    Up,
    Down,
}

/// <summary>KPI tile: label, big value, optional trend delta and caption.</summary>
public class StatCard : TemplatedControl
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<StatCard, string?>(nameof(Label));

    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<StatCard, string?>(nameof(Value));

    public static readonly StyledProperty<string?> DeltaProperty =
        AvaloniaProperty.Register<StatCard, string?>(nameof(Delta));

    public static readonly StyledProperty<string?> CaptionProperty =
        AvaloniaProperty.Register<StatCard, string?>(nameof(Caption));

    public static readonly StyledProperty<Trend> TrendProperty =
        AvaloniaProperty.Register<StatCard, Trend>(nameof(Trend));

    public static readonly StyledProperty<IconKind> IconProperty =
        AvaloniaProperty.Register<StatCard, IconKind>(nameof(Icon));

    public static readonly DirectProperty<StatCard, IconKind> TrendIconProperty =
        AvaloniaProperty.RegisterDirect<StatCard, IconKind>(nameof(TrendIcon), o => o.TrendIcon);

    private IconKind _trendIcon = IconKind.Minus;

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? Delta
    {
        get => GetValue(DeltaProperty);
        set => SetValue(DeltaProperty, value);
    }

    public string? Caption
    {
        get => GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public Trend Trend
    {
        get => GetValue(TrendProperty);
        set => SetValue(TrendProperty, value);
    }

    public IconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IconKind TrendIcon
    {
        get => _trendIcon;
        private set => SetAndRaise(TrendIconProperty, ref _trendIcon, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TrendProperty)
        {
            PseudoClasses.Set(":up", Trend == Trend.Up);
            PseudoClasses.Set(":down", Trend == Trend.Down);
            TrendIcon = Trend switch
            {
                Trend.Up => IconKind.TrendingUp,
                Trend.Down => IconKind.TrendingDown,
                _ => IconKind.Minus,
            };
        }
        else if (change.Property == IconProperty)
        {
            PseudoClasses.Set(":has-icon", Icon != IconKind.None);
        }
    }
}
