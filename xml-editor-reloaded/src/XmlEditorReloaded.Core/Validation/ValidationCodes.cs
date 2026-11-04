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

    /// <summary>A table with symmetric key pairs contains a record without its reversed counterpart.</summary>
    public const string AsymmetricPair = "validation.asymmetric_pair";

    /// <summary>A numeric value is outside the curated inclusive range of its field.</summary>
    public const string OutOfRange = "validation.out_of_range";

    /// <summary>Several fields of an exclusive flag group are set at the same time.</summary>
    public const string ExclusiveFlags = "validation.exclusive_flags";

    /// <summary>An indexed field is set above the limit that its class value permits.</summary>
    public const string ClassValueLimit = "validation.class_value_limit";

    /// <summary>The table has no metadata and cannot be schema-validated.</summary>
    public const string Unmapped = "validation.unmapped";

    /// <summary>The table file is not well-formed XML and was not validated.</summary>
    public const string MalformedFile = "validation.malformed_file";

    /// <summary>The table file could not be read at all.</summary>
    public const string UnreadableFile = "validation.unreadable_file";

    /// <summary>A language overlay file is malformed or cannot be read.</summary>
    public const string MalformedOverlay = "validation.malformed_overlay";

    /// <summary>A load diagnostic reports a structural problem that does not block validation.</summary>
    public const string LoadDiagnostic = "validation.load_diagnostic";
}
