namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Input length limit of one text field, optionally scoped to an overlay language. The limit restricts
/// typing in the detail form; it never validates, truncates, or rewrites an existing value.
/// </summary>
public sealed class ColumnTextLimit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTextLimit"/> class.
    /// </summary>
    /// <param name="xmlFieldName">XML element name of the field, case-sensitive.</param>
    /// <param name="languageCode">
    /// Overlay language code this limit applies to, or an empty string for the neutral limit that
    /// applies to the base table and to every language without a specific row.
    /// </param>
    /// <param name="maxLength">Maximum number of characters the user may type, greater than zero.</param>
    /// <param name="showCounter">Whether the detail form shows the remaining-character counter.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFieldName"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageCode"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxLength"/> is not positive.</exception>
    public ColumnTextLimit(string xmlFieldName, string languageCode, int maxLength, bool showCounter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldName);
        ArgumentNullException.ThrowIfNull(languageCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength);

        XmlFieldName = xmlFieldName;
        LanguageCode = languageCode;
        MaxLength = maxLength;
        ShowCounter = showCounter;
    }

    /// <summary>
    /// Gets the XML element name of the field, case-sensitive.
    /// </summary>
    public string XmlFieldName { get; }

    /// <summary>
    /// Gets the overlay language code, or an empty string for the neutral limit.
    /// </summary>
    public string LanguageCode { get; }

    /// <summary>
    /// Gets the maximum number of characters the user may type.
    /// </summary>
    public int MaxLength { get; }

    /// <summary>
    /// Gets a value indicating whether the detail form shows the remaining-character counter.
    /// </summary>
    public bool ShowCounter { get; }
}
