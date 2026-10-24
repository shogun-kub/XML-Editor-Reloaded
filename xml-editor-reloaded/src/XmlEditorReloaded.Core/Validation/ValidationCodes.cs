namespace XmlEditorReloaded.Core.Validation;

/// <summary>
/// Stable validation issue codes emitted by table validators.
/// </summary>
public static class ValidationCodes
{
    /// <summary>A field is not described by metadata.</summary>
    public const string UnknownField = "validation.unknown_field";

    /// <summary>A value does not match the column's declared data type.</summary>
    public const string InvalidType = "validation.invalid_type";

    /// <summary>A lookup value does not exist in the referenced lookup table.</summary>
    public const string LookupMissing = "validation.lookup_missing";

    /// <summary>Two records share the same primary key.</summary>
    public const string DuplicateKey = "validation.duplicate_key";

    /// <summary>A record has no resolvable primary key.</summary>
    public const string MissingKey = "validation.missing_key";

    /// <summary>A dictionary field mask is malformed or produces an invalid index.</summary>
    public const string DictMask = "validation.dict_mask";

    /// <summary>The table has no metadata and cannot be schema-validated.</summary>
    public const string Unmapped = "validation.unmapped";
}
