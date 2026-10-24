using System.Collections.Concurrent;
using System.IO;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Localization;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Default runtime binding for one <c>TableData</c> directory.
/// </summary>
public sealed class DataSetSession : IDataSetSession
{
    private readonly ITableMetadataProvider _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSetSession"/> class.
    /// </summary>
    /// <param name="tableDataDirectory">Absolute <c>TableData</c> directory.</param>
    /// <param name="metadataProvider">Metadata provider shared by the application.</param>
    /// <param name="catalog">Table catalog of the data set.</param>
    /// <param name="repository">Table repository of the data set.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tableDataDirectory"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public DataSetSession(
        string tableDataDirectory,
        ITableMetadataProvider metadataProvider,
        ITableCatalog catalog,
        ITableRepository repository)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(repository);

        TableDataDirectory = tableDataDirectory;
        GameDataDirectory = Directory.GetParent(tableDataDirectory)?.FullName;
        _metadataProvider = metadataProvider;
        Catalog = catalog;
        Repository = repository;
        Lookups = new XmlFileLookupRepository(tableDataDirectory, metadataProvider: metadataProvider);
        Translations = new TranslationCatalog(metadataProvider, catalog, tableDataDirectory);
    }

    /// <inheritdoc />
    public string TableDataDirectory { get; }

    /// <inheritdoc />
    public string? GameDataDirectory { get; }

    /// <inheritdoc />
    public ITableCatalog Catalog { get; }

    /// <inheritdoc />
    public ITableRepository Repository { get; }

    /// <inheritdoc />
    public ILookupRepository Lookups { get; }

    /// <inheritdoc />
    public ITranslationCatalog Translations { get; }

    /// <inheritdoc />
    public async Task<OperationResult<TableWorkspace>> OpenTableAsync(
        TableDescriptor descriptor,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        TableMetadata? metadata = null;
        var effectiveDescriptor = descriptor;
        if (descriptor.IsMapped)
        {
            var metadataResult = await _metadataProvider
                .GetTableAsync(descriptor.TableName, cancellationToken)
                .ConfigureAwait(false);
            if (metadataResult.IsSuccess)
            {
                metadata = metadataResult.Data;
            }
            else if (metadataResult.ErrorCode != ErrorCodes.MetadataNotFound)
            {
                return OperationResult<TableWorkspace>.Failure(metadataResult);
            }
            else
            {
                effectiveDescriptor = descriptor with { IsMapped = false };
            }
        }

        LanguageDescriptor? language = null;
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            var languages = await _metadataProvider.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);
            if (!languages.IsSuccess)
            {
                return OperationResult<TableWorkspace>.Failure(languages);
            }

            language = languages.Data!
                .FirstOrDefault(candidate => string.Equals(
                    candidate.Code,
                    languageCode,
                    StringComparison.OrdinalIgnoreCase));
            if (language is null)
            {
                return OperationResult<TableWorkspace>.Failure(
                    ErrorCodes.OverlayNotFound,
                    $"Language '{languageCode}' is not configured in the metadata database.");
            }
        }

        return await TableWorkspace
            .LoadAsync(effectiveDescriptor, metadata, language, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}

/// <summary>
/// Creates <see cref="DataSetSession"/> instances and caches them per <c>TableData</c> directory.
/// </summary>
public sealed class DataSetSessionFactory : IDataSetSessionFactory
{
    private readonly ConcurrentDictionary<string, IDataSetSession> _sessions =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ITableMetadataProvider _metadataProvider;
    private readonly ITableCatalog _catalog;
    private readonly ITableRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSetSessionFactory"/> class.
    /// </summary>
    /// <param name="metadataProvider">Metadata provider shared by the application.</param>
    /// <param name="catalog">Table catalog shared by the application.</param>
    /// <param name="repository">Table repository shared by the application.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public DataSetSessionFactory(
        ITableMetadataProvider metadataProvider,
        ITableCatalog catalog,
        ITableRepository repository)
    {
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(repository);

        _metadataProvider = metadataProvider;
        _catalog = catalog;
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<OperationResult<IDataSetSession>> OpenAsync(
        string? selectedPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tableData = DataSetPaths.ResolveTableDataDirectory(selectedPath);
        if (tableData is null)
        {
            return Task.FromResult(OperationResult<IDataSetSession>.Failure(
                ErrorCodes.IoFileNotFound,
                "The selected path does not contain a TableData directory."));
        }

        var session = _sessions.GetOrAdd(
            tableData,
            path => new DataSetSession(path, _metadataProvider, _catalog, _repository));
        return Task.FromResult(OperationResult<IDataSetSession>.Success(session));
    }
}
