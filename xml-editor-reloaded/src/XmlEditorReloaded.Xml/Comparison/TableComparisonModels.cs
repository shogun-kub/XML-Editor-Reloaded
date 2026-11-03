using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Availability of a table file in the two compared data sets.
/// </summary>
public enum TableComparisonStatus
{
    /// <summary>The table exists in both data sets.</summary>
    Both,

    /// <summary>The table exists only in the left (baseline) data set.</summary>
    LeftOnly,

    /// <summary>The table exists only in the right (candidate) data set.</summary>
    RightOnly,
}

/// <summary>
/// Outcome of loading one side of a compared table.
/// </summary>
public enum TableSideStatus
{
    /// <summary>The file parsed and is editable.</summary>
    Loaded,

    /// <summary>The file parsed but was flagged read-only.</summary>
    LoadedReadOnly,

    /// <summary>The file is not well-formed XML; no snapshot is available.</summary>
    Malformed,

    /// <summary>The file could not be read; no snapshot is available.</summary>
    Failed,
}

/// <summary>
/// Result of comparing one field between the two data sets.
/// </summary>
public enum FieldComparisonStatus
{
    /// <summary>The field exists on both sides with the same inferred type and cardinality.</summary>
    Same,

    /// <summary>The field is present only in the left (baseline) data set.</summary>
    LeftOnly,

    /// <summary>The field is present only in the right (candidate) data set.</summary>
    RightOnly,

    /// <summary>The field exists on both sides but its inferred value type differs.</summary>
    TypeChanged,

    /// <summary>
    /// The field exists on both sides with the same inferred type but becomes a repeated element
    /// on one side only.
    /// </summary>
    CardinalityChanged,
}

/// <summary>
/// Kind of difference reported for a record, a field value, a root singleton field, or an overlay cell.
/// </summary>
public enum ComparisonDifferenceKind
{
    /// <summary>The item exists only in the left (baseline) data set.</summary>
    LeftOnly,

    /// <summary>The item exists only in the right (candidate) data set.</summary>
    RightOnly,

    /// <summary>The item exists on both sides with a different value.</summary>
    Changed,
}

/// <summary>
/// How records of one table were matched between the two data sets.
/// </summary>
public enum RecordIdentityKind
{
    /// <summary>Records were matched by the record key declared in the table metadata.</summary>
    DeclaredKey,

    /// <summary>The table declares no record key, so records were matched by their order.</summary>
    Positional,
}

/// <summary>
/// Presence of one language overlay file on one side of the comparison.
/// </summary>
public enum OverlayComparisonStatus
{
    /// <summary>Only the left (baseline) data set has an overlay for this language.</summary>
    LeftOnly,

    /// <summary>Only the right (candidate) data set has an overlay for this language.</summary>
    RightOnly,

    /// <summary>Both data sets have an overlay for this language.</summary>
    Both,
}

/// <summary>
/// Load state of one overlay file.
/// </summary>
public enum OverlayFileStatus
{
    /// <summary>The overlay file does not exist in this data set.</summary>
    Missing,

    /// <summary>The overlay file parsed and its content was compared.</summary>
    Loaded,

    /// <summary>The overlay file is not well-formed XML; its content was not compared.</summary>
    Malformed,

    /// <summary>The overlay file could not be read; its content was not compared.</summary>
    Unreadable,
}

/// <summary>
/// Availability of a lookup table referenced by a column.
/// </summary>
public enum LookupAvailability
{
    /// <summary>The lookup table file does not exist in this data set.</summary>
    Missing,

    /// <summary>The lookup table file exists but cannot be parsed or read.</summary>
    Unreadable,

    /// <summary>The lookup table file exists and was parsed successfully.</summary>
    Available,
}

/// <summary>
/// Structural and sparse profile of one side of a compared table.
/// </summary>
/// <param name="RootElement">Name of the document root element.</param>
/// <param name="RecordElement">Name of the element treated as one record.</param>
/// <param name="RecordCount">Number of records found.</param>
/// <param name="FieldCount">Number of distinct direct child element names across all records.</param>
/// <param name="DistinctRecordShapes">Number of distinct field-name sets across all records.</param>
/// <param name="MinFieldsPerRecord">Smallest number of direct child elements in a record.</param>
/// <param name="MaxFieldsPerRecord">Largest number of direct child elements in a record.</param>
/// <param name="AverageFieldsPerRecord">Mean number of direct child elements per record.</param>
/// <param name="EmptyRecordCount">Number of records without any child elements.</param>
/// <param name="OverlayLanguages">Language codes with an overlay file next to this table, sorted ordinally.</param>
public sealed record TableComparisonSide(
    string RootElement,
    string RecordElement,
    int RecordCount,
    int FieldCount,
    int DistinctRecordShapes,
    int MinFieldsPerRecord,
    int MaxFieldsPerRecord,
    double AverageFieldsPerRecord,
    int EmptyRecordCount,
    IReadOnlyList<string> OverlayLanguages);

/// <summary>
/// Load outcome and structural snapshot for one side of a compared table.
/// </summary>
/// <param name="Status">Load status of the side.</param>
/// <param name="Snapshot">Structural snapshot, or <see langword="null"/> when the file could not be analyzed.</param>
/// <param name="Diagnostics">Diagnostics produced while loading the file.</param>
/// <param name="ErrorText">Operation error text when the file could not be read; otherwise <see langword="null"/>.</param>
public sealed record TableSideComparison(
    TableSideStatus Status,
    TableComparisonSide? Snapshot,
    IReadOnlyList<LoadDiagnostic> Diagnostics,
    string? ErrorText);

/// <summary>
/// Comparison of one field between the left and right data sets.
/// </summary>
/// <param name="Name">Case-sensitive XML element name.</param>
/// <param name="Status">Kind of difference found.</param>
/// <param name="LeftType">Inferred type on the left side, or <see langword="null"/> when the field is absent.</param>
/// <param name="RightType">Inferred type on the right side, or <see langword="null"/> when the field is absent.</param>
/// <param name="LeftPresence">Number of left records that contain the field.</param>
/// <param name="RightPresence">Number of right records that contain the field.</param>
/// <param name="LeftMaxOccurrences">Maximum occurrences of the field within one left record.</param>
/// <param name="RightMaxOccurrences">Maximum occurrences of the field within one right record.</param>
public sealed record FieldComparison(
    string Name,
    FieldComparisonStatus Status,
    ColumnDataType? LeftType,
    ColumnDataType? RightType,
    int LeftPresence,
    int RightPresence,
    int LeftMaxOccurrences,
    int RightMaxOccurrences);

/// <summary>
/// One field value that differs between the two data sets.
/// </summary>
/// <param name="Name">Field name; root singleton fields use a path such as <c>COUNTRY/countryName</c>.</param>
/// <param name="Kind">Kind of difference.</param>
/// <param name="LeftValue">Left value, or <see langword="null"/> when the field is absent on the left.</param>
/// <param name="RightValue">Right value, or <see langword="null"/> when the field is absent on the right.</param>
/// <remarks>
/// Values are the exact XML text of leaf elements; nested elements are rendered with their markup.
/// Repeated fields join their occurrences with a newline. A value longer than
/// <see cref="TableComparisonLimits.MaxValueLength"/> is truncated so the report stays bounded.
/// </remarks>
public sealed record RecordFieldChange(
    string Name,
    ComparisonDifferenceKind Kind,
    string? LeftValue,
    string? RightValue);

/// <summary>
/// Comparison of one record present on at least one side with a different value set.
/// </summary>
/// <param name="Key">User-facing record key; a duplicate key carries an occurrence suffix such as <c>5 #2</c>.</param>
/// <param name="LeftOrder">Zero-based record position on the left, or <c>-1</c> when the record is absent.</param>
/// <param name="RightOrder">Zero-based record position on the right, or <c>-1</c> when the record is absent.</param>
/// <param name="Kind">Kind of difference at the record level.</param>
/// <param name="Fields">
/// Differing fields of a changed record, in left-first-appearance order followed by right-only fields.
/// The list is empty for an added or a removed record so the report stays bounded.
/// </param>
public sealed record RecordComparison(
    string Key,
    int LeftOrder,
    int RightOrder,
    ComparisonDifferenceKind Kind,
    IReadOnlyList<RecordFieldChange> Fields)
{
    /// <summary>
    /// Gets the number of differing fields of a changed record; zero for an added or a removed record.
    /// </summary>
    public int ChangedFieldCount => Fields.Count;
}

/// <summary>
/// Value-level comparison of one table: record presence, changed field values, and root singleton fields.
/// </summary>
/// <param name="IdentityKind">How records were matched between the data sets.</param>
/// <param name="AddedRecordCount">Records present only on the right, counted even when details are capped.</param>
/// <param name="RemovedRecordCount">Records present only on the left, counted even when details are capped.</param>
/// <param name="ChangedRecordCount">Records present on both sides with at least one differing field.</param>
/// <param name="ChangedFieldCount">
/// Number of differing field values across changed records; fields added to or removed from a record are
/// included.
/// </param>
/// <param name="RecordsTruncated">
/// Whether the detailed record and field lists were capped by
/// <see cref="TableComparisonLimits.MaxRecordDetailsPerTable"/> or
/// <see cref="TableComparisonLimits.MaxFieldDetailsPerTable"/>. The counts remain exact.
/// </param>
/// <param name="Records">Differing records; identical records are not reported.</param>
/// <param name="RootFields">Differing root-level singleton fields such as <c>COUNTRY/countryName</c>.</param>
public sealed record TableValueComparison(
    RecordIdentityKind IdentityKind,
    int AddedRecordCount,
    int RemovedRecordCount,
    int ChangedRecordCount,
    int ChangedFieldCount,
    bool RecordsTruncated,
    IReadOnlyList<RecordComparison> Records,
    IReadOnlyList<RecordFieldChange> RootFields)
{
    /// <summary>
    /// Gets an empty value comparison, used when a side is absent or could not be analyzed.
    /// </summary>
    public static TableValueComparison Empty { get; } = new(
        RecordIdentityKind.Positional,
        0,
        0,
        0,
        0,
        false,
        Array.Empty<RecordComparison>(),
        Array.Empty<RecordFieldChange>());

    /// <summary>
    /// Gets a value indicating whether any record or root singleton field differs.
    /// </summary>
    public bool HasDifferences =>
        AddedRecordCount > 0
        || RemovedRecordCount > 0
        || ChangedRecordCount > 0
        || RootFields.Count > 0;

    /// <summary>
    /// Gets the number of differing records regardless of whether their details were capped.
    /// </summary>
    public int RecordDifferenceCount => AddedRecordCount + RemovedRecordCount + ChangedRecordCount;

    /// <summary>
    /// Gets the number of differing root singleton fields.
    /// </summary>
    public int RootFieldChangeCount => RootFields.Count;
}

/// <summary>
/// Load state and cell count of one language overlay file on one side.
/// </summary>
/// <param name="FilePath">Absolute overlay file path, or <see langword="null"/> when the overlay is absent.</param>
/// <param name="Status">Load state of the overlay file.</param>
/// <param name="ErrorText">Error text when the file is malformed or unreadable; otherwise <see langword="null"/>.</param>
/// <param name="CellCount">Number of leaf cells in the overlay; zero when the file was not parsed.</param>
public sealed record OverlaySide(
    string? FilePath,
    OverlayFileStatus Status,
    string? ErrorText,
    int CellCount)
{
    /// <summary>
    /// Gets a side that has no overlay file for this language.
    /// </summary>
    public static OverlaySide Missing { get; } = new(null, OverlayFileStatus.Missing, null, 0);
}

/// <summary>
/// One translated overlay cell that differs between the two data sets.
/// </summary>
/// <param name="Key">Record key, or the singleton path such as <c>COUNTRY</c> for a root-level cell.</param>
/// <param name="FieldPath">Field path relative to the record or singleton.</param>
/// <param name="Kind">Kind of difference.</param>
/// <param name="LeftValue">Left translated value, or <see langword="null"/> when the cell is absent on the left.</param>
/// <param name="RightValue">Right translated value, or <see langword="null"/> when the cell is absent on the right.</param>
public sealed record OverlayCellChange(
    string Key,
    string FieldPath,
    ComparisonDifferenceKind Kind,
    string? LeftValue,
    string? RightValue);

/// <summary>
/// Content comparison of one language overlay.
/// </summary>
/// <param name="LanguageCode">Overlay language code, for example <c>ru</c>.</param>
/// <param name="Status">Whether the overlay exists on one or both sides.</param>
/// <param name="Left">Load state of the left overlay file.</param>
/// <param name="Right">Load state of the right overlay file.</param>
/// <param name="AddedCellCount">Cells present only on the right; zero when a side could not be parsed.</param>
/// <param name="RemovedCellCount">Cells present only on the left; zero when a side could not be parsed.</param>
/// <param name="ChangedCellCount">Cells present on both sides with a different translation.</param>
/// <param name="CellsTruncated">
/// Whether the detailed cell list was capped by
/// <see cref="TableComparisonLimits.MaxOverlayCellDetailsPerLanguage"/>. The counts remain exact.
/// </param>
/// <param name="Cells">Differing overlay cells; empty when one side is missing, malformed, or unreadable.</param>
/// <remarks>
/// Cell-level differences are reported only when both overlay files parsed. An overlay that exists on
/// one side only is reported through <paramref name="Status"/> and the side states, without listing every
/// translated cell as removed.
/// </remarks>
public sealed record OverlayComparison(
    string LanguageCode,
    OverlayComparisonStatus Status,
    OverlaySide Left,
    OverlaySide Right,
    int AddedCellCount,
    int RemovedCellCount,
    int ChangedCellCount,
    bool CellsTruncated,
    IReadOnlyList<OverlayCellChange> Cells)
{
    /// <summary>
    /// Gets a value indicating whether the overlay presence, its load state, or its content differs.
    /// </summary>
    public bool HasDifferences =>
        Status != OverlayComparisonStatus.Both
        || Left.Status != OverlayFileStatus.Loaded
        || Right.Status != OverlayFileStatus.Loaded
        || AddedCellCount > 0
        || RemovedCellCount > 0
        || ChangedCellCount > 0;
}

/// <summary>
/// Caps applied while building a comparison report. Counts are always exact; only detail lists are capped.
/// </summary>
public static class TableComparisonLimits
{
    /// <summary>
    /// Maximum number of characters kept for one XML value in the report.
    /// </summary>
    public const int MaxValueLength = 300;

    /// <summary>
    /// Maximum number of record details kept per table.
    /// </summary>
    public const int MaxRecordDetailsPerTable = 2000;

    /// <summary>
    /// Maximum number of differing field values kept per table across all record details.
    /// </summary>
    public const int MaxFieldDetailsPerTable = 20000;

    /// <summary>
    /// Maximum number of differing overlay cells kept per language.
    /// </summary>
    public const int MaxOverlayCellDetailsPerLanguage = 2000;
}

/// <summary>
/// Comparison of one table file, including per-field and per-side results.
/// </summary>
/// <param name="TableName">Dotted table name derived from the relative path.</param>
/// <param name="RelativePath">Path relative to <c>TableData</c>, using forward slashes.</param>
/// <param name="Status">Whether the table exists on both sides or only on one.</param>
/// <param name="Left">Left-side load result, or <see langword="null"/> when the table is absent.</param>
/// <param name="Right">Right-side load result, or <see langword="null"/> when the table is absent.</param>
/// <param name="Fields">Field comparisons in left-first-appearance order followed by right-only fields.</param>
public sealed record TableComparisonEntry(
    TableName TableName,
    string RelativePath,
    TableComparisonStatus Status,
    TableSideComparison? Left,
    TableSideComparison? Right,
    IReadOnlyList<FieldComparison> Fields)
{
    /// <summary>
    /// Gets the value-level comparison of the table, including root singleton fields.
    /// </summary>
    public TableValueComparison Value { get; init; } = TableValueComparison.Empty;

    /// <summary>
    /// Gets the content comparison of the language overlays of the table, one entry per language.
    /// </summary>
    public IReadOnlyList<OverlayComparison> Overlays { get; init; } = Array.Empty<OverlayComparison>();

    /// <summary>
    /// Gets a value indicating whether at least one side of the table is malformed.
    /// </summary>
    public bool HasMalformedSide =>
        Left?.Status == TableSideStatus.Malformed || Right?.Status == TableSideStatus.Malformed;

    /// <summary>
    /// Gets a value indicating whether at least one side could not be read at all.
    /// </summary>
    public bool HasLoadFailure =>
        Left?.Status == TableSideStatus.Failed || Right?.Status == TableSideStatus.Failed;

    /// <summary>
    /// Gets a value indicating whether record values or root singleton fields differ.
    /// </summary>
    public bool HasValueDifferences => Value.HasDifferences;

    /// <summary>
    /// Gets a value indicating whether an overlay is absent, unreadable, or has different content.
    /// </summary>
    public bool HasOverlayDifferences =>
        Overlays.Any(static overlay => overlay.HasDifferences);

    /// <summary>
    /// Gets the number of fields that exist only in the right (candidate) data set.
    /// </summary>
    public int AddedFieldCount =>
        Fields.Count(static candidate => candidate.Status == FieldComparisonStatus.RightOnly);

    /// <summary>
    /// Gets the number of fields that exist only in the left (baseline) data set.
    /// </summary>
    public int RemovedFieldCount =>
        Fields.Count(static candidate => candidate.Status == FieldComparisonStatus.LeftOnly);

    /// <summary>
    /// Gets the number of fields whose inferred type differs between the data sets.
    /// </summary>
    public int TypeChangedFieldCount =>
        Fields.Count(static candidate => candidate.Status == FieldComparisonStatus.TypeChanged);

    /// <summary>
    /// Gets the number of fields whose cardinality differs between the data sets.
    /// </summary>
    public int CardinalityChangedFieldCount =>
        Fields.Count(static candidate => candidate.Status == FieldComparisonStatus.CardinalityChanged);

    /// <summary>
    /// Gets a value indicating whether the entry contains any reportable difference: presence,
    /// load problems, root or record element names, record counts, overlays, field shapes, record
    /// values, root singleton fields, or overlay content.
    /// </summary>
    public bool HasDifferences =>
        Status != TableComparisonStatus.Both
        || HasMalformedSide
        || HasLoadFailure
        || Left?.Snapshot is null
        || Right?.Snapshot is null
        || !string.Equals(Left.Snapshot.RootElement, Right.Snapshot.RootElement, StringComparison.Ordinal)
        || !string.Equals(Left.Snapshot.RecordElement, Right.Snapshot.RecordElement, StringComparison.Ordinal)
        || Left.Snapshot.RecordCount != Right.Snapshot.RecordCount
        || Left.Snapshot.FieldCount != Right.Snapshot.FieldCount
        || !Left.Snapshot.OverlayLanguages.SequenceEqual(Right.Snapshot.OverlayLanguages, StringComparer.Ordinal)
        || Fields.Any(static candidate => candidate.Status != FieldComparisonStatus.Same)
        || HasValueDifferences
        || HasOverlayDifferences;
}

/// <summary>
/// Availability of one lookup table reference in both data sets.
/// </summary>
/// <param name="Table">Table that declares the lookup column.</param>
/// <param name="FieldName">XML field name of the lookup column.</param>
/// <param name="LookupTable">Dotted name of the referenced lookup table.</param>
/// <param name="LeftAvailability">Availability of the lookup file in the left data set.</param>
/// <param name="RightAvailability">Availability of the lookup file in the right data set.</param>
public sealed record LookupReferenceComparison(
    TableName Table,
    string FieldName,
    string LookupTable,
    LookupAvailability LeftAvailability,
    LookupAvailability RightAvailability);

/// <summary>
/// Aggregate counts of a table comparison report.
/// </summary>
/// <param name="LeftTableCount">Number of base tables found in the left data set.</param>
/// <param name="RightTableCount">Number of base tables found in the right data set.</param>
/// <param name="CommonTableCount">Number of tables present in both data sets.</param>
/// <param name="LeftOnlyTableCount">Number of tables present only in the left data set.</param>
/// <param name="RightOnlyTableCount">Number of tables present only in the right data set.</param>
/// <param name="IdenticalTableCount">Number of common tables without reported differences.</param>
/// <param name="ChangedTableCount">Number of common tables with at least one reported difference.</param>
/// <param name="MalformedTableCount">Number of entries with at least one malformed side.</param>
/// <param name="FailedTableCount">Number of entries with at least one unreadable side.</param>
/// <param name="AddedFieldCount">Fields present only in the right data set among common tables.</param>
/// <param name="RemovedFieldCount">Fields present only in the left data set among common tables.</param>
/// <param name="TypeChangedFieldCount">Common-table fields with a different inferred type.</param>
/// <param name="CardinalityChangedFieldCount">Common-table fields that became or stopped being repeated elements.</param>
/// <param name="AddedRecordCount">Records present only in the right data set among common tables.</param>
/// <param name="RemovedRecordCount">Records present only in the left data set among common tables.</param>
/// <param name="ChangedRecordCount">Records present on both sides with at least one differing field.</param>
/// <param name="ChangedFieldCount">Differing field values across changed records.</param>
/// <param name="ChangedRootFieldCount">Differing root singleton fields, for example <c>COUNTRY/countryName</c>.</param>
/// <param name="DifferingOverlayCellCount">
/// Differing translated overlay cells across languages where both overlay files parsed.
/// </param>
/// <param name="LeftRecordTotal">Sum of record counts across left-side snapshots.</param>
/// <param name="RightRecordTotal">Sum of record counts across right-side snapshots.</param>
public sealed record TableComparisonSummary(
    int LeftTableCount,
    int RightTableCount,
    int CommonTableCount,
    int LeftOnlyTableCount,
    int RightOnlyTableCount,
    int IdenticalTableCount,
    int ChangedTableCount,
    int MalformedTableCount,
    int FailedTableCount,
    int AddedFieldCount,
    int RemovedFieldCount,
    int TypeChangedFieldCount,
    int CardinalityChangedFieldCount,
    int AddedRecordCount,
    int RemovedRecordCount,
    int ChangedRecordCount,
    int ChangedFieldCount,
    int ChangedRootFieldCount,
    int DifferingOverlayCellCount,
    long LeftRecordTotal,
    long RightRecordTotal);

/// <summary>
/// Read-only cross-version comparison of two <c>TableData</c> directories.
/// The report compares table presence, record counts, field sets, inferred types, lookup
/// references, sparse profiles, record values, root singleton fields, and overlay content.
/// Nothing is written to either data set.
/// </summary>
/// <param name="LeftDirectory">Absolute <c>TableData</c> directory of the baseline data set.</param>
/// <param name="RightDirectory">Absolute <c>TableData</c> directory of the candidate data set.</param>
/// <param name="GeneratedAtUtc">UTC timestamp of the comparison.</param>
/// <param name="Summary">Aggregate counts.</param>
/// <param name="Tables">Table entries ordered by table name.</param>
/// <param name="LookupReferences">Lookup column references and their availability on both sides.</param>
/// <param name="Warnings">Non-fatal report-level notes, for example missing metadata.</param>
public sealed record TableComparisonReport(
    string LeftDirectory,
    string RightDirectory,
    DateTime GeneratedAtUtc,
    TableComparisonSummary Summary,
    IReadOnlyList<TableComparisonEntry> Tables,
    IReadOnlyList<LookupReferenceComparison> LookupReferences,
    IReadOnlyList<string> Warnings);
