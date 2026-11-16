using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace Lumen.Gallery.Pages;

public partial class InputsPage : UserControl
{
    private static readonly Regex s_email = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public InputsPage() => InitializeComponent();

    private void OnEmailChanged(object? sender, TextChangedEventArgs e)
    {
        var text = EmailBox.Text;
        EmailField.Error = string.IsNullOrEmpty(text) || s_email.IsMatch(text)
            ? null
            : "Please enter a valid email address.";
    }
}
