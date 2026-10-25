namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Describes a non-destructive limit that depends on the value of a class field, for example the
/// number of load-bearing pockets that an LBE class permits. Validation reports fields above the
/// limit but never clears them.
/// </summary>
public sealed class ClassValueLimit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassValueLimit"/> class.
    /// </summary>
    /// <param name="classField">Direct-child XML field that selects the limit.</param>
    /// <param name="classValue">Exact class value, compared case-sensitively after trimming.</param>
    /// <param name="fieldMask">
    /// Direct-child field mask with one <c>%Index%</c> placeholder, for example
    /// <c>lbePocketIndex%Index%</c>.
    /// </param>
    /// <param name="maxOrdinal">Highest one-based ordinal that may carry a value.</param>
    /// <exception cref="ArgumentException">Thrown when a string argument is empty.</exception>
    public ClassValueLimit(string classField, string classValue, string fieldMask, int maxOrdinal)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(classField);
        ArgumentNullException.ThrowIfNull(classValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldMask);
        ArgumentOutOfRangeException.ThrowIfNegative(maxOrdinal);

        ClassField = classField;
        ClassValue = classValue;
        FieldMask = fieldMask;
        MaxOrdinal = maxOrdinal;
    }

    /// <summary>
    /// Gets the direct-child XML field that selects the limit.
    /// </summary>
    public string ClassField { get; }

    /// <summary>
    /// Gets the exact class value, compared after trimming.
    /// </summary>
    public string ClassValue { get; }

    /// <summary>
    /// Gets the direct-child field mask with one <c>%Index%</c> placeholder.
    /// </summary>
    public string FieldMask { get; }

    /// <summary>
    /// Gets the highest one-based ordinal that may carry a value; later ordinals must be zero or empty.
    /// </summary>
    public int MaxOrdinal { get; }
}
