using System.Globalization;
using System.Windows.Data;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Converts a raw lookup identifier to its display name, marking values that are absent from the
/// lookup file. The converter is created per grid column and carries that column's lookup data.
/// </summary>
internal sealed class LookupDisplayConverter : IValueConverter
{
    private readonly LookupColumnData _lookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupDisplayConverter"/> class.
    /// </summary>
    /// <param name="lookup">Lookup data of the column.</param>
    public LookupDisplayConverter(LookupColumnData lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);
        _lookup = lookup;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        _lookup.Display(value as string);

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value as string ?? string.Empty;
}

/// <summary>
/// Converts a raw lookup identifier to a tooltip that shows the identifier and unknown state.
/// </summary>
internal sealed class LookupTooltipConverter : IValueConverter
{
    private readonly LookupColumnData _lookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupTooltipConverter"/> class.
    /// </summary>
    /// <param name="lookup">Lookup data of the column.</param>
    public LookupTooltipConverter(LookupColumnData lookup)
    {
        ArgumentNullException.ThrowIfNull(lookup);
        _lookup = lookup;
    }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        _lookup.GetTooltip(value as string);

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value as string ?? string.Empty;
}

/// <summary>
/// Converts the sparse <c>0</c>/<c>1</c> lexical representation of a Boolean field to and from a
/// checkbox state.
/// </summary>
internal sealed class BooleanValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static BooleanValueConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
        {
            return boolean;
        }

        if (value is not string text)
        {
            return false;
        }

        var trimmed = text.Trim();
        return trimmed == "1"
            || (bool.TryParse(trimmed, out var parsed) && parsed);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? "1" : "0";
}
