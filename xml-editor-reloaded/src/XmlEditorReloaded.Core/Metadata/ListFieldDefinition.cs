namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Describes a repeated child element of a record that maps to a logical list field,
/// for example <c>DrugEffect</c> appearing multiple times.
/// </summary>
public sealed class ListFieldDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListFieldDefinition"/> class.
    /// </summary>
    /// <param name="xmlFieldName">XML element name that may repeat inside the record.</param>
    public ListFieldDefinition(string xmlFieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldName);
        XmlFieldName = xmlFieldName;
    }

    /// <summary>
    /// Gets the XML element name that may repeat inside the record.
    /// </summary>
    public string XmlFieldName { get; }

    /// <summary>
    /// Gets or sets the resource key of a display name for UI grids.
    /// </summary>
    public string? DisplayNameKey { get; init; }
}

/// <summary>
/// Describes an indexed group of child elements whose names follow a mask,
/// for example <c>ItemCapacityPerSize%Index%</c> standing for <c>ItemCapacityPerSize0</c>, <c>ItemCapacityPerSize1</c>, …
/// </summary>
public sealed class DictFieldDefinition
{
    /// <summary>The placeholder substituted with the element index.</summary>
    public const string IndexPlaceholder = "%Index%";

    /// <summary>
    /// Initializes a new instance of the <see cref="DictFieldDefinition"/> class.
    /// </summary>
    /// <param name="xmlFieldMask">Element-name mask containing exactly one <see cref="IndexPlaceholder"/>.</param>
    /// <exception cref="ArgumentException">Thrown when the mask does not contain the placeholder.</exception>
    public DictFieldDefinition(string xmlFieldMask)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldMask);
        if (!xmlFieldMask.Contains(IndexPlaceholder, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Dict field mask must contain '{IndexPlaceholder}'.",
                nameof(xmlFieldMask));
        }

        XmlFieldMask = xmlFieldMask;
    }

    /// <summary>
    /// Gets the element-name mask containing exactly one <see cref="IndexPlaceholder"/>.
    /// </summary>
    public string XmlFieldMask { get; }

    /// <summary>
    /// Gets or sets the resource key of a display name for UI grids.
    /// </summary>
    public string? DisplayNameKey { get; init; }

    /// <summary>
    /// Expands the mask with the given index to produce a concrete element name.
    /// </summary>
    /// <param name="index">Zero-based element index.</param>
    /// <returns>The concrete element name, for example <c>ItemCapacityPerSize2</c>.</returns>
    public string Expand(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        return XmlFieldMask.Replace(IndexPlaceholder, index.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
}
