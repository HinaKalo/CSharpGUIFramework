using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Lumen.UI.Controls;

/// <summary>A single toast notification card.</summary>
public class ToastCard : SeverityControl
{
    private Button? _closeButton;

    internal event EventHandler? CloseRequested;

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

    private void OnCloseClick(object? sender, RoutedEventArgs e) => CloseRequested?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Stack of toasts anchored to the bottom-right of its parent. Place one in your window root
/// (<see cref="AppShell"/> already includes one) and call <see cref="Toast"/> helpers anywhere.
/// </summary>
public class ToastHost : ItemsControl
{
    private static readonly List<WeakReference<ToastHost>> s_hosts = [];

    public static readonly StyledProperty<int> MaxItemsProperty =
        AvaloniaProperty.Register<ToastHost, int>(nameof(MaxItems), 4);

    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    /// <summary>The most recently attached host (used when no anchor is given).</summary>
    public static ToastHost? Current
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
    public static ToastHost? FindFor(Visual? anchor)
    {
        if (anchor is not null && TopLevel.GetTopLevel(anchor) is { } top)
        {
            return top.GetVisualDescendants().OfType<ToastHost>().FirstOrDefault() ?? Current;
        }

        return Current;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        lock (s_hosts)
        {
            s_hosts.RemoveAll(w => !w.TryGetTarget(out var h) || h == this);
            s_hosts.Add(new WeakReference<ToastHost>(this));
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

    /// <summary>Shows a toast. <paramref name="duration"/> null = 4 s; <see cref="TimeSpan.Zero"/> = sticky.</summary>
    public ToastCard Show(string title, string? message = null, Severity severity = Severity.Info, TimeSpan? duration = null)
    {
        var card = new ToastCard { Title = title, Message = message, Severity = severity };
        card.CloseRequested += (_, _) => Dismiss(card);
        card.Classes.Add("entering");
        Items.Add(card);
        DispatcherTimer.RunOnce(() => card.Classes.Remove("entering"), TimeSpan.FromMilliseconds(16));

        while (Items.Count > MaxItems && Items[0] is ToastCard oldest)
        {
            Items.Remove(oldest);
        }

        var lifetime = duration ?? TimeSpan.FromSeconds(4);
        if (lifetime > TimeSpan.Zero)
        {
            DispatcherTimer.RunOnce(() => Dismiss(card), lifetime);
        }

        return card;
    }

    /// <summary>Animates the toast out and removes it.</summary>
    public void Dismiss(ToastCard card)
    {
        if (!Items.Contains(card) || card.Classes.Contains("leaving"))
        {
            return;
        }

        card.Classes.Add("leaving");
        DispatcherTimer.RunOnce(() => Items.Remove(card), TimeSpan.FromMilliseconds(220));
    }
}

/// <summary>Static helpers for showing toasts on the current <see cref="ToastHost"/>.</summary>
public static class Toast
{
    public static ToastCard? Show(string title, string? message = null, Severity severity = Severity.Info, TimeSpan? duration = null, Visual? anchor = null) =>
        ToastHost.FindFor(anchor)?.Show(title, message, severity, duration);

    public static ToastCard? Info(string title, string? message = null, Visual? anchor = null) =>
        Show(title, message, Severity.Info, anchor: anchor);

    public static ToastCard? Success(string title, string? message = null, Visual? anchor = null) =>
        Show(title, message, Severity.Success, anchor: anchor);

    public static ToastCard? Warning(string title, string? message = null, Visual? anchor = null) =>
        Show(title, message, Severity.Warning, anchor: anchor);

    public static ToastCard? Error(string title, string? message = null, Visual? anchor = null) =>
        Show(title, message, Severity.Danger, anchor: anchor);
}
