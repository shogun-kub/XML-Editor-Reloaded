using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// One nested record of a workspace row, for example an <c>ASSIGNMENT</c> of a facility type. The record
/// carries its own element and the key resolved from the nested key segment.
/// </summary>
public sealed class NestedWorkspaceRecord
{
    internal NestedWorkspaceRecord(int segmentIndex, int index, string? key, TableRecord record)
    {
        SegmentIndex = segmentIndex;
        Index = index;
        Key = key;
        Record = record;
    }

    /// <summary>
    /// Gets the key-segment index this record belongs to, where the primary segment is zero.
    /// </summary>
    public int SegmentIndex { get; }

    /// <summary>
    /// Gets the zero-based occurrence of the record among its siblings, used for base-table edits.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the resolved nested key, or <see langword="null"/> when the key field is absent.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    /// Gets the key formatted for display, falling back to the occurrence number.
    /// </summary>
    public string KeyDisplay => string.IsNullOrEmpty(Key) ? $"#{Index + 1}" : Key;

    /// <summary>
    /// Gets the nested record facade over its XML element.
    /// </summary>
    public TableRecord Record { get; }
}
