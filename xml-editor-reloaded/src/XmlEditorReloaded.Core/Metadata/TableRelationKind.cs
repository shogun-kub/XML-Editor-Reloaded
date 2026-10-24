namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Describes how a relation links a source table to a target table.
/// </summary>
public enum TableRelationKind
{
    /// <summary>
    /// The source field stores a target key, for example <c>Items.Attachments.itemIndex</c> pointing at
    /// <c>Items.Items.uiIndex</c>.
    /// </summary>
    Reference,

    /// <summary>
    /// The target table holds one row per source record, matched by a key field. The source field is
    /// the key that is matched, for example <c>Items.Items.ubClassIndex</c> to
    /// <c>Items.Magazines.uiIndex</c>.
    /// </summary>
    SubTable,

    /// <summary>
    /// The target table holds child rows that reference the source record. The target value field is
    /// the field in the target table that stores the source key.
    /// </summary>
    ChildRows,

    /// <summary>
    /// The target table contains records that point back at the source record. The relation is a
    /// read-only navigation view of incoming references.
    /// </summary>
    Reverse,
}
