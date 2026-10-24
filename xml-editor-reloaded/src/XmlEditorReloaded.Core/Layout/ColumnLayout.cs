namespace XmlEditorReloaded.Core.Layout;

/// <summary>
/// Persisted presentation state of one grid column.
/// </summary>
public sealed class ColumnLayout
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnLayout"/> class.
    /// </summary>
    /// <param name="name">XML field name of the column, matched case-sensitively.</param>
    /// <param name="isVisible">Whether the column is shown in the grid.</param>
    /// <param name="width">Last user-resized width in device-independent pixels.</param>
    /// <param name="displayOrder">Zero-based position in the grid.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty.</exception>
    public ColumnLayout(string name, bool isVisible, double width, int displayOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        IsVisible = isVisible;
        Width = width;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Gets the XML field name of the column, matched case-sensitively.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the column is shown in the grid.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// Gets the last user-resized width in device-independent pixels.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Gets the zero-based position in the grid.
    /// </summary>
    public int DisplayOrder { get; }
}
