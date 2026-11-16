using Avalonia;
using Avalonia.Headless;
using Lumen.UI;
using Lumen.UI.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApp))]

namespace Lumen.UI.Tests;

public sealed class TestApp : Application
{
    public override void Initialize() => Styles.Add(new LumenTheme());

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseSkia()
            .UseLumen()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
