using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Lumen.UI.Controls;

namespace Lumen.Gallery.Pages;

public partial class FeedbackPage : UserControl
{
    public FeedbackPage() => InitializeComponent();

    private void OnToastInfo(object? sender, RoutedEventArgs e) =>
        Toast.Info("Sync started", "We'll let you know when it finishes.");

    private void OnToastSuccess(object? sender, RoutedEventArgs e) =>
        Toast.Success("Changes saved");

    private void OnToastWarning(object? sender, RoutedEventArgs e) =>
        Toast.Warning("Low battery", "Plug in to keep working.");

    private void OnToastError(object? sender, RoutedEventArgs e) =>
        Toast.Error("Upload failed", "The server returned 503. Try again in a moment.");

    private async void OnConfirm(object? sender, RoutedEventArgs e)
    {
        var ok = await Dialog.ConfirmAsync("Publish changes?", "Your teammates will see the update immediately.", "Publish", anchor: this);
        Toast.Show(ok ? "Published" : "Cancelled", severity: ok ? Severity.Success : Severity.Neutral, anchor: this);
    }

    private async void OnDelete(object? sender, RoutedEventArgs e)
    {
        var ok = await Dialog.ConfirmAsync(
            "Delete project?",
            "This permanently removes the project and all of its data. This action cannot be undone.",
            "Delete",
            isDanger: true,
            anchor: this);
        if (ok)
        {
            Toast.Error("Project deleted", anchor: this);
        }
    }

    private async void OnAlert(object? sender, RoutedEventArgs e) =>
        await Dialog.AlertAsync("All set", "Your workspace has been created.", anchor: this);

    private async void OnCustom(object? sender, RoutedEventArgs e)
    {
        var name = new TextBox { PlaceholderText = "My awesome project" };
        var create = new Button { Content = "Create", IsDefault = true };
        create.Classes.Add("primary");
        var cancel = new Button { Content = "Cancel", IsCancel = true };

        var title = new TextBlock { Text = "New project" };
        title.Classes.Add("title");
        var description = new TextBlock { Text = "Any control can be shown as a dialog.", TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        description.Classes.Add("secondary");

        var form = new StackPanel
        {
            Spacing = 14,
            Width = 380,
            Children =
            {
                title,
                description,
                new FormField { Label = "Project name", Content = name },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 6, 0, 0),
                    Children = { cancel, create },
                },
            },
        };

        create.Click += (_, _) => Dialog.Close(form, name.Text);
        cancel.Click += (_, _) => Dialog.Close(form);
        form.AttachedToVisualTree += (_, _) => name.Focus();

        var result = await Dialog.ShowAsync(form, this);
        if (result is string { Length: > 0 } projectName)
        {
            Toast.Success("Project created", projectName);
        }
    }
}
