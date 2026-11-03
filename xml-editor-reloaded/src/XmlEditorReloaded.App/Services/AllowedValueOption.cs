namespace XmlEditorReloaded.App.Services;

/// <summary>
/// One selectable value of a field, used by filter, bulk-edit, and detail value editors. The handler
/// text is the raw value written to the document; the display text is the semantic name, and the
/// optional dropdown text keeps the identifier visible while the list is open.
/// </summary>
/// <param name="Value">Raw value written to the document, for example a lookup identifier.</param>
/// <param name="DisplayText">Semantic text shown when the field is not being edited.</param>
/// <param name="DropdownText">
/// Optional text shown in the open dropdown, usually the display text with the raw identifier.
/// </param>
public sealed record AllowedValueOption(string Value, string DisplayText, string? DropdownText = null)
{
    /// <summary>
    /// Gets the text shown in the open dropdown.
    /// </summary>
    public string ListText => DropdownText ?? DisplayText;

    /// <summary>
    /// Returns the raw value, so an editable combo box stores the identifier when the user picks an
    /// option instead of the display name.
    /// </summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Returns the semantic text of a raw value, or the value itself when it is unknown.
    /// </summary>
    /// <param name="value">Raw value read from the document.</param>
    /// <param name="options">Allowed values of the current field.</param>
    /// <returns>Semantic text, or the raw value when no option matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public static string GetDisplay(string value, IReadOnlyList<AllowedValueOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (value.Length == 0)
        {
            return value;
        }

        foreach (var option in options)
        {
            if (string.Equals(option.Value, value, StringComparison.Ordinal))
            {
                return option.DisplayText;
            }
        }

        return value;
    }

    /// <summary>
    /// Maps text entered in an editable value field back to a raw value. Raw identifiers take priority
    /// over display texts so two options cannot shadow each other's value.
    /// </summary>
    /// <param name="text">Text entered by the user or written by the dropdown.</param>
    /// <param name="options">Allowed values of the current field.</param>
    /// <returns>The matching raw value, or <paramref name="text"/> when no option matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public static string Resolve(string text, IReadOnlyList<AllowedValueOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (text.Length == 0)
        {
            return text;
        }

        foreach (var option in options)
        {
            if (string.Equals(option.Value, text, StringComparison.Ordinal))
            {
                return option.Value;
            }
        }

        foreach (var option in options)
        {
            if (string.Equals(option.DisplayText, text, StringComparison.Ordinal)
                || string.Equals(option.ListText, text, StringComparison.Ordinal))
            {
                return option.Value;
            }
        }

        return text;
    }
}
