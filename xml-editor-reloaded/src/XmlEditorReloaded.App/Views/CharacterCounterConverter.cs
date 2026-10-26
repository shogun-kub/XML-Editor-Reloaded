using System.Globalization;
using System.Windows.Data;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.Views;/// <summary>
/// Shows the remaining characters of a text box: the visible text and the maximum length are combined
/// so the counter follows typing without changing the two-way value binding.
/// </summary>
public sealed class CharacterCounterConverter : IMultiValueConverter
{
    /// <summary>
    /// Formats the counter from the visible text and the field limit.
    /// </summary>
    /// <param name="values">
    /// First value: text currently shown in the text box; second value: maximum length, where zero means
    /// unlimited.
    /// </param>
    /// <param name="targetType">Target property type, ignored.</param>
    /// <param name="parameter">Converter parameter, ignored.</param>
    /// <param name="culture">Culture used for the conversion, ignored.</param>
    /// <returns>The localized counter text, or an empty string when the field is unlimited.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return string.Empty;
        }

        var text = values[0] as string ?? string.Empty;
        var maxLength = values[1] is int limit ? limit : 0;
        return CharacterCounter.Format(maxLength, text.Length);
    }

    /// <summary>
    /// Not supported; the counter is a one-way display binding.
    /// </summary>
    /// <param name="value">Value to convert back.</param>
    /// <param name="targetTypes">Target property types, ignored.</param>
    /// <param name="parameter">Converter parameter, ignored.</param>
    /// <param name="culture">Culture used for the conversion, ignored.</param>
    /// <returns>Never returns because the conversion is not supported.</returns>
    /// <exception cref="NotSupportedException">Always thrown.</exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("The character counter is a one-way display binding.");
}
