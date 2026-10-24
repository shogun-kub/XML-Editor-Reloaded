using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Repository;

/// <summary>
/// Loads table documents with metadata when available and falls back to document-driven mode otherwise.
/// </summary>
public sealed class TableRepository : ITableRepository
{
    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly TableFileLoader _loader;
    private readonly TableFileSaver _saver;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableRepository"/> class.
    /// </summary>
    /// <param name="metadataProvider">Optional metadata provider for mapped tables.</param>
    /// <param name="loader">Optional file loader; a default loader is used when omitted.</param>
    /// <param name="saver">Optional file saver; a default saver is used when omitted.</param>
    public TableRepository(
        ITableMetadataProvider? metadataProvider = null,
        TableFileLoader? loader = null,
        TableFileSaver? saver = null)
    {
        _metadataProvider = metadataProvider;
        _loader = loader ?? new TableFileLoader();
        _saver = saver ?? new TableFileSaver();
    }

    /// <inheritdoc />
    public async Task<OperationResult<TableLoadResult>> LoadTableAsync(
        TableDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        TableMetadata? metadata = null;
        var effectiveDescriptor = descriptor;

        if (descriptor.IsMapped && _metadataProvider is not null)
        {
            var metadataResult = await _metadataProvider.GetTableAsync(descriptor.TableName, cancellationToken).ConfigureAwait(false);
            if (metadataResult.IsSuccess)
            {
                metadata = metadataResult.Data;
            }
            else if (metadataResult.ErrorCode != ErrorCodes.MetadataNotFound)
            {
                return OperationResult<TableLoadResult>.Failure(metadataResult);
            }
            else
            {
                effectiveDescriptor = descriptor with { IsMapped = false };
            }
        }
        else if (descriptor.IsMapped)
        {
            effectiveDescriptor = descriptor with { IsMapped = false };
        }

        var load = await _loader.LoadAsync(effectiveDescriptor, metadata, cancellationToken).ConfigureAwait(false);
        if (!load.IsSuccess)
        {
            return load;
        }

        if (metadata is null && load.Data!.Status != TableDocumentStatus.Malformed)
        {
            var diagnostics = load.Data.Diagnostics.ToList();
            diagnostics.Add(new LoadDiagnostic(
                ErrorCodes.MetadataUnmapped,
                $"Table '{descriptor.TableName}' has no metadata and is loaded in document-driven mode."));
            return OperationResult<TableLoadResult>.Success(load.Data with { Diagnostics = diagnostics });
        }

        return load;
    }

    /// <inheritdoc />
    public Task<OperationResult> SaveTableAsync(
        TableLoadResult loadResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(loadResult);

        if (loadResult.Document is not TableDocument document)
        {
            return Task.FromResult(OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "The load result has no saveable document."));
        }

        return _saver.SaveAsync(document, cancellationToken);
    }
}
