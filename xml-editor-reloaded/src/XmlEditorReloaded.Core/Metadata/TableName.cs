namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Identifies a table by its dotted name, for example <c>Items.Items</c> or <c>Lookup.ItemClass</c>.
/// </summary>
/// <param name="Value">The dotted table name as stored in metadata and shown in the UI.</param>
public readonly record struct TableName(string Value) : IComparable<TableName>
{
    /// <summary>
    /// Gets the dotted table name, for example <c>Items.Items</c>.
    /// </summary>
    public string Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    /// <summary>
    /// Converts dots to directory separators and appends <c>.xml</c>, producing the path
    /// of the table file relative to the <c>TableData</c> directory.
    /// </summary>
    /// <returns>For example <c>Items/Items.xml</c> for <c>Items.Items</c>.</returns>
    public string ToRelativePath() => Value.Replace('.', '/') + ".xml";

    /// <summary>
    /// Parses a dotted table name from arbitrary text.
    /// </summary>
    /// <param name="value">The dotted name, for example <c>Items.Items</c>.</param>
    /// <returns>A parsed table name.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, or whitespace.</exception>
    public static TableName Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new TableName(value.Trim());
    }

    /// <summary>
    /// Attempts to derive a dotted table name from a file path relative to the <c>TableData</c> directory.
    /// </summary>
    /// <param name="relativePath">Relative path such as <c>Items/Items.xml</c>.</param>
    /// <param name="tableName">The parsed table name when the path is valid.</param>
    /// <returns><see langword="true"/> when the path ends with <c>.xml</c>; otherwise <see langword="false"/>.</returns>
    public static bool TryFromRelativePath(string relativePath, out TableName tableName)
    {
        tableName = default;
        if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var withoutExtension = relativePath[..^4];
        var normalized = withoutExtension.Replace('\\', '/').Replace('/', '.');
        if (normalized.Length == 0 || normalized.StartsWith('.') || normalized.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        tableName = new TableName(normalized);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(TableName other) => string.CompareOrdinal(Value, other.Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}
