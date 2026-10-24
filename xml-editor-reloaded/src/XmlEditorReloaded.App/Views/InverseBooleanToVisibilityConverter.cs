using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Shows an element when the bound Boolean value is <see langword="false"/>. Used to hide the standalone
/// grid chrome when the grid is embedded in the item workspace.
/// </summary>
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? Visibility.Collapsed : Visibility.Visible;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
