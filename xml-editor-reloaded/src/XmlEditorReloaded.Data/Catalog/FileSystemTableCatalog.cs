using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Data.Catalog;

/// <summary>
/// Discovers base table files under a <c>TableData</c> directory and pairs them with language overlays.
/// </summary>
public sealed class FileSystemTableCatalog : ITableCatalog
{
    private readonly IReadOnlyList<LanguageDescriptor> _languages;
    private readonly ITableMetadataProvider? _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemTableCatalog"/> class.
    /// </summary>
    /// <param name="languages">Languages whose overlay prefixes should be recognized; may be empty.</param>
    /// <param name="metadataProvider">Optional metadata provider used to mark tables as mapped.</param>
    public FileSystemTableCatalog(
        IEnumerable<LanguageDescriptor>? languages = null,
        ITableMetadataProvider? metadataProvider = null)
    {
        _languages = (languages ?? Array.Empty<LanguageDescriptor>()).ToArray();
        _metadataProvider = metadataProvider;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Thrown when <paramref name="tableDataDirectory"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
        string tableDataDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        if (!Directory.Exists(tableDataDirectory))
        {
            return OperationResult<IReadOnlyList<TableDescriptor>>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The TableData directory '{tableDataDirectory}' does not exist.");
        }

        HashSet<string>? mappedTables = null;
        if (_metadataProvider is not null)
        {
            var metadata = await _metadataProvider.GetAllTablesAsync(cancellationToken).ConfigureAwait(false);
            if (!metadata.IsSuccess)
            {
                return OperationResult<IReadOnlyList<TableDescriptor>>.Failure(metadata);
            }

            mappedTables = metadata.Data!
                .Select(static table => table.TableName.Value)
                .ToHashSet(StringComparer.Ordinal);
        }

        IReadOnlyList<LanguageDescriptor> languages = _languages;
        if (languages.Count == 0 && _metadataProvider is not null)
        {
            var languageResult = await _metadataProvider.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);
            if (!languageResult.IsSuccess)
            {
                return OperationResult<IReadOnlyList<TableDescriptor>>.Failure(languageResult);
            }

            languages = languageResult.Data!;
        }

        try
        {
            var descriptors = new List<TableDescriptor>();
            foreach (var file in Directory.EnumerateFiles(tableDataDirectory, "*.xml", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileName = Path.GetFileName(file);
                if (IsOverlayFileName(fileName, languages))
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(tableDataDirectory, file).Replace('\\', '/');
                if (!TableName.TryFromRelativePath(relativePath, out var tableName))
                {
                    continue;
                }

                var overlays = new List<OverlayDescriptor>();
                var directory = Path.GetDirectoryName(file);
                foreach (var language in languages)
                {
                    var overlayName = language.ToOverlayFileName(fileName);
                    var overlayPath = Path.Combine(directory ?? tableDataDirectory, overlayName);
                    if (File.Exists(overlayPath))
                    {
                        overlays.Add(new OverlayDescriptor(language, file, overlayPath));
                    }
                }

                descriptors.Add(new TableDescriptor(
                    tableName,
                    file,
                    relativePath,
                    mappedTables?.Contains(tableName.Value) ?? false,
                    overlays));
            }

            descriptors.Sort(static (left, right) => left.TableName.CompareTo(right.TableName));
            return OperationResult<IReadOnlyList<TableDescriptor>>.Success(descriptors);
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<IReadOnlyList<TableDescriptor>>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the TableData directory '{tableDataDirectory}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<IReadOnlyList<TableDescriptor>>.Failure(
                ErrorCodes.IoError,
                $"The TableData directory '{tableDataDirectory}' could not be scanned: {exception.Message}");
        }
    }

    private static bool IsOverlayFileName(string fileName, IReadOnlyList<LanguageDescriptor> languages) =>
        languages.Any(language => fileName.StartsWith(
            language.OverlayPrefix + ".",
            StringComparison.OrdinalIgnoreCase));
}
