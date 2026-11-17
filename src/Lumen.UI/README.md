# Lumen.UI

A modern, token-driven component library for **Avalonia 12**, built on top of FluentTheme.

- Light / dark / system themes and runtime accent colors (`ThemeManager`)
- Design tokens: `Lumen.Brush.*`, `Lumen.Radius.*`, `Lumen.Shadow.*`, `Lumen.FontSize.*`
- Restyled built-in controls (button variants `primary` / `soft` / `outline` / `ghost` / `danger` …)
- Components: `AppShell`, `Card`, `Badge`, `Avatar`, `StatCard`, `EmptyState`, `Skeleton`, `Spinner`,
  `FormField`, `SearchBox`, `SegmentedControl`, `InfoBar`, `Toast`, `Dialog`, `Icon` (65 stroke icons)
- Trim and Native AOT compatible

## Usage

```csharp
AppBuilder.Configure<App>().UsePlatformDetect().UseLumen();
```

```xml
<Application xmlns:l="https://github.com/HinaKalo/lumen">
  <Application.Styles>
    <l:LumenTheme />
  </Application.Styles>
</Application>
```

Start a new app from the template:

```bash
dotnet new install Lumen.Templates
dotnet new lumen-app -n MyTool
```

Docs, gallery and screenshots: https://github.com/HinaKalo/CSharpGUIFramework
