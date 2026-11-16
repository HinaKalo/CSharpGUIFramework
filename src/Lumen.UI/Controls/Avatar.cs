using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Lumen.UI.Controls;

public enum PresenceStatus
{
    None,
    Online,
    Away,
    Busy,
    Offline,
}

/// <summary>
/// Circular avatar showing an image, or initials on a deterministic tinted background derived from
/// <see cref="DisplayName"/>. Classes: <c>square</c> for rounded-square shape.
/// </summary>
public class Avatar : TemplatedControl
{
    private static readonly Color[] s_palette =
    [
        Color.Parse("#6366F1"), Color.Parse("#0EA5E9"), Color.Parse("#14B8A6"), Color.Parse("#22C55E"),
        Color.Parse("#F59E0B"), Color.Parse("#F97316"), Color.Parse("#EF4444"), Color.Parse("#EC4899"),
        Color.Parse("#8B5CF6"), Color.Parse("#64748B"),
    ];

    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<Avatar, IImage?>(nameof(Source));

    public static readonly StyledProperty<string?> DisplayNameProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(DisplayName));

    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<Avatar, double>(nameof(Size), 36d);

    public static readonly StyledProperty<PresenceStatus> StatusProperty =
        AvaloniaProperty.Register<Avatar, PresenceStatus>(nameof(Status));

    public static readonly DirectProperty<Avatar, string> InitialsProperty =
        AvaloniaProperty.RegisterDirect<Avatar, string>(nameof(Initials), o => o.Initials);

    public static readonly DirectProperty<Avatar, IBrush?> FillProperty =
        AvaloniaProperty.RegisterDirect<Avatar, IBrush?>(nameof(Fill), o => o.Fill);

    public static readonly DirectProperty<Avatar, double> InitialsFontSizeProperty =
        AvaloniaProperty.RegisterDirect<Avatar, double>(nameof(InitialsFontSize), o => o.InitialsFontSize);

    private string _initials = string.Empty;
    private IBrush? _fill;
    private double _initialsFontSize = 14;

    public Avatar()
    {
        UpdateVisuals();
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string? DisplayName
    {
        get => GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public PresenceStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>Up to two initials derived from <see cref="DisplayName"/>.</summary>
    public string Initials
    {
        get => _initials;
        private set => SetAndRaise(InitialsProperty, ref _initials, value);
    }

    /// <summary>Background: the image (if any) or a tint chosen from the name.</summary>
    public IBrush? Fill
    {
        get => _fill;
        private set => SetAndRaise(FillProperty, ref _fill, value);
    }

    public double InitialsFontSize
    {
        get => _initialsFontSize;
        private set => SetAndRaise(InitialsFontSizeProperty, ref _initialsFontSize, value);
    }

    public static string GetInitials(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "?";
        }

        var parts = name.Split([' ', '.', '_', '-'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return "?";
        }

        var first = parts[0];
        if (parts.Length == 1)
        {
            // CJK names: use the last character(s); latin: first letter.
            return char.IsLetter(first[0]) && first[0] > 0x2E80
                ? first[^Math.Min(2, first.Length)..]
                : first[..1].ToUpperInvariant();
        }

        return string.Concat(char.ToUpperInvariant(parts[0][0]), char.ToUpperInvariant(parts[^1][0]));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty || change.Property == DisplayNameProperty || change.Property == SizeProperty)
        {
            UpdateVisuals();
        }
        else if (change.Property == StatusProperty)
        {
            PseudoClasses.Set(":online", Status == PresenceStatus.Online);
            PseudoClasses.Set(":away", Status == PresenceStatus.Away);
            PseudoClasses.Set(":busy", Status == PresenceStatus.Busy);
            PseudoClasses.Set(":offline", Status == PresenceStatus.Offline);
            PseudoClasses.Set(":has-status", Status != PresenceStatus.None);
        }
    }

    private void UpdateVisuals()
    {
        Initials = GetInitials(DisplayName);
        var isWide = Initials.Length > 1 && Initials[0] > 0x2E80;
        InitialsFontSize = Math.Max(9, Math.Round(Size * (isWide ? 0.3 : 0.4)));
        PseudoClasses.Set(":image", Source is IImageBrushSource);

        if (Source is IImageBrushSource image)
        {
            Fill = new ImageBrush(image) { Stretch = Stretch.UniformToFill };
            return;
        }

        var hash = 0;
        foreach (var ch in DisplayName ?? string.Empty)
        {
            hash = unchecked((hash * 31) + ch);
        }

        Fill = new SolidColorBrush(s_palette[Math.Abs(hash % s_palette.Length)]);
    }
}
