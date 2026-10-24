using System.Globalization;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// An item row shown in the image viewer: its index, name, and graphic identity.
/// </summary>
public sealed class ItemGraphicsOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemGraphicsOption"/> class.
    /// </summary>
    /// <param name="index">Item index (<c>uiIndex</c>).</param>
    /// <param name="name">Item name (<c>szItemName</c>), or an empty string.</param>
    /// <param name="graphicType">Graphic type (<c>ubGraphicType</c>), or <see langword="null"/> when absent or invalid.</param>
    /// <param name="graphicNum">Graphic index (<c>ubGraphicNum</c>), or <see langword="null"/> when absent or invalid.</param>
    public ItemGraphicsOption(int index, string name, int? graphicType, int? graphicNum)
    {
        ArgumentNullException.ThrowIfNull(name);
        Index = index;
        Name = name;
        GraphicType = graphicType;
        GraphicNum = graphicNum;
    }

    /// <summary>
    /// Gets the item index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the item name, possibly empty.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the graphic type, or <see langword="null"/> when the record does not define a valid one.
    /// A missing type is treated as the gun sheet (zero) by the viewer.
    /// </summary>
    public int? GraphicType { get; }

    /// <summary>
    /// Gets the graphic index, or <see langword="null"/> when the record does not define a valid one.
    /// </summary>
    public int? GraphicNum { get; }

    /// <summary>
    /// Gets the text shown in the item selector, for example <c>311: String</c>.
    /// </summary>
    public string DisplayText => Name.Length == 0
        ? Index.ToString(CultureInfo.CurrentCulture)
        : $"{Index.ToString(CultureInfo.CurrentCulture)}: {Name}";

    /// <summary>
    /// Creates an option from an item record without requiring metadata.
    /// </summary>
    /// <param name="record">Item record.</param>
    /// <param name="fallbackIndex">Index used when <c>uiIndex</c> is absent or invalid.</param>
    /// <returns>The projected item option.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is null.</exception>
    public static ItemGraphicsOption FromRecord(TableRecord record, int fallbackIndex)
    {
        ArgumentNullException.ThrowIfNull(record);

        return new ItemGraphicsOption(
            ParseNonNegative(record.GetRawValue("uiIndex")) ?? fallbackIndex,
            record.GetRawValue("szItemName") ?? string.Empty,
            ParseNonNegative(record.GetRawValue("ubGraphicType")),
            ParseNonNegative(record.GetRawValue("ubGraphicNum")));
    }

    private static int? ParseNonNegative(string? rawText) =>
        int.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value >= 0
            ? value
            : null;
}
