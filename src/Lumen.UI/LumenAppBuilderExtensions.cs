using Avalonia;

namespace Lumen.UI;

public static class LumenAppBuilderExtensions
{
    /// <summary>
    /// Registers everything Lumen needs at the platform level (currently the bundled Inter font).
    /// Call it on the <see cref="AppBuilder"/> in <c>Program.BuildAvaloniaApp</c>.
    /// </summary>
    public static AppBuilder UseLumen(this AppBuilder builder) => builder.WithInterFont();
}
