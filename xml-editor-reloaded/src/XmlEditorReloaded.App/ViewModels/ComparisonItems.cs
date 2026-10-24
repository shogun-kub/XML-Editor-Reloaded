using System.Globalization;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Comparison;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Display projection of one <see cref="TableComparisonEntry"/> for the comparison window.
/// </summary>
public sealed class ComparisonTableItem
{
    /// <summary>
    /// Initializes a new item for a table comparison entry.
    /// </summary>
    /// <param name="entry">Table comparison entry to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entry"/> is null.</exception>
    public ComparisonTableItem(TableComparisonEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entry = entry;
        StatusText = ResolveStatusText(entry);
        LeftRecordsText = FormatCount(entry.Left?.Snapshot?.RecordCount);
        RightRecordsText = FormatCount(entry.Right?.Snapshot?.RecordCount);
        ProfileText = BuildProfileText(entry);
        DiagnosticsText = BuildDiagnosticsText(entry);
    }

    /// <summary>
    /// Gets the underlying comparison entry.
    /// </summary>
    public TableComparisonEntry Entry { get; }

    /// <summary>
    /// Gets the dotted table name.
    /// </summary>
    public string TableName => Entry.TableName.Value;

    /// <summary>
    /// Gets the localized presence/load status label.
    /// </summary>
    public string StatusText { get; }

    /// <summary>
    /// Gets the left record count, or a dash when the table is absent.
    /// </summary>
    public string LeftRecordsText { get; }

    /// <summary>
    /// Gets the right record count, or a dash when the table is absent.
    /// </summary>
    public string RightRecordsText { get; }

    /// <summary>
    /// Gets the number of fields present only in the right data set.
    /// </summary>
    public int AddedFields => Entry.AddedFieldCount;

    /// <summary>
    /// Gets the number of fields present only in the left data set.
    /// </summary>
    public int RemovedFields => Entry.RemovedFieldCount;

    /// <summary>
    /// Gets the number of fields with a different inferred type.
    /// </summary>
    public int ChangedTypes => Entry.TypeChangedFieldCount;

    /// <summary>
    /// Gets a value indicating whether the entry contains any reported difference.
    /// </summary>
    public bool HasDifferences => Entry.HasDifferences;

    /// <summary>
    /// Gets the multi-line sparse profile of both sides.
    /// </summary>
    public string ProfileText { get; }

    /// <summary>
    /// Gets the multi-line load diagnostics of both sides, or an empty string when none exist.
    /// </summary>
    public string DiagnosticsText { get; }

    private static string ResolveStatusText(TableComparisonEntry entry)
    {
        if (entry.HasLoadFailure)
        {
            return Strings.Get("CompareStatusLoadFailed");
        }

        if (entry.HasMalformedSide)
        {
            return Strings.Get("CompareStatusMalformed");
        }

        return entry.Status switch
        {
            TableComparisonStatus.LeftOnly => Strings.Get("CompareStatusLeftOnly"),
            TableComparisonStatus.RightOnly => Strings.Get("CompareStatusRightOnly"),
            _ => Strings.Get("CompareStatusBoth"),
        };
    }

    private static string BuildProfileText(TableComparisonEntry entry)
    {
        var lines = new List<string>();
        AppendSide(lines, "Left", entry.Left);
        AppendSide(lines, "Right", entry.Right);
        return string.Join(Environment.NewLine, lines);
    }

    private static void AppendSide(
        List<string> lines,
        string labelKey,
        TableSideComparison? side)
    {
        var label = Strings.Get($"CompareSide{labelKey}");
        if (side is null)
        {
            lines.Add(Strings.Format("CompareProfileMissing", label));
            return;
        }

        if (side.Snapshot is null)
        {
            lines.Add(Strings.Format("CompareProfileUnreadable", label, ResolveSideStatus(side.Status)));
            if (!string.IsNullOrWhiteSpace(side.ErrorText))
            {
                lines.Add(side.ErrorText);
            }

            return;
        }

        var snapshot = side.Snapshot;
        lines.Add(Strings.Format("CompareRootRecordLine", label, snapshot.RootElement, snapshot.RecordElement));
        var overlays = snapshot.OverlayLanguages.Count == 0
            ? Strings.Get("CompareOverlaysNone")
            : string.Join(", ", snapshot.OverlayLanguages);
        lines.Add(Strings.Format(
            "CompareProfileLine",
            label,
            snapshot.RecordCount,
            snapshot.FieldCount,
            snapshot.DistinctRecordShapes,
            snapshot.MinFieldsPerRecord,
            snapshot.MaxFieldsPerRecord,
            snapshot.AverageFieldsPerRecord.ToString("0.0", CultureInfo.CurrentCulture),
            snapshot.EmptyRecordCount,
            overlays));
    }

    private static string ResolveSideStatus(TableSideStatus status) => status switch
    {
        TableSideStatus.Malformed => Strings.Get("CompareStatusMalformed"),
        TableSideStatus.Failed => Strings.Get("CompareStatusLoadFailed"),
        TableSideStatus.LoadedReadOnly => Strings.Get("CompareStatusReadOnly"),
        _ => Strings.Get("CompareStatusLoaded"),
    };

    private static string BuildDiagnosticsText(TableComparisonEntry entry)
    {
        var lines = new List<string>();
        AppendDiagnostics(lines, "Left", entry.Left);
        AppendDiagnostics(lines, "Right", entry.Right);
        return string.Join(Environment.NewLine, lines);
    }

    private static void AppendDiagnostics(List<string> lines, string labelKey, TableSideComparison? side)
    {
        if (side is null || side.Diagnostics.Count == 0)
        {
            return;
        }

        var label = Strings.Get($"CompareSide{labelKey}");
        foreach (var diagnostic in side.Diagnostics)
        {
            var location = diagnostic.Line is int line
                ? $" (line {line.ToString(CultureInfo.CurrentCulture)}, position "
                  + $"{(diagnostic.Position ?? 1).ToString(CultureInfo.CurrentCulture)})"
                : string.Empty;
            lines.Add($"{label}: {diagnostic.Code}: {diagnostic.Message}{location}");
        }
    }

    private static string FormatCount(int? count) =>
        count?.ToString(CultureInfo.CurrentCulture) ?? "—";
}

/// <summary>
/// Display projection of one <see cref="FieldComparison"/>.
/// </summary>
public sealed class ComparisonFieldItem
{
    /// <summary>
    /// Initializes a new item for a field comparison.
    /// </summary>
    /// <param name="field">Field comparison to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="field"/> is null.</exception>
    public ComparisonFieldItem(FieldComparison field)
    {
        ArgumentNullException.ThrowIfNull(field);
        Name = field.Name;
        StatusText = Strings.Get($"CompareField{field.Status}");
        LeftTypeText = FormatType(field.LeftType);
        RightTypeText = FormatType(field.RightType);
        LeftPresenceText = FormatPresence(field.LeftPresence, field.LeftMaxOccurrences, field.LeftType);
        RightPresenceText = FormatPresence(field.RightPresence, field.RightMaxOccurrences, field.RightType);
        HasDifference = field.Status != FieldComparisonStatus.Same;
    }

    /// <summary>
    /// Gets the XML field name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the localized difference kind.
    /// </summary>
    public string StatusText { get; }

    /// <summary>
    /// Gets the localized left type, or a dash when the field is absent.
    /// </summary>
    public string LeftTypeText { get; }

    /// <summary>
    /// Gets the localized right type, or a dash when the field is absent.
    /// </summary>
    public string RightTypeText { get; }

    /// <summary>
    /// Gets the left presence count with the maximum occurrence count when it is repeated.
    /// </summary>
    public string LeftPresenceText { get; }

    /// <summary>
    /// Gets the right presence count with the maximum occurrence count when it is repeated.
    /// </summary>
    public string RightPresenceText { get; }

    /// <summary>
    /// Gets a value indicating whether the field differs between the data sets.
    /// </summary>
    public bool HasDifference { get; }

    private static string FormatType(ColumnDataType? type) => type switch
    {
        null => "—",
        ColumnDataType.String => Strings.Get("CompareTypeString"),
        ColumnDataType.Integer => Strings.Get("CompareTypeInteger"),
        ColumnDataType.Decimal => Strings.Get("CompareTypeDecimal"),
        ColumnDataType.Boolean => Strings.Get("CompareTypeBoolean"),
        ColumnDataType.Lookup => Strings.Get("CompareTypeLookup"),
        ColumnDataType.Flags => Strings.Get("CompareTypeFlags"),
        ColumnDataType.Nested => Strings.Get("CompareTypeNested"),
        _ => Strings.Get("CompareTypeRaw"),
    };

    private static string FormatPresence(int presence, int maxOccurrences, ColumnDataType? type)
    {
        if (type is null)
        {
            return "—";
        }

        var text = presence.ToString(CultureInfo.CurrentCulture);
        return maxOccurrences > 1
            ? $"{text} (×{maxOccurrences.ToString(CultureInfo.CurrentCulture)})"
            : text;
    }
}

/// <summary>
/// Display projection of one <see cref="LookupReferenceComparison"/>.
/// </summary>
public sealed class ComparisonLookupItem
{
    /// <summary>
    /// Initializes a new item for a lookup reference.
    /// </summary>
    /// <param name="reference">Lookup reference comparison to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="reference"/> is null.</exception>
    public ComparisonLookupItem(LookupReferenceComparison reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        TableName = reference.Table.Value;
        FieldName = reference.FieldName;
        LookupTable = reference.LookupTable;
        LeftText = FormatAvailability(reference.LeftAvailability);
        RightText = FormatAvailability(reference.RightAvailability);
        HasProblem = reference.LeftAvailability != LookupAvailability.Available
            || reference.RightAvailability != LookupAvailability.Available;
    }

    /// <summary>
    /// Gets the table that declares the lookup column.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Gets the field name of the lookup column.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the referenced lookup table name.
    /// </summary>
    public string LookupTable { get; }

    /// <summary>
    /// Gets the localized left availability label.
    /// </summary>
    public string LeftText { get; }

    /// <summary>
    /// Gets the localized right availability label.
    /// </summary>
    public string RightText { get; }

    /// <summary>
    /// Gets a value indicating whether the lookup is missing or unreadable on at least one side.
    /// </summary>
    public bool HasProblem { get; }

    private static string FormatAvailability(LookupAvailability availability) => availability switch
    {
        LookupAvailability.Missing => Strings.Get("CompareLookupMissing"),
        LookupAvailability.Unreadable => Strings.Get("CompareLookupUnreadable"),
        _ => Strings.Get("CompareLookupAvailable"),
    };
}
