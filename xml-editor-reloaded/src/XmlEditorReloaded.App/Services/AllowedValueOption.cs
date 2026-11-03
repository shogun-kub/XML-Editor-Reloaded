namespace XmlEditorReloaded.App.Services;

/// <summary>
/// One selectable value of a column, used by filter and bulk-edit value fields. The handler text is the
/// raw value written to the document; the display text may add a lookup name or a localized label.
/// </summary>
/// <param name="Value">Raw value written to the document, for example a lookup identifier.</param>
/// <param name="DisplayText">Human-readable text shown in the dropdown.</param>
public sealed record AllowedValueOption(string Value, string DisplayText)
{
    /// <summary>
    /// Returns the raw value, so an editable combo box stores the identifier when the user picks an
    /// option instead of the display name.
    /// </summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;
}
