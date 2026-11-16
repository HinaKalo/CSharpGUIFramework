using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Lumen.UI.Icons;
using Lumen.UI.Services;

namespace Lumen.UI.Controls;

/// <summary>
/// Labelled form row: label (+ required marker), the input as content, and a hint or error line.
/// <code>&lt;lumen:FormField Label="Email" Hint="We never share it"&gt;&lt;TextBox /&gt;&lt;/lumen:FormField&gt;</code>
/// </summary>
public class FormField : ContentControl
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(Label));

    public static readonly StyledProperty<string?> HintProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(Hint));

    public static readonly StyledProperty<string?> ErrorProperty =
        AvaloniaProperty.Register<FormField, string?>(nameof(Error));

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(IsRequired));

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Hint
    {
        get => GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }

    /// <summary>Validation message; when set it replaces the hint and turns the field red.</summary>
    public string? Error
    {
        get => GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ErrorProperty || change.Property == HintProperty)
        {
            var hasError = !string.IsNullOrEmpty(Error);
            PseudoClasses.Set(":error", hasError);
            PseudoClasses.Set(":has-hint", !hasError && !string.IsNullOrEmpty(Hint));
        }
        else if (change.Property == IsRequiredProperty)
        {
            PseudoClasses.Set(":required", IsRequired);
        }
    }
}

/// <summary>TextBox with a leading search icon, a clear button and Esc-to-clear.</summary>
public class SearchBox : TextBox
{
    private readonly Button _clearButton;

    public SearchBox()
    {
        PlaceholderText = "Search";
        InnerLeftContent = new Icon
        {
            Kind = IconKind.Search,
            Size = 15,
            Margin = new Thickness(10, 0, 0, 0),
            [!Icon.ForegroundProperty] = this.GetResourceObservable("Lumen.Brush.TextTertiary").ToBinding(),
        };

        _clearButton = new Button
        {
            Content = new Icon { Kind = IconKind.X, Size = 14 },
            Margin = new Thickness(0, 0, 4, 0),
            Width = 24,
            Height = 24,
            MinHeight = 0,
            Padding = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false,
            Focusable = false,
        };
        _clearButton.Classes.Add("ghost");
        _clearButton.Classes.Add("icon");
        _clearButton.Classes.Add("sm");
        _clearButton.Click += (_, _) => Clear();
        InnerRightContent = _clearButton;
    }

    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty)
        {
            _clearButton.IsVisible = !string.IsNullOrEmpty(Text);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && !string.IsNullOrEmpty(Text))
        {
            Clear();
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }
}

/// <summary>iOS / shadcn style segmented control. Behaves like a single-selection ListBox.</summary>
public class SegmentedControl : ListBox
{
    protected override Type StyleKeyOverride => typeof(SegmentedControl);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new SegmentedItem();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<SegmentedItem>(item, out recycleKey);
}

/// <summary>Item container for <see cref="SegmentedControl"/>.</summary>
public class SegmentedItem : ListBoxItem
{
    protected override Type StyleKeyOverride => typeof(SegmentedItem);
}

/// <summary>Icon button that flips light / dark via <see cref="ThemeManager"/>.</summary>
public class ThemeToggleButton : Button
{
    private readonly Icon _icon = new() { Size = 16 };

    public ThemeToggleButton()
    {
        Content = _icon;
        Classes.Add("ghost");
        Classes.Add("icon");
        ToolTip.SetTip(this, "Toggle theme");
        Click += (_, _) => ThemeManager.Toggle();
    }

    protected override Type StyleKeyOverride => typeof(Button);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateIcon();
        ActualThemeVariantChanged += OnThemeChanged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ActualThemeVariantChanged -= OnThemeChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnThemeChanged(object? sender, EventArgs e) => UpdateIcon();

    private void UpdateIcon() =>
        _icon.Kind = ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark ? IconKind.Sun : IconKind.Moon;
}
