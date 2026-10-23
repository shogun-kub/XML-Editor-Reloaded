namespace XmlEditorReloaded.Core.Localization;

/// <summary>
/// Report describing the state of a language overlay relative to its base table.
/// </summary>
/// <param name="Issues">Detected translation issues.</param>
public sealed record TranslationReport(IReadOnlyList<TranslationIssue> Issues)
{
    /// <summary>
    /// Gets an empty report with no issues.
    /// </summary>
    public static TranslationReport Empty { get; } = new(Array.Empty<TranslationIssue>());
}

/// <summary>
/// A single translation problem detected while comparing a base table with an overlay.
/// </summary>
/// <param name="Kind">Category of the issue.</param>
/// <param name="RecordKey">Record key the issue refers to, or <see langword="null"/> when not applicable.</param>
/// <param name="FieldPath">Slash-separated field path the issue refers to, or <see langword="null"/> when record-level.</param>
/// <param name="Message">Human-readable description.</param>
public sealed record TranslationIssue(TranslationIssueKind Kind, string? RecordKey, string? FieldPath, string Message);

/// <summary>
/// Category of a translation issue.
/// </summary>
public enum TranslationIssueKind
{
    /// <summary>A base record has no corresponding overlay record.</summary>
    MissingTranslation,

    /// <summary>An overlay record has no corresponding base record.</summary>
    OrphanKey,

    /// <summary>The same key appears more than once in the overlay.</summary>
    DuplicateKey,

    /// <summary>An overlay value is an empty string.</summary>
    EmptyValue,

    /// <summary>An overlay value is identical to the base value.</summary>
    IdenticalToBase,

    /// <summary>A nested collection has a different element count in base and overlay, for example gear kits.</summary>
    KeyCountMismatch,

    /// <summary>An overlay record key could not be resolved from metadata.</summary>
    UnresolvableKey,
}
