using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Validation;

/// <summary>
/// Read-only validator for loaded table documents. It reports unknown fields, type mismatches,
/// missing lookup references, duplicate keys, missing keys, and malformed dictionary masks.
/// </summary>
public sealed class TableValidator
{
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Validates a table document without mutating it.
    /// </summary>
    /// <param name="document">Loaded table document.</param>
    /// <param name="lookupRepository">Optional lookup repository used to validate lookup references.</param>
    /// <param name="languageCode">Optional language code for lookup overlays.</param>
    /// <param name="cancellationToken">Token to cancel validation.</param>
    /// <param name="relationTargetValues">
    /// Optional map from a relation source field to the keys of its resolved target table. Values are
    /// validated against the complete target, not against the relation filter, because filters only
    /// narrow picker options.
    /// </param>
    /// <returns>
    /// A validation report. Unknown fields are informational; type, lookup, key, and mask issues are warnings.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<ValidationReport> ValidateAsync(
        TableDocument document,
        ILookupRepository? lookupRepository = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? relationTargetValues = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        var tableName = document.Descriptor.TableName;
        var metadata = document.Metadata;
        if (metadata is null)
        {
            return new ValidationReport(new[]
            {
                new ValidationIssue(
                    ValidationSeverity.Info,
                    ValidationCodes.Unmapped,
                    "The table has no metadata and was validated in document-driven mode.",
                    tableName),
            });
        }

        var issues = new List<ValidationIssue>();
        var resolver = new OverlayKeyResolver(metadata);
        var records = resolver.ResolvePrimary(document.Xml);

        foreach (var record in records.Where(static record => !record.IsKeyResolved))
        {
            var message = record.MissingKeyFields.Count > 0
                ? $"A record is missing compound key field(s): {string.Join(", ", record.MissingKeyFields)}."
                : "A record has no resolvable primary key.";
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.MissingKey,
                message,
                tableName));
        }

        foreach (var group in records
                     .Where(static record => record.Key is not null)
                     .GroupBy(static record => record.Key!, StringComparer.Ordinal)
                     .Where(static group => group.Count() > 1))
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.DuplicateKey,
                $"Primary key '{group.Key}' occurs {group.Count()} times.",
                tableName,
                group.Key));
        }

        ValidateDictMasks(metadata, tableName, document.Xml, issues);

        var lookupCache = new Dictionary<string, IReadOnlyList<LookupEntry>>(StringComparer.Ordinal);
        foreach (var record in records)
        {
            await ValidateElementAsync(
                    record.Element,
                    string.Empty,
                    record.Key,
                    metadata,
                    tableName,
                    lookupRepository,
                    languageCode,
                    lookupCache,
                    relationTargetValues,
                    issues,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return new ValidationReport(issues);
    }

    private static void ValidateDictMasks(
        TableMetadata metadata,
        TableName tableName,
        XDocument document,
        List<ValidationIssue> issues)
    {
        foreach (var definition in metadata.DictFields)
        {
            var parts = definition.XmlFieldMask.Split(DictFieldDefinition.IndexPlaceholder, StringSplitOptions.None);
            if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) && string.IsNullOrEmpty(parts[1]))
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Warning,
                    ValidationCodes.DictMask,
                    $"Dictionary mask '{definition.XmlFieldMask}' must contain exactly one %Index% placeholder.",
                    tableName,
                    FieldPath: definition.XmlFieldMask));
                continue;
            }

            if (document.Root is null)
            {
                continue;
            }

            foreach (var element in document.Root.Descendants())
            {
                var name = element.Name.LocalName;
                if (!name.StartsWith(parts[0], StringComparison.Ordinal)
                    || !name.EndsWith(parts[1], StringComparison.Ordinal)
                    || name.Length <= parts[0].Length + parts[1].Length)
                {
                    continue;
                }

                var indexText = name[parts[0].Length..^parts[1].Length];
                if (!int.TryParse(indexText, NumberStyles.None, CultureInfo.InvariantCulture, out _))
                {
                    issues.Add(new ValidationIssue(
                        ValidationSeverity.Warning,
                        ValidationCodes.DictMask,
                        $"Dictionary field '{name}' does not have a numeric index.",
                        tableName,
                        FieldPath: name));
                }
            }
        }
    }

    private static async Task ValidateElementAsync(
        XElement element,
        string prefix,
        string? recordKey,
        TableMetadata metadata,
        TableName tableName,
        ILookupRepository? lookupRepository,
        string? languageCode,
        Dictionary<string, IReadOnlyList<LookupEntry>> lookupCache,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? relationTargetValues,
        List<ValidationIssue> issues,
        CancellationToken cancellationToken)
    {
        foreach (var child in element.Elements())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = prefix.Length == 0 ? child.Name.LocalName : prefix + "/" + child.Name.LocalName;
            if (child.HasElements)
            {
                await ValidateElementAsync(
                        child,
                        path,
                        recordKey,
                        metadata,
                        tableName,
                        lookupRepository,
                        languageCode,
                        lookupCache,
                        relationTargetValues,
                        issues,
                        cancellationToken)
                    .ConfigureAwait(false);
                continue;
            }

            await ValidateLeafAsync(
                    child,
                    path,
                    recordKey,
                    metadata,
                    tableName,
                    lookupRepository,
                    languageCode,
                    lookupCache,
                    relationTargetValues,
                    issues,
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static async Task ValidateLeafAsync(
        XElement leaf,
        string path,
        string? recordKey,
        TableMetadata metadata,
        TableName tableName,
        ILookupRepository? lookupRepository,
        string? languageCode,
        Dictionary<string, IReadOnlyList<LookupEntry>> lookupCache,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? relationTargetValues,
        List<ValidationIssue> issues,
        CancellationToken cancellationToken)
    {
        var fieldName = leaf.Name.LocalName;
        metadata.TryGetColumn(fieldName, out var column);
        metadata.TryGetColumnHint(fieldName, out var hint);
        if (column is null && hint is null)
        {
            // Hints are curated per present field, so only declaring tables can claim a verified
            // field set. For hint-only tables an unknown field is simply left to discovery.
            if (metadata.Columns.Count > 0 && !IsKnownField(fieldName, metadata))
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Info,
                    ValidationCodes.UnknownField,
                    $"Field '{path}' is not described by metadata.",
                    tableName,
                    recordKey,
                    path));
            }

            return;
        }

        var dataType = hint?.DataType ?? column?.DataType;
        if (dataType is null or ColumnDataType.String or ColumnDataType.Raw or ColumnDataType.Nested)
        {
            return;
        }

        if (string.IsNullOrEmpty(leaf.Value))
        {
            return;
        }

        if (!IsValidValue(dataType.Value, leaf.Value))
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.InvalidType,
                $"Value '{leaf.Value}' is not a valid {dataType} value.",
                tableName,
                recordKey,
                path));
            return;
        }

        var lookupTable = hint?.LookupTable ?? column?.LookupTable;
        if (lookupTable is not null && lookupRepository is not null
            && dataType is ColumnDataType.Lookup or ColumnDataType.Integer)
        {
            if (!long.TryParse(leaf.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return;
            }

            var entries = await GetLookupAsync(
                    lookupTable,
                    lookupRepository,
                    languageCode,
                    lookupCache,
                    cancellationToken)
                .ConfigureAwait(false);

            if (entries is not null && entries.All(entry => entry.Id != id))
            {
                issues.Add(new ValidationIssue(
                    ValidationSeverity.Warning,
                    ValidationCodes.LookupMissing,
                    $"Value '{id}' is not present in lookup table '{lookupTable}'.",
                    tableName,
                    recordKey,
                    path));
            }
        }

        if (relationTargetValues is not null
            && string.Equals(path, fieldName, StringComparison.Ordinal)
            && relationTargetValues.TryGetValue(fieldName, out var targetValues)
            && !targetValues.Contains(leaf.Value.Trim()))
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.LookupMissing,
                $"Value '{leaf.Value}' is not present in the target table of relation field '{fieldName}'.",
                tableName,
                recordKey,
                path));
        }
    }

    private static async Task<IReadOnlyList<LookupEntry>?> GetLookupAsync(
        string lookupTable,
        ILookupRepository lookupRepository,
        string? languageCode,
        Dictionary<string, IReadOnlyList<LookupEntry>> cache,
        CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(lookupTable, out var cached))
        {
            return cached;
        }

        TableName lookupName;
        try
        {
            lookupName = TableName.Parse(lookupTable);
        }
        catch (ArgumentException)
        {
            return null;
        }

        var result = await lookupRepository.GetEntriesAsync(lookupName, languageCode, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return null;
        }

        cache[lookupTable] = result.Data!;
        return result.Data;
    }

    private static bool IsKnownField(string fieldName, TableMetadata metadata)
    {
        if (metadata.ListFields.Any(field => string.Equals(field.XmlFieldName, fieldName, StringComparison.Ordinal)))
        {
            return true;
        }

        if (metadata.RecordKeyFields.Any(field =>
                string.Equals(field, fieldName, StringComparison.Ordinal)))
        {
            return true;
        }

        if (metadata.RecordKey?.Segments.Any(segment =>
                string.Equals(segment.KeyField, fieldName, StringComparison.Ordinal)) == true)
        {
            return true;
        }

        return metadata.DictFields.Any(definition => IsExpandedDictField(fieldName, definition.XmlFieldMask));
    }

    private static bool IsExpandedDictField(string fieldName, string mask)
    {
        var parts = mask.Split(DictFieldDefinition.IndexPlaceholder, StringSplitOptions.None);
        if (parts.Length != 2 || !fieldName.StartsWith(parts[0], StringComparison.Ordinal)
            || !fieldName.EndsWith(parts[1], StringComparison.Ordinal)
            || fieldName.Length <= parts[0].Length + parts[1].Length)
        {
            return false;
        }

        var indexText = fieldName[parts[0].Length..^parts[1].Length];
        return int.TryParse(indexText, NumberStyles.None, CultureInfo.InvariantCulture, out _);
    }

    private static bool IsValidValue(ColumnDataType dataType, string value) => dataType switch
    {
        ColumnDataType.Integer =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Lookup =>
            long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Flags =>
            uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Decimal =>
            decimal.TryParse(value, DecimalStyles, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Boolean => value.Trim() is "0" or "1",
        _ => true,
    };
}
