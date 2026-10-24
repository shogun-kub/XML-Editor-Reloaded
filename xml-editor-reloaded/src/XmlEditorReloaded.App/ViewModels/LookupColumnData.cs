using System.Globalization;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One selectable entry of a lookup column, including values that are present in the document but
/// missing from the lookup file.
/// </summary>
public sealed class LookupOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupOption"/> class.
    /// </summary>
    /// <param name="idText">Lexical identifier written back to the XML document.</param>
    /// <param name="name">Human-readable lookup name, or <see langword="null"/> for an unknown value.</param>
    /// <param name="isUnknown">Whether the value is absent from the lookup file.</param>
    public LookupOption(string idText, string? name, bool isUnknown)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(idText);
        IdText = idText;
        Name = name ?? string.Empty;
        IsUnknown = isUnknown;
    }

    /// <summary>
    /// Gets the identifier text that is written back to the document.
    /// </summary>
    public string IdText { get; }

    /// <summary>
    /// Gets the lookup name, or an empty string for an unknown value.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the value is absent from the lookup file.
    /// </summary>
    public bool IsUnknown { get; }

    /// <summary>
    /// Gets the text shown in a combo box.
    /// </summary>
    public string DisplayText => IsUnknown
        ? Strings.Format("LookupUnknownFormat", IdText)
        : Name;

    /// <summary>
    /// Gets the text shown in the dropdown list, which keeps the identifier visible next to the name.
    /// </summary>
    public string DropdownText => IsUnknown
        ? Strings.Format("LookupUnknownFormat", IdText)
        : $"{Name} ({IdText})";
}

/// <summary>
/// Loaded lookup entries and the unknown values found in one column of the current table. The data is
/// read-only; selecting an option writes the original identifier, never the display name.
/// </summary>
public sealed class LookupColumnData
{
    private readonly Dictionary<long, LookupOption> _byId;
    private readonly List<LookupEntry> _entries;

    private LookupColumnData(
        TableName tableName,
        IReadOnlyList<LookupEntry> entries,
        IReadOnlyList<LookupOption> options,
        string? errorCode,
        string? errorText)
    {
        TableName = tableName;
        _entries = entries.ToList();
        _byId = new Dictionary<long, LookupOption>();
        foreach (var option in options)
        {
            if (!option.IsUnknown
                && long.TryParse(option.IdText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
                && !_byId.ContainsKey(id))
            {
                _byId.Add(id, option);
            }
        }

        Options = options;
        ErrorCode = errorCode;
        ErrorText = errorText;
    }

    /// <summary>
    /// Gets the dotted lookup table name, for example <c>Lookup.ItemClass</c>.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the known lookup entries in file order, including entries that no row currently uses.
    /// </summary>
    public IReadOnlyList<LookupEntry> Entries => _entries;

    /// <summary>
    /// Gets the selectable options: known entries first, then unknown values found in the column.
    /// </summary>
    public IReadOnlyList<LookupOption> Options { get; }

    /// <summary>
    /// Gets the error code when the lookup file could not be loaded, or <see langword="null"/>.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the error text when the lookup file could not be loaded, or <see langword="null"/>.
    /// </summary>
    public string? ErrorText { get; }

    /// <summary>
    /// Gets a value indicating whether the lookup entries were loaded successfully.
    /// </summary>
    public bool IsAvailable => ErrorCode is null;

    /// <summary>
    /// Creates lookup data from loaded entries and the raw values of one column.
    /// </summary>
    /// <param name="tableName">Dotted lookup table name.</param>
    /// <param name="entries">Entries read from the lookup file, in file order.</param>
    /// <param name="values">Raw values of the column, including empty and unknown values.</param>
    /// <returns>Lookup data whose options include unknown values in first-appearance order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entries"/> or <paramref name="values"/> is null.</exception>
    public static LookupColumnData Create(
        TableName tableName,
        IReadOnlyList<LookupEntry> entries,
        IEnumerable<string?> values)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(values);

        var knownById = new HashSet<long>();
        var options = new List<LookupOption>();
        foreach (var entry in entries)
        {
            if (!knownById.Add(entry.Id))
            {
                continue;
            }

            options.Add(new LookupOption(
                entry.Id.ToString(CultureInfo.InvariantCulture),
                entry.Name,
                isUnknown: false));
        }

        var seenUnknown = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var trimmed = value.Trim();
            if (long.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)
                && knownById.Contains(id))
            {
                continue;
            }

            if (seenUnknown.Add(trimmed))
            {
                options.Add(new LookupOption(trimmed, name: null, isUnknown: true));
            }
        }

        return new LookupColumnData(tableName, entries, options, errorCode: null, errorText: null);
    }

    /// <summary>
    /// Creates a failed lookup marker while keeping the column editable as plain text.
    /// </summary>
    /// <param name="tableName">Dotted lookup table name.</param>
    /// <param name="errorCode">Machine-readable lookup error code.</param>
    /// <param name="errorText">Human-readable lookup error text.</param>
    /// <returns>Lookup data with no options and <see cref="IsAvailable"/> set to <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorCode"/> or <paramref name="errorText"/> is empty.</exception>
    public static LookupColumnData Failed(TableName tableName, string errorCode, string errorText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorText);
        return new LookupColumnData(
            tableName,
            Array.Empty<LookupEntry>(),
            Array.Empty<LookupOption>(),
            errorCode,
            errorText);
    }

    /// <summary>
    /// Returns the display text of a raw column value: the lookup name when known, otherwise the raw
    /// value with an explicit unknown marker.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <returns>The display text, or an empty string for an absent or empty value.</returns>
    public string Display(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        if (!IsAvailable)
        {
            return rawValue.Trim();
        }

        return TryGetOption(rawValue, out var option)
            ? option.DisplayText
            : Strings.Format("LookupUnknownFormat", rawValue.Trim());
    }

    /// <summary>
    /// Returns the lookup name of a raw value when it is known.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <returns>The lookup name, or <see langword="null"/> when the value is absent or unknown.</returns>
    public string? GetName(string? rawValue) =>
        TryGetOption(rawValue, out var option) && !option.IsUnknown ? option.Name : null;

    /// <summary>
    /// Returns the tooltip describing a raw value, including the identifier and whether it is unknown.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <returns>A localized tooltip, or an empty string for an absent or empty value.</returns>
    public string GetTooltip(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return string.Empty;
        }

        if (!IsAvailable)
        {
            return ErrorText ?? string.Empty;
        }

        return TryGetOption(rawValue, out var option)
            ? Strings.Format("LookupValueTooltipFormat", option.DisplayText, option.IdText)
            : Strings.Format("LookupUnknownTooltipFormat", rawValue.Trim());
    }

    /// <summary>
    /// Returns the name of an exact lookup entry, including entries whose identifier is a combination
    /// of bits rather than a single bit.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <returns>The exact entry name, or <see langword="null"/> when no entry matches.</returns>
    public string? GetExactEntryName(string? rawValue)
    {
        if (!TryParseId(rawValue, out var id))
        {
            return null;
        }

        return _entries.FirstOrDefault(entry => entry.Id == id)?.Name;
    }

    private bool TryGetOption(string? rawValue, out LookupOption option)
    {
        option = null!;
        if (!TryParseId(rawValue, out var id))
        {
            return false;
        }

        return _byId.TryGetValue(id, out option!);
    }

    private static bool TryParseId(string? rawValue, out long id) =>
        long.TryParse(
            rawValue?.Trim(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out id);
}
