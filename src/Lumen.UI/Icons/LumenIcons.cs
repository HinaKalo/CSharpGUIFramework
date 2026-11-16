using System.Collections.Concurrent;
using System.Globalization;
using Avalonia.Media;

namespace Lumen.UI.Icons;

/// <summary>
/// Geometry registry for <see cref="IconKind"/>. All icons live on a 24×24 grid and are meant to be
/// stroked (not filled) with round caps/joins. Shapes follow the Lucide / Feather icon sets (ISC / MIT).
/// Register your own with <see cref="Register"/>.
/// </summary>
public static class LumenIcons
{
    private static readonly ConcurrentDictionary<IconKind, Geometry> s_cache = new();
    private static readonly ConcurrentDictionary<string, Geometry> s_custom = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Path data (SVG mini-language, absolute starts) for every built-in icon.</summary>
    public static IReadOnlyDictionary<IconKind, string> PathData { get; } = new Dictionary<IconKind, string>
    {
        [IconKind.None] = string.Empty,
        [IconKind.Activity] = "M22 12 H18 L15 21 L9 3 L6 12 H2",
        [IconKind.AlertCircle] = C(12, 12, 10) + "M12 8 V12 M12 16 H12.01",
        [IconKind.AlertTriangle] = "M10.29 3.86 L1.82 18 A2 2 0 0 0 3.53 21 H20.47 A2 2 0 0 0 22.18 18 L13.71 3.86 A2 2 0 0 0 10.29 3.86 Z M12 9 V13 M12 17 H12.01",
        [IconKind.ArrowLeft] = "M19 12 H5 M12 19 L5 12 L12 5",
        [IconKind.ArrowRight] = "M5 12 H19 M12 5 L19 12 L12 19",
        [IconKind.BarChart] = "M18 20 V10 M12 20 V4 M6 20 V14",
        [IconKind.Bell] = "M6 8 A6 6 0 0 1 18 8 C18 15 21 17 21 17 H3 C3 17 6 15 6 8 Z M10.3 21 A1.94 1.94 0 0 0 13.7 21",
        [IconKind.Bookmark] = "M19 21 L12 16 L5 21 V5 A2 2 0 0 1 7 3 H17 A2 2 0 0 1 19 5 Z",
        [IconKind.Calendar] = R(3, 4, 18, 18, 2) + "M16 2 V6 M8 2 V6 M3 10 H21",
        [IconKind.Check] = "M20 6 L9 17 L4 12",
        [IconKind.ChevronDown] = "M6 9 L12 15 L18 9",
        [IconKind.ChevronLeft] = "M15 18 L9 12 L15 6",
        [IconKind.ChevronRight] = "M9 18 L15 12 L9 6",
        [IconKind.ChevronUp] = "M18 15 L12 9 L6 15",
        [IconKind.Circle] = C(12, 12, 10),
        [IconKind.CircleCheck] = C(12, 12, 10) + "M9 12 L11 14 L15 10",
        [IconKind.CircleX] = C(12, 12, 10) + "M15 9 L9 15 M9 9 L15 15",
        [IconKind.Clock] = C(12, 12, 10) + "M12 6 V12 L16 14",
        [IconKind.Copy] = R(9, 9, 13, 13, 2) + "M5 15 H4 A2 2 0 0 1 2 13 V4 A2 2 0 0 1 4 2 H13 A2 2 0 0 1 15 4 V5",
        [IconKind.CreditCard] = R(1, 4, 22, 16, 2) + "M1 10 H23",
        [IconKind.Download] = "M21 15 V19 A2 2 0 0 1 19 21 H5 A2 2 0 0 1 3 19 V15 M7 10 L12 15 L17 10 M12 15 V3",
        [IconKind.Droplet] = "M12 2.69 L17.66 8.35 A8 8 0 1 1 6.34 8.35 Z",
        [IconKind.Eye] = "M1 12 C1 12 5 4 12 4 C19 4 23 12 23 12 C23 12 19 20 12 20 C5 20 1 12 1 12 Z " + C(12, 12, 3),
        [IconKind.ExternalLink] = "M18 13 V19 A2 2 0 0 1 16 21 H5 A2 2 0 0 1 3 19 V8 A2 2 0 0 1 5 6 H11 M15 3 H21 V9 M10 14 L21 3",
        [IconKind.FileText] = "M14 2 H6 A2 2 0 0 0 4 4 V20 A2 2 0 0 0 6 22 H18 A2 2 0 0 0 20 20 V8 Z M14 2 V8 H20 M16 13 H8 M16 17 H8 M10 9 H8",
        [IconKind.Filter] = "M22 3 H2 L10 12.46 V19 L14 21 V12.46 Z",
        [IconKind.Folder] = "M22 19 A2 2 0 0 1 20 21 H4 A2 2 0 0 1 2 19 V5 A2 2 0 0 1 4 3 H9 L11 6 H20 A2 2 0 0 1 22 8 Z",
        [IconKind.Globe] = C(12, 12, 10) + "M2 12 H22 M12 2 A15.3 15.3 0 0 1 16 12 A15.3 15.3 0 0 1 12 22 A15.3 15.3 0 0 1 8 12 A15.3 15.3 0 0 1 12 2 Z",
        [IconKind.Grid] = R(3, 3, 7, 7, 1) + R(14, 3, 7, 7, 1) + R(14, 14, 7, 7, 1) + R(3, 14, 7, 7, 1),
        [IconKind.Heart] = "M20.84 4.61 A5.5 5.5 0 0 0 13.06 4.61 L12 5.67 L10.94 4.61 A5.5 5.5 0 0 0 3.16 12.39 L4.22 13.45 L12 21.23 L19.78 13.45 L20.84 12.39 A5.5 5.5 0 0 0 20.84 4.61 Z",
        [IconKind.Home] = "M3 10 L12 3 L21 10 V20 A1 1 0 0 1 20 21 H15 V14 H9 V21 H4 A1 1 0 0 1 3 20 Z",
        [IconKind.Inbox] = "M22 12 H16 L14 15 H10 L8 12 H2 M5.45 5.11 L2 12 V18 A2 2 0 0 0 4 20 H20 A2 2 0 0 0 22 18 V12 L18.55 5.11 A2 2 0 0 0 16.76 4 H7.24 A2 2 0 0 0 5.45 5.11 Z",
        [IconKind.Info] = C(12, 12, 10) + "M12 16 V12 M12 8 H12.01",
        [IconKind.Layers] = "M12 2 L2 7 L12 12 L22 7 Z M2 17 L12 22 L22 17 M2 12 L12 17 L22 12",
        [IconKind.LayoutDashboard] = R(3, 3, 7, 9, 1) + R(14, 3, 7, 5, 1) + R(14, 12, 7, 9, 1) + R(3, 16, 7, 5, 1),
        [IconKind.Lock] = R(3, 11, 18, 11, 2) + "M7 11 V7 A5 5 0 0 1 17 7 V11",
        [IconKind.LogOut] = "M9 21 H5 A2 2 0 0 1 3 19 V5 A2 2 0 0 1 5 3 H9 M16 17 L21 12 L16 7 M21 12 H9",
        [IconKind.Mail] = R(2, 4, 20, 16, 2) + "M22 7 L13.03 12.7 A1.94 1.94 0 0 1 10.97 12.7 L2 7",
        [IconKind.Menu] = "M4 6 H20 M4 12 H20 M4 18 H20",
        [IconKind.MessageSquare] = "M21 15 A2 2 0 0 1 19 17 H7 L3 21 V5 A2 2 0 0 1 5 3 H19 A2 2 0 0 1 21 5 Z",
        [IconKind.Minus] = "M5 12 H19",
        [IconKind.Moon] = "M12 3 A6 6 0 0 0 21 12 A9 9 0 1 1 12 3 Z",
        [IconKind.MoreHorizontal] = C(12, 12, 1) + C(19, 12, 1) + C(5, 12, 1),
        [IconKind.MousePointer] = "M3 3 L10.07 19.97 L12.58 12.58 L19.97 10.07 Z M13 13 L19 19",
        [IconKind.Package] = "M21 16 V8 A2 2 0 0 0 20 6.27 L13 2.27 A2 2 0 0 0 11 2.27 L4 6.27 A2 2 0 0 0 3 8 V16 A2 2 0 0 0 4 17.73 L11 21.73 A2 2 0 0 0 13 21.73 L20 17.73 A2 2 0 0 0 21 16 Z M3.27 6.96 L12 12.01 L20.73 6.96 M12 22.08 V12",
        [IconKind.PanelLeft] = R(3, 3, 18, 18, 2) + "M9 3 V21",
        [IconKind.Pencil] = "M17 3 A2.828 2.828 0 1 1 21 7 L7.5 20.5 L2 22 L3.5 16.5 Z",
        [IconKind.Plus] = "M5 12 H19 M12 5 V19",
        [IconKind.RefreshCw] = "M3 12 A9 9 0 0 1 12 3 A9.75 9.75 0 0 1 18.74 5.74 L21 8 M21 3 V8 H16 M21 12 A9 9 0 0 1 12 21 A9.75 9.75 0 0 1 5.26 18.26 L3 16 M8 16 H3 V21",
        [IconKind.Search] = C(11, 11, 8) + "M21 21 L16.65 16.65",
        [IconKind.Settings] = "M12.22 2 h-0.44 a2 2 0 0 0 -2 2 v0.18 a2 2 0 0 1 -1 1.73 l-0.43 0.25 a2 2 0 0 1 -2 0 l-0.15 -0.08 a2 2 0 0 0 -2.73 0.73 l-0.22 0.38 a2 2 0 0 0 0.73 2.73 l0.15 0.1 a2 2 0 0 1 1 1.72 v0.51 a2 2 0 0 1 -1 1.74 l-0.15 0.09 a2 2 0 0 0 -0.73 2.73 l0.22 0.38 a2 2 0 0 0 2.73 0.73 l0.15 -0.08 a2 2 0 0 1 2 0 l0.43 0.25 a2 2 0 0 1 1 1.73 V20 a2 2 0 0 0 2 2 h0.44 a2 2 0 0 0 2 -2 v-0.18 a2 2 0 0 1 1 -1.73 l0.43 -0.25 a2 2 0 0 1 2 0 l0.15 0.08 a2 2 0 0 0 2.73 -0.73 l0.22 -0.39 a2 2 0 0 0 -0.73 -2.73 l-0.15 -0.08 a2 2 0 0 1 -1 -1.74 v-0.5 a2 2 0 0 1 1 -1.74 l0.15 -0.09 a2 2 0 0 0 0.73 -2.73 l-0.22 -0.38 a2 2 0 0 0 -2.73 -0.73 l-0.15 0.08 a2 2 0 0 1 -2 0 l-0.43 -0.25 a2 2 0 0 1 -1 -1.73 V4 a2 2 0 0 0 -2 -2 Z " + C(12, 12, 3),
        [IconKind.Sliders] = "M21 4 H14 M10 4 H3 M21 12 H12 M8 12 H3 M21 20 H16 M12 20 H3 M14 2 V6 M8 10 V14 M16 18 V22",
        [IconKind.Square] = R(3, 3, 18, 18, 2),
        [IconKind.Star] = "M12 2 L15.09 8.26 L22 9.27 L17 14.14 L18.18 21.02 L12 17.77 L5.82 21.02 L7 14.14 L2 9.27 L8.91 8.26 Z",
        [IconKind.Sun] = C(12, 12, 4) + "M12 2 V4 M12 20 V22 M4.93 4.93 L6.34 6.34 M17.66 17.66 L19.07 19.07 M2 12 H4 M20 12 H22 M6.34 17.66 L4.93 19.07 M19.07 4.93 L17.66 6.34",
        [IconKind.ToggleRight] = R(1, 5, 22, 14, 7) + C(16, 12, 3),
        [IconKind.Trash] = "M3 6 H21 M19 6 V20 A2 2 0 0 1 17 22 H7 A2 2 0 0 1 5 20 V6 M8 6 V4 A2 2 0 0 1 10 2 H14 A2 2 0 0 1 16 4 V6 M10 11 V17 M14 11 V17",
        [IconKind.TrendingDown] = "M23 18 L13.5 8.5 L8.5 13.5 L1 6 M17 18 H23 V12",
        [IconKind.TrendingUp] = "M23 6 L13.5 15.5 L8.5 10.5 L1 18 M17 6 H23 V12",
        [IconKind.Type] = "M4 7 V4 H20 V7 M9 20 H15 M12 4 V20",
        [IconKind.Upload] = "M21 15 V19 A2 2 0 0 1 19 21 H5 A2 2 0 0 1 3 19 V15 M17 8 L12 3 L7 8 M12 3 V15",
        [IconKind.User] = "M19 21 V19 A4 4 0 0 0 15 15 H9 A4 4 0 0 0 5 19 V21 " + C(12, 7, 4),
        [IconKind.Users] = "M16 21 V19 A4 4 0 0 0 12 15 H6 A4 4 0 0 0 2 19 V21 " + C(9, 7, 4) + "M22 21 V19 A4 4 0 0 0 19 15.13 M16 3.13 A4 4 0 0 1 16 10.88",
        [IconKind.X] = "M18 6 L6 18 M6 6 L18 18",
        [IconKind.Zap] = "M13 2 L3 14 H12 L11 22 L21 10 H12 L13 2 Z",
    };

    /// <summary>Returns the (cached, frozen) geometry for a built-in icon.</summary>
    public static Geometry? Get(IconKind kind)
    {
        if (kind == IconKind.None)
        {
            return null;
        }

        return s_cache.GetOrAdd(kind, k => Geometry.Parse(PathData[k]));
    }

    /// <summary>Returns a custom icon registered with <see cref="Register"/>.</summary>
    public static Geometry? Get(string name) => s_custom.TryGetValue(name, out var g) ? g : null;

    /// <summary>Registers a custom 24×24 stroke icon by name (usable via <c>Icon.Name</c>).</summary>
    public static void Register(string name, string pathData) => s_custom[name] = Geometry.Parse(pathData);

    private static string F(double v) => v.ToString(CultureInfo.InvariantCulture);

    /// <summary>Circle as path data.</summary>
    private static string C(double cx, double cy, double r) =>
        $"M{F(cx - r)} {F(cy)} A{F(r)} {F(r)} 0 1 0 {F(cx + r)} {F(cy)} A{F(r)} {F(r)} 0 1 0 {F(cx - r)} {F(cy)} Z ";

    /// <summary>Rounded rectangle as path data.</summary>
    private static string R(double x, double y, double w, double h, double r) =>
        $"M{F(x + r)} {F(y)} H{F(x + w - r)} A{F(r)} {F(r)} 0 0 1 {F(x + w)} {F(y + r)} " +
        $"V{F(y + h - r)} A{F(r)} {F(r)} 0 0 1 {F(x + w - r)} {F(y + h)} " +
        $"H{F(x + r)} A{F(r)} {F(r)} 0 0 1 {F(x)} {F(y + h - r)} " +
        $"V{F(y + r)} A{F(r)} {F(r)} 0 0 1 {F(x + r)} {F(y)} Z ";
}
