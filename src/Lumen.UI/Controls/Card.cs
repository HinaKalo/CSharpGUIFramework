using Avalonia;
using Avalonia.Controls;

namespace Lumen.UI.Controls;

/// <summary>
/// Surface container with optional header (title + description + actions) and footer.
/// Classes: <c>interactive</c> (hover lift), <c>flat</c> (no shadow), <c>subtle</c> (tinted, borderless),
/// <c>compact</c> (smaller padding).
/// </summary>
public class Card : ContentControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Header));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Card, string?>(nameof(Description));

    public static readonly StyledProperty<object?> ActionsProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Actions));

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(Footer));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>Content shown at the top-right of the header (buttons, menus...).</summary>
    public object? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeaderProperty || change.Property == DescriptionProperty || change.Property == ActionsProperty)
        {
            PseudoClasses.Set(":has-header", Header is not null || !string.IsNullOrEmpty(Description) || Actions is not null);
        }
        else if (change.Property == FooterProperty)
        {
            PseudoClasses.Set(":has-footer", Footer is not null);
        }
    }
}
