using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Data.Localization;

/// <summary>
/// Combines table metadata and the file-system catalog into translation information.
/// </summary>
public sealed class TranslationCatalog : ITranslationCatalog
{
    private readonly ITableMetadataProvider _metadataProvider;
    private readonly ITableCatalog _tableCatalog;
    private readonly string _tableDataDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationCatalog"/> class.
    /// </summary>
    /// <param name="metadataProvider">Provider of translatable field metadata.</param>
    /// <param name="tableCatalog">Catalog used to discover overlays for a table.</param>
    /// <param name="tableDataDirectory">Directory passed to the catalog for discovery.</param>
    public TranslationCatalog(
        ITableMetadataProvider metadataProvider,
        ITableCatalog tableCatalog,
        string tableDataDirectory)
    {
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentNullException.ThrowIfNull(tableCatalog);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        _metadataProvider = metadataProvider;
        _tableCatalog = tableCatalog;
        _tableDataDirectory = tableDataDirectory;
    }

    /// <inheritdoc />
    public Task<OperationResult<IReadOnlyList<OverlayDescriptor>>> GetOverlayDescriptorsAsync(
        TableDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(OperationResult<IReadOnlyList<OverlayDescriptor>>.Success(descriptor.Overlays));
    }

    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyList<string>>> GetTranslatableFieldsAsync(
        TableName tableName,
        CancellationToken cancellationToken = default)
    {
        var metadata = await _metadataProvider.GetTableAsync(tableName, cancellationToken).ConfigureAwait(false);
        return metadata.IsSuccess
            ? OperationResult<IReadOnlyList<string>>.Success(metadata.Data!.TranslatableFieldPaths)
            : OperationResult<IReadOnlyList<string>>.Failure(metadata);
    }

    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesForAsync(
        TableName tableName,
        CancellationToken cancellationToken = default)
    {
        var tables = await _tableCatalog.GetTablesAsync(_tableDataDirectory, cancellationToken).ConfigureAwait(false);
        if (!tables.IsSuccess)
        {
            return OperationResult<IReadOnlyList<LanguageDescriptor>>.Failure(tables);
        }

        var descriptor = tables.Data!.FirstOrDefault(table => table.TableName == tableName);
        if (descriptor is null)
        {
            return OperationResult<IReadOnlyList<LanguageDescriptor>>.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{tableName}' was not found in the data directory.");
        }

        return OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(
            descriptor.Overlays.Select(static overlay => overlay.Language).ToArray());
    }
}
