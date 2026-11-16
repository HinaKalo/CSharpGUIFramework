using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Lumen.Gallery;
using Lumen.UI.Controls;
using Lumen.UI.Services;
using Xunit;

namespace Lumen.UI.Tests;

/// <summary>Renders every gallery page (light + dark) — doubles as a smoke test for all controls.</summary>
public class GallerySnapshotTests
{
    [AvaloniaTheory]
    [InlineData(ThemeMode.Light)]
    [InlineData(ThemeMode.Dark)]
    public void Every_gallery_page_renders(ThemeMode mode)
    {
        ThemeManager.SetMode(mode);
        var window = new MainWindow
        {
            RequestedThemeVariant = mode == ThemeMode.Dark ? ThemeVariant.Dark : ThemeVariant.Light,
        };
        window.Show();
        var shell = Assert.IsType<AppShell>(window.Content);
        var pages = shell.NavItems!.OfType<NavItem>().Where(n => !n.IsHeader).ToList();
        Assert.NotEmpty(pages);

        foreach (var item in pages)
        {
            shell.SelectedNavItem = item;
            Settle();
            Assert.NotNull(shell.CurrentPage);

            var path = Path.Combine(Snapshot.OutputDirectory, $"gallery-{item.Tag}.{mode.ToString().ToLowerInvariant()}.png");
            using var frame = window.CaptureRenderedFrame();
            using var stream = File.Create(path);
#pragma warning disable CS0618
            frame!.Save(stream);
#pragma warning restore CS0618
        }

        shell.IsPaneOpen = false;
        Settle();
        using (var collapsed = window.CaptureRenderedFrame())
        using (var stream = File.Create(Path.Combine(Snapshot.OutputDirectory, $"gallery-collapsed.{mode.ToString().ToLowerInvariant()}.png")))
        {
#pragma warning disable CS0618
            collapsed!.Save(stream);
#pragma warning restore CS0618
        }

        window.Close();
        ThemeManager.SetMode(ThemeMode.Light);
    }

    [AvaloniaFact]
    public async Task Dialog_and_toast_render_over_the_shell()
    {
        ThemeManager.SetMode(ThemeMode.Light);
        var window = new MainWindow { RequestedThemeVariant = ThemeVariant.Light };
        window.Show();
        var shell = Assert.IsType<AppShell>(window.Content);
        Settle();

        Toast.Success("Changes saved", "Your profile was updated.", anchor: shell);
        Toast.Error("Upload failed", "The server returned 503.", anchor: shell);
        var confirm = Dialog.ConfirmAsync("Delete project?", "This cannot be undone.", "Delete", isDanger: true, anchor: shell);
        Settle();
        Assert.False(confirm.IsCompleted);

        using (var frame = window.CaptureRenderedFrame())
        using (var stream = File.Create(Path.Combine(Snapshot.OutputDirectory, "gallery-overlays.light.png")))
        {
#pragma warning disable CS0618
            frame!.Save(stream);
#pragma warning restore CS0618
        }

        DialogHost.FindFor(shell)!.Close(true);
        Assert.True(await confirm);
        window.Close();
    }

    private static void Settle()
    {
        for (var i = 0; i < 40; i++)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }
    }
}
