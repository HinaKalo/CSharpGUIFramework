using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Lumen.UI.Controls;

/// <summary>
/// Hosts modal in-window dialogs over its content (scrim + centered card).
/// <see cref="AppShell"/> already includes one; otherwise wrap your window content with it.
/// </summary>
public class DialogHost : ContentControl
{
    private static readonly List<WeakReference<DialogHost>> s_hosts = [];

    public static readonly StyledProperty<object?> DialogContentProperty =
        AvaloniaProperty.Register<DialogHost, object?>(nameof(DialogContent));

    public static readonly StyledProperty<bool> IsDialogOpenProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(IsDialogOpen));

    public static readonly StyledProperty<bool> CloseOnClickAwayProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(CloseOnClickAway), true);

    public static readonly StyledProperty<double> DialogMaxWidthProperty =
        AvaloniaProperty.Register<DialogHost, double>(nameof(DialogMaxWidth), 460d);

    private TaskCompletionSource<object?>? _pending;
    private Border? _overlay;
    private IInputElement? _restoreFocus;

    public object? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    public bool IsDialogOpen
    {
        get => GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    public bool CloseOnClickAway
    {
        get => GetValue(CloseOnClickAwayProperty);
        set => SetValue(CloseOnClickAwayProperty, value);
    }

    public double DialogMaxWidth
    {
        get => GetValue(DialogMaxWidthProperty);
        set => SetValue(DialogMaxWidthProperty, value);
    }

    /// <summary>The most recently attached host.</summary>
    public static DialogHost? Current
    {
        get
        {
            lock (s_hosts)
            {
                for (var i = s_hosts.Count - 1; i >= 0; i--)
                {
                    if (s_hosts[i].TryGetTarget(out var host))
                    {
                        return host;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>Finds the host in the same window as <paramref name="anchor"/>.</summary>
    public static DialogHost? FindFor(Visual? anchor)
    {
        if (anchor is not null)
        {
            var ancestor = anchor.FindAncestorOfType<DialogHost>(includeSelf: true);
            if (ancestor is not null)
            {
                return ancestor;
            }

            if (TopLevel.GetTopLevel(anchor) is { } top)
            {
                return top.GetVisualDescendants().OfType<DialogHost>().FirstOrDefault() ?? Current;
            }
        }

        return Current;
    }

    /// <summary>Opens <paramref name="content"/> as a modal dialog; completes when <see cref="Close"/> is called.</summary>
    public Task<object?> ShowAsync(object content)
    {
        _pending?.TrySetResult(null);
        _pending = new TaskCompletionSource<object?>();
        _restoreFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
        DialogContent = content;
        IsDialogOpen = true;
        return _pending.Task;
    }

    /// <summary>Closes the open dialog with an optional result.</summary>
    public void Close(object? result = null)
    {
        if (!IsDialogOpen)
        {
            return;
        }

        IsDialogOpen = false;
        DialogContent = null;
        var pending = _pending;
        _pending = null;
        pending?.TrySetResult(result);
        _restoreFocus?.Focus();
        _restoreFocus = null;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        lock (s_hosts)
        {
            s_hosts.RemoveAll(w => !w.TryGetTarget(out var h) || h == this);
            s_hosts.Add(new WeakReference<DialogHost>(this));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        lock (s_hosts)
        {
            s_hosts.RemoveAll(w => !w.TryGetTarget(out var h) || h == this);
        }

        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_overlay is not null)
        {
            _overlay.PointerPressed -= OnOverlayPressed;
        }

        _overlay = e.NameScope.Find<Border>("PART_Overlay");
        if (_overlay is not null)
        {
            _overlay.PointerPressed += OnOverlayPressed;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsDialogOpenProperty)
        {
            PseudoClasses.Set(":open", IsDialogOpen);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsDialogOpen && e.Key == Key.Escape)
        {
            Close();
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private void OnOverlayPressed(object? sender, PointerPressedEventArgs e)
    {
        if (CloseOnClickAway && ReferenceEquals(e.Source, _overlay))
        {
            Close();
            e.Handled = true;
        }
    }
}

/// <summary>Ready-made dialogs on top of <see cref="DialogHost"/>.</summary>
public static class Dialog
{
    /// <summary>Shows arbitrary content and returns the value passed to <see cref="DialogHost.Close"/>.</summary>
    public static Task<object?> ShowAsync(object content, Visual? anchor = null) =>
        DialogHost.FindFor(anchor)?.ShowAsync(content) ?? Task.FromResult<object?>(null);

    /// <summary>Closes the dialog that contains <paramref name="element"/>.</summary>
    public static void Close(Visual element, object? result = null) =>
        DialogHost.FindFor(element)?.Close(result);

    /// <summary>Confirm / cancel dialog. Returns true when confirmed.</summary>
    public static async Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText = "Confirm",
        string cancelText = "Cancel",
        bool isDanger = false,
        Visual? anchor = null)
    {
        var host = DialogHost.FindFor(anchor);
        if (host is null)
        {
            return false;
        }

        var result = await host.ShowAsync(BuildMessage(host, title, message, confirmText, cancelText, isDanger));
        return result is true;
    }

    /// <summary>Single-button information dialog.</summary>
    public static async Task AlertAsync(string title, string message, string okText = "OK", Visual? anchor = null)
    {
        var host = DialogHost.FindFor(anchor);
        if (host is null)
        {
            return;
        }

        await host.ShowAsync(BuildMessage(host, title, message, okText, null, false));
    }

    private static Control BuildMessage(DialogHost host, string title, string message, string confirmText, string? cancelText, bool isDanger)
    {
        var titleBlock = new TextBlock { Text = title, TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        titleBlock.Classes.Add("title");

        var messageBlock = new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        messageBlock.Classes.Add("body");
        messageBlock.Classes.Add("secondary");

        var confirm = new Button { Content = confirmText, IsDefault = true, MinWidth = 88 };
        confirm.Classes.Add(isDanger ? "danger" : "primary");
        confirm.Click += (_, _) => host.Close(true);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 12, 0, 0),
        };

        if (cancelText is not null)
        {
            var cancel = new Button { Content = cancelText, IsCancel = true, MinWidth = 88 };
            cancel.Click += (_, _) => host.Close(false);
            buttons.Children.Add(cancel);
        }

        buttons.Children.Add(confirm);

        var panel = new StackPanel { Spacing = 8, Width = 380 };
        panel.Children.Add(titleBlock);
        panel.Children.Add(messageBlock);
        panel.Children.Add(buttons);
        panel.AttachedToVisualTree += (_, _) => confirm.Focus();
        return panel;
    }
}
