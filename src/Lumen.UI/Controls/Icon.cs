using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Lumen.UI.Icons;

namespace Lumen.UI.Controls;

/// <summary>
/// A crisp stroke icon drawn from a 24×24 geometry. Color follows <see cref="Foreground"/>
/// (inherited from the parent text color by default).
/// <code>&lt;lumen:Icon Kind="Search" Size="16" /&gt;</code>
/// </summary>
public class Icon : Control
{
    public static readonly StyledProperty<IconKind> KindProperty =
        AvaloniaProperty.Register<Icon, IconKind>(nameof(Kind));

    /// <summary>Name of a custom icon registered via <see cref="LumenIcons.Register"/>.</summary>
    public static readonly StyledProperty<string?> NameKeyProperty =
        AvaloniaProperty.Register<Icon, string?>(nameof(NameKey));

    /// <summary>Explicit 24×24 geometry; takes precedence over <see cref="Kind"/>.</summary>
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<Icon, Geometry?>(nameof(Data));

    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<Icon, double>(nameof(Size), 16d);

    /// <summary>Stroke thickness in 24-unit grid space (scaled with the icon).</summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Icon, double>(nameof(StrokeThickness), 2d);

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<Icon>();

    /// <summary>Optional fill (e.g. for solid variants). Null = outline only.</summary>
    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<Icon, IBrush?>(nameof(Fill));

    static Icon()
    {
        AffectsRender<Icon>(KindProperty, NameKeyProperty, DataProperty, StrokeThicknessProperty, ForegroundProperty, FillProperty);
        AffectsMeasure<Icon>(SizeProperty);
    }

    public IconKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string? NameKey
    {
        get => GetValue(NameKeyProperty);
        set => SetValue(NameKeyProperty, value);
    }

    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    private Geometry? ResolveGeometry() =>
        Data ?? (NameKey is { Length: > 0 } name ? LumenIcons.Get(name) : LumenIcons.Get(Kind));

    protected override Size MeasureOverride(Size availableSize) => new(Size, Size);

    public override void Render(DrawingContext context)
    {
        var geometry = ResolveGeometry();
        if (geometry is null)
        {
            return;
        }

        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0)
        {
            return;
        }

        var scale = side / 24d;
        var offsetX = (Bounds.Width - side) / 2;
        var offsetY = (Bounds.Height - side) / 2;
        var pen = Foreground is { } brush
            ? new Pen(brush, StrokeThickness, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round)
            : null;

        using (context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(offsetX, offsetY)))
        {
            context.DrawGeometry(Fill, pen, geometry);
        }
    }
}
