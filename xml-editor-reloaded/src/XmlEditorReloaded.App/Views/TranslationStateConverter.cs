using System.Globalization;
using System.Windows.Data;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Converts a row's <see cref="TableRowViewModel.TranslationState"/> dictionary into the untranslated
/// flag for one field, so a grid cell can highlight it.
/// </summary>
internal sealed class TranslationStateConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static TranslationStateConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is IReadOnlyDictionary<string, bool> state
        && parameter is string fieldName
        && state.TryGetValue(fieldName, out var untranslated)
        && untranslated;

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
