using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Styling;
using Avalonia.Threading;
using Lumen.UI.Services;

namespace Lumen.UI.Tests;

/// <summary>
/// Renders controls headlessly to PNG for visual review.
/// Output folder: $LUMEN_SNAPSHOT_DIR, or "snapshots" next to the test binaries.
/// </summary>
internal static class Snapshot
{
    public static string OutputDirectory
    {
        get
        {
            var dir = Environment.GetEnvironmentVariable("LUMEN_SNAPSHOT_DIR");
            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(AppContext.BaseDirectory, "snapshots");
            }

            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public static string Render(Control content, string name, ThemeMode mode, double width = 900, double height = 700)
    {
        ThemeManager.SetMode(mode);
        var window = new Window
        {
            Width = width,
            Height = height,
            Content = content,
            RequestedThemeVariant = mode == ThemeMode.Dark ? ThemeVariant.Dark : ThemeVariant.Light,
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();

        var path = Path.Combine(OutputDirectory, $"{name}.{mode.ToString().ToLowerInvariant()}.png");
        using (var frame = window.CaptureRenderedFrame())
        {
            using var stream = File.Create(path);
#pragma warning disable CS0618 // quality-less overload is fine for PNG review output
            frame!.Save(stream);
#pragma warning restore CS0618
        }

        window.Close();
        return path;
    }
}
