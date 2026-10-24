using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Provides language overlay discovery and translatable field metadata.
/// </summary>
public interface ITranslationCatalog
{
    /// <summary>
    /// Returns the overlays discovered for a table descriptor.
    /// </summary>
    /// <param name="descriptor">Table descriptor.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the overlays;
    /// on failure, <c>error_code</c> and <c>error_text</c> describe the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<OverlayDescriptor>>> GetOverlayDescriptorsAsync(TableDescriptor descriptor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the slash-separated field paths of a table that may be translated via overlays.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the field paths;
    /// on failure, <c>error_code</c> is <c>metadata.not_found</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<string>>> GetTranslatableFieldsAsync(TableName tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the languages that have an overlay for the given table.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the languages;
    /// on failure, <c>error_code</c> and <c>error_text</c> describe the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesForAsync(TableName tableName, CancellationToken cancellationToken = default);
}
