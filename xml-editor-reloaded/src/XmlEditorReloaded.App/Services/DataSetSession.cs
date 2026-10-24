using System.Collections.Concurrent;
using System.IO;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Localization;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Relations;
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
        RelationResolver = new TableRelationResolver(
            catalog,
            repository,
            tableDataDirectory,
            metadataProvider);
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
    public ITableRelationResolver RelationResolver { get; }

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

    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
        CancellationToken cancellationToken = default) =>
        await _metadataProvider.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public Task<OperationResult<TableSetWorkspace>> OpenTableSetAsync(
        TableDescriptor descriptor,
        string? languageCode = null,
        CancellationToken cancellationToken = default) =>
        TableSetWorkspace.LoadAsync(
            descriptor,
            Catalog,
            _metadataProvider,
            TableDataDirectory,
            languageCode,
            cancellationToken);

    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
        TableName sourceTable,
        CancellationToken cancellationToken = default) =>
        await _metadataProvider.GetRelationsAsync(sourceTable, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<OperationResult<TableDescriptor>> EnsureOverlayAsync(
        TableDescriptor descriptor,
        LanguageDescriptor language,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(language);
        cancellationToken.ThrowIfCancellationRequested();

        var existing = descriptor.Overlays.FirstOrDefault(overlay =>
            string.Equals(overlay.Language.Code, language.Code, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return OperationResult<TableDescriptor>.Success(descriptor);
        }

        var metadataResult = await _metadataProvider
            .GetTableAsync(descriptor.TableName, cancellationToken)
            .ConfigureAwait(false);
        if (!metadataResult.IsSuccess)
        {
            return OperationResult<TableDescriptor>.Failure(metadataResult);
        }

        var directory = Path.GetDirectoryName(descriptor.FilePath);
        if (string.IsNullOrEmpty(directory))
        {
            return OperationResult<TableDescriptor>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The directory of table file '{descriptor.FilePath}' could not be resolved.");
        }

        var overlayPath = Path.Combine(
            directory,
            language.ToOverlayFileName(Path.GetFileName(descriptor.FilePath)));
        var overlayDescriptor = new OverlayDescriptor(language, descriptor.FilePath, overlayPath);
        var overlay = File.Exists(overlayPath)
            ? await OverlayDocument
                .LoadAsync(overlayDescriptor, metadataResult.Data!, cancellationToken: cancellationToken)
                .ConfigureAwait(false)
            : await OverlayDocument
                .CreateAsync(overlayDescriptor, metadataResult.Data!, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        if (!overlay.IsSuccess)
        {
            return OperationResult<TableDescriptor>.Failure(overlay);
        }

        return OperationResult<TableDescriptor>.Success(
            descriptor with { Overlays = descriptor.Overlays.Append(overlayDescriptor).ToArray() });
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
