using Lumen.Gallery.Pages;
using Lumen.UI.Controls;
using Lumen.UI.Icons;

namespace Lumen.Gallery;

/// <summary>Navigation model for the gallery (also used by the snapshot tests).</summary>
public static class GalleryPages
{
    public static IReadOnlyList<NavItem> Create() =>
    [
        NavItem.Header("GET STARTED"),
        new("Overview", IconKind.LayoutDashboard, () => new OverviewPage()) { Tag = "overview" },
        new("Foundations", IconKind.Droplet, () => new FoundationsPage()) { Tag = "foundations" },
        NavItem.Header("COMPONENTS"),
        new("Buttons", IconKind.MousePointer, () => new ButtonsPage()) { Tag = "buttons" },
        new("Inputs", IconKind.ToggleRight, () => new InputsPage()) { Tag = "inputs" },
        new("Data display", IconKind.Package, () => new DisplayPage()) { Tag = "display", Badge = "New" },
        new("Feedback", IconKind.Bell, () => new FeedbackPage()) { Tag = "feedback" },
        new("Layout", IconKind.Layers, () => new LayoutPage()) { Tag = "layout" },
        new("Icons", IconKind.Star, () => new IconsPage()) { Tag = "icons" },
    ];
}
