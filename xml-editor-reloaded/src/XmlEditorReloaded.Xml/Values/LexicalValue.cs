using System.Globalization;

namespace XmlEditorReloaded.Xml.Values;

/// <summary>
/// A value exactly as it appears in the XML text, preserving its lexical form such as <c>1.0</c>,
/// <c>-0.2</c>, leading zeros, and empty strings.
/// </summary>
/// <param name="RawText">The original text content of the element.</param>
public sealed record LexicalValue(string RawText)
{
    /// <summary>
    /// Gets an empty lexical value.
    /// </summary>
    public static LexicalValue Empty { get; } = new(string.Empty);

    /// <summary>
    /// Gets a value indicating whether the text is empty (but present in the document).
    /// </summary>
    public bool IsEmpty => RawText.Length == 0;

    /// <summary>
    /// Gets the text parsed as a signed 32-bit integer using invariant culture, or <see langword="null"/>.
    /// </summary>
    public int? AsInt32 =>
        int.TryParse(RawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : null;

    /// <summary>
    /// Gets the text parsed as a decimal using invariant culture, or <see langword="null"/>.
    /// </summary>
    public decimal? AsDecimal =>
        decimal.TryParse(RawText, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) ? value : null;

    /// <summary>
    /// Gets the text parsed as a JA2 boolean (<c>0</c> or <c>1</c>), or <see langword="null"/> for other text.
    /// </summary>
    public bool? AsBoolean => RawText switch
    {
        "0" => false,
        "1" => true,
        _ => null,
    };

    /// <summary>
    /// Attempts to parse the text as a signed 32-bit integer using invariant culture.
    /// </summary>
    /// <param name="value">The parsed value when successful.</param>
    /// <returns><see langword="true"/> when the text is a valid integer; otherwise <see langword="false"/>.</returns>
    public bool TryGetInt32(out int value) =>
        int.TryParse(RawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    /// <summary>
    /// Attempts to parse the text as a decimal using invariant culture.
    /// </summary>
    /// <param name="value">The parsed value when successful.</param>
    /// <returns><see langword="true"/> when the text is a valid decimal; otherwise <see langword="false"/>.</returns>
    public bool TryGetDecimal(out decimal value) =>
        decimal.TryParse(RawText, NumberStyles.Number, CultureInfo.InvariantCulture, out value);

    /// <summary>
    /// Returns the original text without normalization.
    /// </summary>
    /// <returns>The raw text.</returns>
    public override string ToString() => RawText;
}
