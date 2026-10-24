using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// Routes edits to the base table or to a selected language overlay and saves them independently.
/// </summary>
public sealed class TableEditSession
{
    private readonly OverlayWriter _overlayWriter = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableEditSession"/> class.
    /// </summary>
    /// <param name="baseDocument">Loaded base table document.</param>
    /// <param name="metadata">Metadata of the base table.</param>
    /// <param name="overlay">Optional overlay for the currently selected language.</param>
    public TableEditSession(TableDocument baseDocument, TableMetadata metadata, OverlayDocument? overlay = null)
    {
        ArgumentNullException.ThrowIfNull(baseDocument);
        ArgumentNullException.ThrowIfNull(metadata);
        BaseDocument = baseDocument;
        Metadata = metadata;
        Overlay = overlay;
    }

    /// <summary>
    /// Gets the base table document.
    /// </summary>
    public TableDocument BaseDocument { get; }

    /// <summary>
    /// Gets the table metadata.
    /// </summary>
    public TableMetadata Metadata { get; }

    /// <summary>
    /// Gets the selected language overlay, or <see langword="null"/> when editing the base table only.
    /// </summary>
    public OverlayDocument? Overlay { get; }

    /// <summary>
    /// Gets the merged base/translation view.
    /// </summary>
    /// <returns>Merged records and root-level singleton cells.</returns>
    public MergedTable Merge() => new OverlayMerger(Metadata).Merge(BaseDocument, Overlay?.Document);

    /// <summary>
    /// Sets a value, routing translatable fields to the overlay when one is selected.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">
    /// When <see langword="true"/>, a missing direct field is appended to the base record instead of
    /// failing. Has no effect on overlay writes, which create missing translated fields on demand.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>overlay.unresolvable_key</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetValue(string recordKey, FieldPath path, string rawText, bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);
        return SetValue(recordKey, path, occurrenceIndex: 0, rawText, addIfMissing);
    }

    /// <summary>
    /// Sets a field value at a specific occurrence of the first path segment. A translatable field of a
    /// selected overlay can only be written through <see cref="SetNestedValue"/>, because the overlay
    /// record key must identify the nested occurrence.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base leaf is appended to the record.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>overlay.unresolvable_key</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetValue(
        string recordKey,
        FieldPath path,
        int occurrenceIndex,
        string rawText,
        bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        if (Overlay is not null && IsTranslatable(path))
        {
            if (occurrenceIndex > 0)
            {
                return OperationResult.Failure(
                    ErrorCodes.OverlayUnresolvableKey,
                    $"Field '{path.Text}' is translatable; write occurrence {occurrenceIndex} through SetNestedValue with a nested record key.");
            }

            return _overlayWriter.SetValue(Overlay, OverlayRecordKey.ForRecord(recordKey), path, rawText);
        }

        var record = BaseDocument.FindRecord(recordKey);
        return record is null
            ? OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.")
            : record.SetFieldValue(path, occurrenceIndex, rawText, addIfMissing);
    }

    /// <summary>
    /// Sets a nested translatable field in the overlay, for example a gear-kit name.
    /// </summary>
    /// <param name="key">Primary and nested record keys.</param>
    /// <param name="path">Field path relative to the resolved nested record.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c> or <c>overlay.unresolvable_key</c>.
    /// </returns>
    public OperationResult SetNestedValue(OverlayRecordKey key, FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        return Overlay is null
            ? OperationResult.Failure(ErrorCodes.XmlNotSaveable, "No language overlay is selected.")
            : _overlayWriter.SetValue(Overlay, key, path, rawText);
    }

    /// <summary>
    /// Sets a root-level translatable field in the overlay, for example <c>COUNTRY/countryName</c>.
    /// </summary>
    /// <param name="path">Path from the document root.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetRootValue(FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        return Overlay is null
            ? OperationResult.Failure(ErrorCodes.XmlNotSaveable, "No language overlay is selected.")
            : _overlayWriter.SetRootValue(Overlay, path, rawText);
    }

    /// <summary>
    /// Saves the base document and the overlay independently. Clean documents are not rewritten.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, the error from the base or overlay saver is returned.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled; the current file is left unchanged.</exception>
    public async Task<OperationResult> SaveAsync(CancellationToken cancellationToken = default)
    {
        var saver = new TableFileSaver();
        var baseSave = await saver.SaveAsync(BaseDocument, cancellationToken).ConfigureAwait(false);
        if (!baseSave.IsSuccess)
        {
            return baseSave;
        }

        return Overlay is null
            ? OperationResult.Success()
            : await Overlay.SaveAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether a field path is translatable according to metadata.
    /// </summary>
    /// <param name="path">Field path relative to a record or root.</param>
    /// <returns><see langword="true"/> when the path matches a translatable metadata path.</returns>
    public bool IsTranslatable(FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var text = path.Text;
        return Metadata.TranslatableFieldPaths.Any(translatable =>
            string.Equals(translatable, text, StringComparison.Ordinal)
            || translatable.EndsWith("/" + text, StringComparison.Ordinal)
            || text.EndsWith("/" + translatable, StringComparison.Ordinal));
    }
}
