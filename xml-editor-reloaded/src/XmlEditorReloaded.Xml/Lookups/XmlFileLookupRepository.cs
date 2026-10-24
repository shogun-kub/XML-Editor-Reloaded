using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Lookups;

/// <summary>
/// Reads lookup tables from <c>TableData/Lookup/*.xml</c> and optionally applies a language overlay.
/// </summary>
public sealed class XmlFileLookupRepository : ILookupRepository
{
    private readonly string _tableDataDirectory;
    private readonly IReadOnlyDictionary<string, LanguageDescriptor> _languages;
    private readonly ITableMetadataProvider? _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFileLookupRepository"/> class.
    /// </summary>
    /// <param name="tableDataDirectory">Absolute path of the <c>TableData</c> directory.</param>
    /// <param name="languages">Languages whose overlay prefixes may be used; may be empty.</param>
    /// <param name="metadataProvider">Optional provider used to discover languages when none are supplied.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tableDataDirectory"/> is empty.</exception>
    public XmlFileLookupRepository(
        string tableDataDirectory,
        IEnumerable<LanguageDescriptor>? languages = null,
        ITableMetadataProvider? metadataProvider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        _tableDataDirectory = tableDataDirectory;
        _metadataProvider = metadataProvider;
        _languages = (languages ?? Array.Empty<LanguageDescriptor>())
            .ToDictionary(static language => language.Code, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Reads the entries of a lookup table and applies the requested language overlay when available.
    /// </summary>
    /// <param name="lookupName">Dotted lookup table name, for example <c>Lookup.ItemClass</c>.</param>
    /// <param name="languageCode">Optional language code whose overlay should override base names.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the entries in base-file order.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>,
    /// <c>xml.invalid_lookup_entry</c>, <c>io.access_denied</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lookupName"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<LookupEntry>>> GetEntriesAsync(
        TableName lookupName,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        var relativePath = lookupName.ToRelativePath();
        var basePath = Path.Combine(_tableDataDirectory, relativePath);

        if (!File.Exists(basePath))
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The lookup file '{basePath}' does not exist.");
        }

        try
        {
            var entries = await ReadEntriesAsync(basePath, cancellationToken).ConfigureAwait(false);

            var languages = _languages;
            if (languages.Count == 0 && _metadataProvider is not null)
            {
                var languageResult = await _metadataProvider.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);
                if (languageResult.IsSuccess)
                {
                    languages = languageResult.Data!
                        .ToDictionary(static language => language.Code, StringComparer.OrdinalIgnoreCase);
                }
            }

            if (languageCode is not null && languages.TryGetValue(languageCode, out var language))
            {
                var overlayPath = Path.Combine(
                    Path.GetDirectoryName(basePath) ?? _tableDataDirectory,
                    language.ToOverlayFileName(Path.GetFileName(basePath)));

                if (File.Exists(overlayPath))
                {
                    var overlayEntries = await ReadEntriesAsync(overlayPath, cancellationToken).ConfigureAwait(false);
                    entries = Merge(entries, overlayEntries);
                }
            }

            return OperationResult<IReadOnlyList<LookupEntry>>.Success(entries);
        }
        catch (XmlException exception)
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.XmlParseError,
                $"The lookup file could not be parsed: {exception.Message}");
        }
        catch (InvalidDataException exception)
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.XmlInvalidLookupEntry,
                exception.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the lookup file '{basePath}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.IoError,
                $"The lookup file '{basePath}' could not be read: {exception.Message}");
        }
    }

    private static async Task<List<LookupEntry>> ReadEntriesAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        using var reader = XmlReader.Create(stream, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreWhitespace = false,
            IgnoreComments = false,
            CloseInput = false,
        });

        var document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        if (document.Root is null)
        {
            throw new XmlException("The lookup document has no root element.");
        }

        var entries = new List<LookupEntry>();
        var index = 0;
        foreach (var element in document.Root.Elements())
        {
            var idText = element.Elements().FirstOrDefault(static child => child.Name.LocalName == "id")?.Value;
            var name = element.Elements().FirstOrDefault(static child => child.Name.LocalName == "name")?.Value;

            if (!int.TryParse(idText, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var id))
            {
                throw new InvalidDataException(
                    $"Lookup entry #{index} in '{path}' has no valid integer 'id' element.");
            }

            if (name is null)
            {
                throw new InvalidDataException(
                    $"Lookup entry #{index} in '{path}' has no 'name' element.");
            }

            entries.Add(new LookupEntry(id, name));
            index++;
        }

        return entries;
    }

    private static List<LookupEntry> Merge(List<LookupEntry> baseEntries, List<LookupEntry> overlayEntries)
    {
        var overlayById = overlayEntries
            .GroupBy(static entry => entry.Id)
            .ToDictionary(static group => group.Key, static group => group.Last().Name);

        return baseEntries
            .Select(entry => overlayById.TryGetValue(entry.Id, out var translated)
                ? new LookupEntry(entry.Id, translated)
                : entry)
            .ToList();
    }
}
