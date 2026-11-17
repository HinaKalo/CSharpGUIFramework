using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lumen.UI.Services;

namespace Lumen.App.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _themeIndex = (int)ThemeManager.Mode;

    public IReadOnlyList<AccentPreset> Accents => ThemeManager.AccentPresets;

    partial void OnThemeIndexChanged(int value) => ThemeManager.SetMode((ThemeMode)value);

    [RelayCommand]
    private void SetAccent(AccentPreset preset) => ThemeManager.SetAccent(preset.Color);
}
