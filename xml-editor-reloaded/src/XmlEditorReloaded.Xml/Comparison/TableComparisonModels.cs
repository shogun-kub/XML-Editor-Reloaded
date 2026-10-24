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
    /// load problems, root or record element names, record counts, overlays, or field shapes.
    /// Value differences inside cells are intentionally not compared.
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
        || Fields.Any(static candidate => candidate.Status != FieldComparisonStatus.Same);
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
    long LeftRecordTotal,
    long RightRecordTotal);

/// <summary>
/// Read-only cross-version comparison of two <c>TableData</c> directories.
/// The report compares table presence, record counts, field sets, inferred types, lookup
/// references, and sparse profiles. Cell values are not compared and nothing is written to
/// either data set.
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
