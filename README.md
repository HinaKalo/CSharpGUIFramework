# Lumen.UI

基于 **Avalonia 12 / .NET 10** 的自用现代化组件库。以官方 FluentTheme 为底，叠加一套设计令牌（颜色 / 字体 / 间距 / 圆角 / 阴影）、重绘的内置控件样式和一组常用自定义控件；亮暗双主题，运行时可切换强调色；附带 Gallery 演示和 `dotnet new` 项目模板，新项目一条命令起步。

![Overview (light)](docs/images/overview-light.png)

<table>
  <tr>
    <td><img src="docs/images/overview-dark.png" alt="Overview dark" /></td>
    <td><img src="docs/images/display-dark.png" alt="Data display dark" /></td>
  </tr>
  <tr>
    <td><img src="docs/images/inputs-light.png" alt="Inputs light" /></td>
    <td><img src="docs/images/overlays-light.png" alt="Dialog and toasts" /></td>
  </tr>
</table>

## 快速开始

### 方式一：用模板新建项目（推荐）

```bash
# 一次性安装模板（在本仓库根目录执行）
dotnet new install ./templates/Lumen.App

# 在本仓库的同级目录创建新项目（默认按 ../CSharpGUIFramework/src/Lumen.UI 引用组件库）
cd ..
dotnet new lumen-app -n MyTool
dotnet run --project MyTool
```

组件库不在同级目录时，用 `--LumenPath` 指定 `Lumen.UI.csproj` 的路径（相对新项目目录或绝对路径）：

```bash
dotnet new lumen-app -n MyTool --LumenPath "D:/libs/CSharpGUIFramework/src/Lumen.UI/Lumen.UI.csproj"
```

生成的项目已接好：`LumenTheme`、`AppShell`（侧边导航 + 顶栏 + 页面切换动画）、CommunityToolkit.Mvvm、示例页面（表单、命令、Toast、确认对话框、主题与强调色设置）。

### 方式二：接入已有项目

1. 引用 `src/Lumen.UI/Lumen.UI.csproj`（或 `dotnet pack src/Lumen.UI -o ./artifacts` 后引用 NuGet 包）。
2. `Program.cs`：

   ```csharp
   AppBuilder.Configure<App>()
       .UsePlatformDetect()
       .UseLumen()          // 注册 Inter 字体
       .LogToTrace();
   ```

3. `App.axaml` 用 `LumenTheme` 替换 `FluentTheme`：

   ```xml
   <Application xmlns:l="https://github.com/HinaKalo/lumen" ...>
     <Application.Styles>
       <l:LumenTheme />
     </Application.Styles>
   </Application>
   ```

所有类型（控件、图标、服务）都在同一个 XAML 命名空间 `https://github.com/HinaKalo/lumen` 下。

## 组件

| 分类 | 组件 |
| --- | --- |
| 布局 | `AppShell`（可折叠侧边栏、顶栏、页面切换、内置 Dialog/Toast 宿主）、`NavItem`、`Card`、`SectionHeader` |
| 展示 | `Icon`（65 个线性图标）、`Badge`、`Avatar`、`StatCard`、`EmptyState`、`Skeleton`、`Spinner` |
| 输入 | `FormField`（标签 + 提示 + 校验错误）、`SearchBox`、`SegmentedControl`、`ThemeToggleButton` |
| 反馈 | `InfoBar`、`Toast` / `ToastHost`、`Dialog` / `DialogHost` |
| 内置控件重绘 | Button / ToggleButton（全新模板）、TextBox、ComboBox、CheckBox、RadioButton、ToggleSwitch、Slider、ProgressBar、ListBox、TabControl、ToolTip、Menu / Flyout |

### 按钮变体

```xml
<Button Classes="primary" Content="Save" />
<Button Classes="soft | outline | ghost | danger | danger-soft | link" />
<Button Classes="sm | lg" />            <!-- 尺寸 -->
<Button Classes="icon ghost"><l:Icon Kind="Settings" /></Button>
<Button Classes="pill" />  <Button Classes="block" />
```

### 文字层级

```xml
<TextBlock Classes="display | headline | title | subtitle | body | caption | overline" />
<TextBlock Classes="body secondary" />  <!-- 修饰：primary secondary tertiary accent success warning danger strong medium mono -->
```

### 反馈：Toast 与对话框

```csharp
Toast.Success("已保存", "设置已更新");
Toast.Error("上传失败", "服务器返回 503");

if (await Dialog.ConfirmAsync("删除项目？", "此操作不可撤销。", "删除", isDanger: true))
{
    // ...
}

var result = await Dialog.ShowAsync(myCustomControl);   // 任意控件当对话框，Dialog.Close(control, value) 返回结果
```

`AppShell` 自带 `DialogHost` 和 `ToastHost`；不使用 `AppShell` 时，把窗口内容包进 `<l:DialogHost>` 并放一个 `<l:ToastHost />` 即可。

### 导航壳

```csharp
public IReadOnlyList<NavItem> NavItems { get; } =
[
    NavItem.Header("WORKSPACE"),
    new("Home", IconKind.Home, () => new HomePage { DataContext = new HomeViewModel() }),
    new("Inbox", IconKind.Inbox, () => new InboxPage()) { Badge = "3" },
];
```

```xml
<l:AppShell Title="My app" NavItems="{Binding NavItems}">
  <l:AppShell.HeaderActions>
    <l:ThemeToggleButton />
  </l:AppShell.HeaderActions>
</l:AppShell>
```

页面首次选中时创建并缓存；`shell.Navigate("Home")` 按标题或 `Tag` 跳转。

## 主题与设计令牌

```csharp
ThemeManager.SetMode(ThemeMode.Dark);          // System / Light / Dark
ThemeManager.Toggle();
ThemeManager.SetAccent(Color.Parse("#0D9488")); // 任意颜色，全局即时生效
ThemeManager.AccentPresets                      // 9 个预设
```

在 XAML 中总是用 `DynamicResource` 引用令牌，切换主题和强调色时自动更新：

| 令牌 | 说明 |
| --- | --- |
| `Lumen.Brush.Background / Surface / SurfaceElevated / SurfaceSubtle / SurfaceHover / Sidebar` | 背景与表面 |
| `Lumen.Brush.Border / BorderSubtle / BorderStrong` | 边框 |
| `Lumen.Brush.TextPrimary / TextSecondary / TextTertiary / TextDisabled` | 文字 |
| `Lumen.Brush.Accent / AccentHover / AccentSubtle / AccentText` | 强调色（跟随 `SetAccent`） |
| `Lumen.Brush.{Success,Warning,Danger,Info}` + `Subtle` / `Text` / `Border` | 状态色 |
| `Lumen.Radius.{Xs,Sm,Md,Lg,Xl,Full}` | 圆角 4 / 6 / 8 / 12 / 16 / 圆 |
| `Lumen.Shadow.{Xs,Sm,Md,Lg}` | 阴影（亮暗分别调校） |
| `Lumen.FontSize.{Caption,Body,Subtitle,Title,Headline,Display}` | 12 / 14 / 16 / 20 / 28 / 36 |
| `Lumen.Height.{Sm,Md,Lg}` | 控件高度 28 / 36 / 44 |

令牌定义在 [`src/Lumen.UI/Tokens`](src/Lumen.UI/Tokens)。Fluent 自带控件通过同目录下的 "Fluent bridge" 映射到这些令牌，所以未重绘的 Fluent 控件也会跟随 Lumen 配色。

### 图标

```xml
<l:Icon Kind="Search" Size="16" />
<l:Icon Data="M4 12 H20" />                 <!-- 任意 24×24 路径 -->
```

```csharp
LumenIcons.Register("rocket", "M4.5 16.5 ...");  // 之后 <l:Icon NameKey="rocket" />
```

图标形状源自 [Lucide](https://lucide.dev) / [Feather](https://feathericons.com)（ISC / MIT）。

## 项目结构

```
src/Lumen.UI/           组件库
  Tokens/               设计令牌（Foundation / Light / Dark + Fluent bridge）
  Themes/               自定义控件与 Button 的 ControlTheme
  Styles/               排版与内置控件样式微调
  Controls/  Icons/  Services/
samples/Lumen.Gallery/  组件演示程序
templates/Lumen.App/    dotnet new 模板（lumen-app）
tests/Lumen.UI.Tests/   Avalonia.Headless 测试：主题切换、强调色、每个 Gallery 页面亮/暗渲染
```

## 开发

```bash
dotnet build CSharpGUIFramework.slnx
dotnet run --project samples/Lumen.Gallery
dotnet test
```

测试会把每个 Gallery 页面的亮/暗截图写到测试输出目录下的 `snapshots/`（或环境变量 `LUMEN_SNAPSHOT_DIR` 指定的目录），方便肉眼回归检查；CI 会把它们作为构建产物上传。

## License

MIT
