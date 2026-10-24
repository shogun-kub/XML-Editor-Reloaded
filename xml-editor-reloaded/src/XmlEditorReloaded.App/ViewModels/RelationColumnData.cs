using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One selectable entry of a relation column, including values that are present in the document but
/// absent from the resolved target table.
/// </summary>
public sealed class RelationOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationOption"/> class.
    /// </summary>
    /// <param name="value">Key text written back to the XML document.</param>
    /// <param name="display">Display name of the target record, or an empty string for unknown values.</param>
    /// <param name="isUnknown">Whether the value is absent from the target table.</param>
    public RelationOption(string value, string display, bool isUnknown)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
        Display = display ?? string.Empty;
        IsUnknown = isUnknown;
    }

    /// <summary>
    /// Gets the key text that is written back to the document.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the display name of the target record, or an empty string for unknown values.
    /// </summary>
    public string Display { get; }

    /// <summary>
    /// Gets a value indicating whether the value is absent from the target table.
    /// </summary>
    public bool IsUnknown { get; }

    /// <summary>
    /// Gets the text shown in the closed picker.
    /// </summary>
    public string DisplayText => IsUnknown
        ? Strings.Format("LookupUnknownFormat", Value)
        : Display;

    /// <summary>
    /// Gets the text shown in the dropdown list, which keeps the key visible next to the name.
    /// </summary>
    public string DropdownText => IsUnknown
        ? Strings.Format("LookupUnknownFormat", Value)
        : $"{Display} ({Value})";
}

/// <summary>
/// Resolved relation data for one field: selectable options and the display text of raw values. The
/// data is read-only; selecting an option writes the original key, never the display name.
/// </summary>
public sealed class RelationColumnData
{
    private readonly RelationTarget? _target;
    private readonly List<RelationOption> _options;

    private RelationColumnData(
        TableRelation relation,
        RelationTarget? target,
        IReadOnlyList<RelationOption> options,
        string? errorCode,
        string? errorText)
    {
        Relation = relation;
        _target = target;
        _options = options.ToList();
        Options = _options;
        ErrorCode = errorCode;
        ErrorText = errorText;
    }

    /// <summary>
    /// Gets the relation metadata of the field.
    /// </summary>
    public TableRelation Relation { get; }

    /// <summary>
    /// Gets the target table name.
    /// </summary>
    public TableName TargetTable => Relation.TargetTable;

    /// <summary>
    /// Gets the selectable options: filtered target records first, then unresolved document values.
    /// </summary>
    public IReadOnlyList<RelationOption> Options { get; }

    /// <summary>
    /// Gets the error code when the target table could not be loaded, or <see langword="null"/>.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the error text when the target table could not be loaded, or <see langword="null"/>.
    /// </summary>
    public string? ErrorText { get; }

    /// <summary>
    /// Gets a value indicating whether the target table was resolved successfully.
    /// </summary>
    public bool IsAvailable => ErrorCode is null;

    /// <summary>
    /// Creates relation data from a resolved target and the raw values of one column.
    /// </summary>
    /// <param name="relation">Relation metadata of the field.</param>
    /// <param name="target">Resolved target rows and filtered options.</param>
    /// <param name="values">Raw values of the column, including empty and unresolved values.</param>
    /// <returns>
    /// Relation data whose options contain the filtered target rows plus values that already exist in
    /// the document but fall outside the filter or are absent from the target.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public static RelationColumnData Create(
        TableRelation relation,
        RelationTarget target,
        IEnumerable<string?> values)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(values);

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var options = new List<RelationOption>();
        foreach (var entry in target.Options)
        {
            if (seen.Add(entry.Value))
            {
                options.Add(new RelationOption(entry.Value, entry.Display, isUnknown: false));
            }
        }

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var trimmed = value.Trim();
            if (!seen.Add(trimmed))
            {
                continue;
            }

            var entry = target.Find(trimmed);
            options.Add(entry is null
                ? new RelationOption(trimmed, string.Empty, isUnknown: true)
                : new RelationOption(entry.Value, entry.Display, isUnknown: false));
        }

        return new RelationColumnData(relation, target, options, errorCode: null, errorText: null);
    }

    /// <summary>
    /// Creates a failed relation marker while keeping the column editable as plain text.
    /// </summary>
    /// <param name="relation">Relation metadata of the field.</param>
    /// <param name="errorCode">Machine-readable relation error code.</param>
    /// <param name="errorText">Human-readable relation error text.</param>
    /// <returns>Relation data with no options and <see cref="IsAvailable"/> set to <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when an error code or text is empty.</exception>
    public static RelationColumnData Failed(TableRelation relation, string errorCode, string errorText)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorText);
        return new RelationColumnData(
            relation,
            target: null,
            Array.Empty<RelationOption>(),
            errorCode,
            errorText);
    }

    /// <summary>
    /// Returns the display text of a raw value: the target name when known, otherwise the raw value
    /// with an explicit unknown marker.
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

        var entry = _target!.Find(rawValue);
        return entry is null
            ? Strings.Format("LookupUnknownFormat", rawValue.Trim())
            : entry.Display;
    }

    /// <summary>
    /// Returns the target display name of a raw value when the value resolves.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <returns>The target name, or <see langword="null"/> when the value is absent or unknown.</returns>
    public string? GetName(string? rawValue) => _target?.Find(rawValue)?.Display;

    /// <summary>
    /// Returns the tooltip describing a raw value, including its key and unknown state.
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

        var entry = _target!.Find(rawValue);
        return entry is null
            ? Strings.Format("RelationUnknownTooltipFormat", rawValue.Trim())
            : Strings.Format("RelationValueTooltipFormat", entry.Display, entry.Value);
    }

    /// <summary>
    /// Filters options by display text or key, which drives the searchable picker.
    /// </summary>
    /// <param name="term">Search term; an empty term returns every option.</param>
    /// <returns>Matching options in their original order.</returns>
    public IReadOnlyList<RelationOption> Filter(string? term)
    {
        var trimmed = term?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return Options;
        }

        return _options
            .Where(option =>
                option.DisplayText.Contains(trimmed, StringComparison.OrdinalIgnoreCase)
                || option.Value.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    /// <summary>
    /// Resolves the canonical key of a raw value for record navigation.
    /// </summary>
    /// <param name="rawValue">Raw value read from the document.</param>
    /// <param name="value">Canonical target key when the value resolves.</param>
    /// <returns><see langword="true"/> when the target contains the value; otherwise <see langword="false"/>.</returns>
    public bool TryGetTargetValue(string? rawValue, out string value)
    {
        var entry = _target?.Find(rawValue);
        if (entry is null)
        {
            value = string.Empty;
            return false;
        }

        value = entry.Value;
        return true;
    }
}
