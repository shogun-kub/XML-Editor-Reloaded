using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Copy;

/// <summary>
/// Defines how a copied record is handled when the target table already contains the same key or, for a
/// lookup row whose key is absent, a row with the same comparison field value.
/// </summary>
public enum TableCopyConflictPolicy
{
    /// <summary>Leave the existing target record unchanged and copy nothing.</summary>
    Skip,

    /// <summary>Copy every source field over the existing target record, keeping target-only fields.</summary>
    Overwrite,

    /// <summary>Copy only source fields that are absent from the existing target record.</summary>
    Merge,
}

/// <summary>
/// Defines how the target key of a copied record is chosen.
/// </summary>
public enum TableCopyKeyMode
{
    /// <summary>Keep the source key and resolve collisions through <see cref="TableCopyRequest.ConflictPolicy"/>.</summary>
    Preserve,

    /// <summary>Allocate a fresh key in the target table for every copied record.</summary>
    Allocate,
}

/// <summary>
/// Action planned for one copied record.
/// </summary>
public enum TableCopyAction
{
    /// <summary>The record is new in the target table.</summary>
    Add,

    /// <summary>The record exists and its fields are overwritten.</summary>
    Overwrite,

    /// <summary>The record exists and only its missing fields are filled.</summary>
    Merge,

    /// <summary>The record exists and is left unchanged.</summary>
    Skip,
}

/// <summary>
/// Outcome of resolving one reference through display text.
/// </summary>
public enum TableCopyRemapStatus
{
    /// <summary>The reference was found in both data sets and the target key differs from the source key.</summary>
    Remapped,

    /// <summary>Several target records share the display text, so the source key is kept unchanged.</summary>
    Ambiguous,

    /// <summary>The display text has no match in the target data set, so the source key is kept unchanged.</summary>
    NotFound,
}

/// <summary>
/// A cross-data-set copy request for one table.
/// </summary>
/// <param name="SourceTableDataDirectory">Absolute <c>TableData</c> directory that is read only.</param>
/// <param name="TargetTableDataDirectory">Absolute <c>TableData</c> directory that receives the records.</param>
/// <param name="TableName">Table copied from the source to the target; both directories must contain it.</param>
/// <param name="RecordKeys">
/// Source record keys to copy, or <see langword="null"/> to copy every record of the table.
/// </param>
/// <param name="ConflictPolicy">
/// Handling of records whose key already exists in the target, or whose comparison field value matches
/// an existing target row of a lookup table.
/// </param>
/// <param name="KeyMode">Whether to preserve source keys or allocate fresh target keys.</param>
/// <param name="CopyRelatedRecords">
/// When <see langword="true"/>, records linked to the copied records through sub-table and child-row
/// relations are copied too, for example a weapon row and attachment rows of an item.
/// </param>
public sealed record TableCopyRequest(
    string SourceTableDataDirectory,
    string TargetTableDataDirectory,
    TableName TableName,
    IReadOnlyList<string>? RecordKeys = null,
    TableCopyConflictPolicy ConflictPolicy = TableCopyConflictPolicy.Skip,
    TableCopyKeyMode KeyMode = TableCopyKeyMode.Preserve,
    bool CopyRelatedRecords = false);

/// <summary>
/// One reference value that is rewritten because the same display text has a different identifier in
/// the target data set.
/// </summary>
/// <param name="Field">XML field name that stores the reference.</param>
/// <param name="SourceValue">Reference value in the source record.</param>
/// <param name="TargetValue">Value written to the target record; equals the source value when unresolved.</param>
/// <param name="DisplayText">Display text used for matching, or <see langword="null"/> when unknown.</param>
/// <param name="Status">Resolution outcome.</param>
public sealed record TableCopyRemap(
    string Field,
    string SourceValue,
    string TargetValue,
    string? DisplayText,
    TableCopyRemapStatus Status);

/// <summary>
/// Preview of the related records that accompany one copied record.
/// </summary>
/// <param name="Table">Related table.</param>
/// <param name="Kind">Relation kind that produced the planned rows.</param>
/// <param name="RecordCount">Number of related source records that will be processed.</param>
/// <param name="AddCount">Number of rows that will be appended.</param>
/// <param name="UpdateCount">Number of existing rows that will be overwritten or merged.</param>
/// <param name="SkippedCount">Number of rows that will not be copied.</param>
/// <param name="RemapCount">Number of reference values that will be rewritten in the related rows.</param>
/// <param name="Warnings">Non-fatal problems detected while planning the related rows.</param>
public sealed record TableCopyRelatedPreview(
    TableName Table,
    TableRelationKind Kind,
    int RecordCount,
    int AddCount,
    int UpdateCount,
    int SkippedCount,
    int RemapCount,
    IReadOnlyList<string> Warnings);

/// <summary>
/// Preview of one source record.
/// </summary>
/// <param name="SourceKey">Primary key in the source table, or an empty string when the table has no key.</param>
/// <param name="SourceDisplay">Display text of the source record, or <see langword="null"/>.</param>
/// <param name="TargetKey">Key the record will have in the target, or <see langword="null"/> when it cannot be computed.</param>
/// <param name="Action">Planned action.</param>
/// <param name="Remaps">Reference values that will be rewritten while copying.</param>
/// <param name="Related">Related-row plans; empty when related copying is disabled.</param>
/// <param name="Warnings">Non-fatal problems detected for this record.</param>
public sealed record TableCopyRecordPreview(
    string SourceKey,
    string? SourceDisplay,
    string? TargetKey,
    TableCopyAction Action,
    IReadOnlyList<TableCopyRemap> Remaps,
    IReadOnlyList<TableCopyRelatedPreview> Related,
    IReadOnlyList<string> Warnings);

/// <summary>
/// Result of planning a cross-data-set copy without writing any file.
/// </summary>
/// <param name="TableName">Table being copied.</param>
/// <param name="SourceTableDataDirectory">Source <c>TableData</c> directory, never modified.</param>
/// <param name="TargetTableDataDirectory">Target <c>TableData</c> directory.</param>
/// <param name="Records">Per-record plans in source document order.</param>
/// <param name="Warnings">Non-fatal problems, including missing requested keys.</param>
public sealed record TableCopyPreview(
    TableName TableName,
    string SourceTableDataDirectory,
    string TargetTableDataDirectory,
    IReadOnlyList<TableCopyRecordPreview> Records,
    IReadOnlyList<string> Warnings)
{
    /// <summary>
    /// Gets the number of records that will be appended.
    /// </summary>
    public int AddCount => Records.Count(static record => record.Action == TableCopyAction.Add);

    /// <summary>
    /// Gets the number of records that will be overwritten.
    /// </summary>
    public int OverwriteCount => Records.Count(static record => record.Action == TableCopyAction.Overwrite);

    /// <summary>
    /// Gets the number of records that will be merged into existing rows.
    /// </summary>
    public int MergeCount => Records.Count(static record => record.Action == TableCopyAction.Merge);

    /// <summary>
    /// Gets the number of records that will be skipped.
    /// </summary>
    public int SkipCount => Records.Count(static record => record.Action == TableCopyAction.Skip);

    /// <summary>
    /// Gets the number of related rows that will be added or updated.
    /// </summary>
    public int RelatedRecordCount
    {
        get
        {
            var count = 0;
            foreach (var record in Records)
            {
                foreach (var related in record.Related)
                {
                    count += related.AddCount + related.UpdateCount;
                }
            }

            return count;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the plan writes anything to the target.
    /// </summary>
    public bool HasChanges => AddCount + OverwriteCount + MergeCount + RelatedRecordCount > 0;
}

/// <summary>
/// Apply summary for one table written by a copy operation.
/// </summary>
/// <param name="Table">Table that was written.</param>
/// <param name="Added">Number of appended records.</param>
/// <param name="Updated">Number of existing records that were overwritten or merged.</param>
/// <param name="Skipped">Number of records left unchanged.</param>
public sealed record TableCopyAppliedTable(
    TableName Table,
    int Added,
    int Updated,
    int Skipped);

/// <summary>
/// Result of a committed cross-data-set copy.
/// </summary>
/// <param name="TableName">Primary table that was copied.</param>
/// <param name="Tables">Per-table apply summaries, with the primary table first.</param>
/// <param name="SavedFiles">Absolute paths of the files that were written.</param>
/// <param name="Warnings">Non-fatal problems collected while planning and applying.</param>
public sealed record TableCopyResult(
    TableName TableName,
    IReadOnlyList<TableCopyAppliedTable> Tables,
    IReadOnlyList<string> SavedFiles,
    IReadOnlyList<string> Warnings);
