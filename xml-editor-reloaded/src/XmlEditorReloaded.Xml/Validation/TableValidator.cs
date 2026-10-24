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
    /// <summary>
    /// Validates a table document without mutating it.
    /// </summary>
    /// <param name="document">Loaded table document.</param>
    /// <param name="lookupRepository">Optional lookup repository used to validate lookup references.</param>
    /// <param name="languageCode">Optional language code for lookup overlays.</param>
    /// <param name="cancellationToken">Token to cancel validation.</param>
    /// <returns>
    /// A validation report. Unknown fields are informational; type, lookup, key, and mask issues are warnings.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<ValidationReport> ValidateAsync(
        TableDocument document,
        ILookupRepository? lookupRepository = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
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
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.MissingKey,
                "A record has no resolvable primary key.",
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
        List<ValidationIssue> issues,
        CancellationToken cancellationToken)
    {
        var fieldName = leaf.Name.LocalName;
        if (!metadata.TryGetColumn(fieldName, out var column) || column is null)
        {
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

        if (string.IsNullOrEmpty(leaf.Value) || column.DataType is ColumnDataType.String or ColumnDataType.Raw or ColumnDataType.Nested)
        {
            return;
        }

        if (!IsValidValue(column.DataType, leaf.Value))
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.InvalidType,
                $"Value '{leaf.Value}' is not a valid {column.DataType} value.",
                tableName,
                recordKey,
                path));
            return;
        }

        if (column.LookupTable is not null && lookupRepository is not null
            && column.DataType is ColumnDataType.Lookup or ColumnDataType.Integer or ColumnDataType.Flags)
        {
            var id = int.Parse(leaf.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            var entries = await GetLookupAsync(
                    column.LookupTable,
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
                    $"Value '{id}' is not present in lookup table '{column.LookupTable}'.",
                    tableName,
                    recordKey,
                    path));
            }
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
        ColumnDataType.Integer or ColumnDataType.Lookup or ColumnDataType.Flags =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Decimal =>
            decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _),
        ColumnDataType.Boolean => value is "0" or "1",
        _ => true,
    };
}
