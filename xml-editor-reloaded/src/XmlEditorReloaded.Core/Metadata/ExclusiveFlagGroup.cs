namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Declares a set of boolean fields where at most one field may be set. The rule is used by
/// validation only: existing data with several set fields is reported, never rewritten.
/// </summary>
public sealed class ExclusiveFlagGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExclusiveFlagGroup"/> class.
    /// </summary>
    /// <param name="groupKey">Stable key that identifies the group within the table.</param>
    /// <param name="fieldNames">Direct-child XML fields that compete for one choice.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="groupKey"/> is empty or <paramref name="fieldNames"/> is empty.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldNames"/> is null.</exception>
    public ExclusiveFlagGroup(string groupKey, IReadOnlyList<string> fieldNames)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupKey);
        ArgumentNullException.ThrowIfNull(fieldNames);
        if (fieldNames.Count == 0)
        {
            throw new ArgumentException("An exclusive flag group needs at least one field.", nameof(fieldNames));
        }

        GroupKey = groupKey;
        FieldNames = fieldNames;
    }

    /// <summary>
    /// Gets the stable group key, for example <c>launcher_type</c>.
    /// </summary>
    public string GroupKey { get; }

    /// <summary>
    /// Gets the ordered direct-child XML fields of the group.
    /// </summary>
    public IReadOnlyList<string> FieldNames { get; }
}
