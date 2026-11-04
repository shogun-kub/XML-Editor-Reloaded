using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Search;

/// <summary>
/// One field value that contains the search term, located by table, record, and field path.
/// </summary>
/// <param name="TableName">Dotted name of the table.</param>
/// <param name="RecordKey">Primary record key, or an empty string for a root-level field.</param>
/// <param name="FieldPath">Slash-separated field path relative to the record or to the document root.</param>
/// <param name="Value">Truncated field text; the match was decided on the complete value.</param>
/// <param name="LanguageCode">
/// Language of the overlay that supplied the value, or <see langword="null"/> when the value comes
/// from the base table.
/// </param>
public sealed record DatasetSearchHit(
    TableName TableName,
    string RecordKey,
    string FieldPath,
    string Value,
    string? LanguageCode);

/// <summary>
/// Result of searching every table of one <c>TableData</c> directory. The match count is exact even
/// when the retained hit list is capped.
/// </summary>
/// <param name="Term">Search term that was used.</param>
/// <param name="MatchCase">Whether the search distinguished letter case.</param>
/// <param name="Hits">Retained hits in scan order: base table first, then overlays in catalog order.</param>
/// <param name="MatchCount">Exact number of matching values, including hits that were not retained.</param>
/// <param name="MatchedTables">Tables with at least one match, in scan order.</param>
/// <param name="SearchedTableCount">Number of tables that were loaded and searched.</param>
/// <param name="FailedTableCount">Number of tables that could not be read or parsed.</param>
/// <param name="SuppressedHitCount">Matching values that were counted but not retained for display.</param>
public sealed record DatasetSearchReport(
    string Term,
    bool MatchCase,
    IReadOnlyList<DatasetSearchHit> Hits,
    int MatchCount,
    IReadOnlyList<TableName> MatchedTables,
    int SearchedTableCount,
    int FailedTableCount,
    int SuppressedHitCount)
{
    /// <summary>
    /// Gets the number of tables with at least one match.
    /// </summary>
    public int MatchedTableCount => MatchedTables.Count;

    /// <summary>
    /// Gets a value indicating whether the search found nothing.
    /// </summary>
    public bool IsEmpty => MatchCount == 0;
}

/// <summary>
/// Searches the field values of every table and language overlay of one data set. Values are compared
/// as exact XML text; a match is a substring of the complete value, and the reported value is truncated
/// only for display.
/// </summary>
public sealed class DatasetSearchService
{
    /// <summary>
    /// Maximum number of hits retained across the whole scan.
    /// </summary>
    public const int MaxHits = 5000;

    /// <summary>
    /// Maximum number of hits retained per table, including its overlays.
    /// </summary>
    public const int MaxHitsPerTable = 500;

    /// <summary>
    /// Maximum number of characters of a reported value.
    /// </summary>
    public const int MaxValueLength = 200;

    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly TableFileLoader _loader;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetSearchService"/> class.
    /// </summary>
    /// <param name="metadataProvider">Optional metadata provider used for mapped tables.</param>
    /// <param name="loader">Optional table loader; a default loader is used when omitted.</param>
    public DatasetSearchService(
        ITableMetadataProvider? metadataProvider = null,
        TableFileLoader? loader = null)
    {
        _metadataProvider = metadataProvider;
        _loader = loader ?? new TableFileLoader();
    }

    /// <summary>
    /// Searches the supplied tables in order.
    /// </summary>
    /// <param name="tables">Base tables discovered by the catalog; overlays are searched with their table.</param>
    /// <param name="term">Non-empty search term; matching is a substring test on the complete value.</param>
    /// <param name="matchCase">When <see langword="true"/>, letter case must match exactly.</param>
    /// <param name="progress">Optional progress receiver, reported once per table.</param>
    /// <param name="cancellationToken">Token to cancel the scan.</param>
    /// <returns>A report with exact counts and a capped hit list.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="term"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tables"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<DatasetSearchReport> SearchAsync(
        IReadOnlyList<TableDescriptor> tables,
        string term,
        bool matchCase = false,
        IProgress<DatasetScanProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tables);
        ArgumentException.ThrowIfNullOrWhiteSpace(term);

        var trimmedTerm = term.Trim();
        var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        var hits = new List<DatasetSearchHit>();
        var matchedTables = new List<TableName>();
        var totalMatches = 0;
        var searchedTables = 0;
        var failedTables = 0;

        for (var index = 0; index < tables.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = tables[index];
            var metadata = await GetMetadataAsync(descriptor, cancellationToken).ConfigureAwait(false);
            var effectiveDescriptor = metadata is null ? descriptor with { IsMapped = false } : descriptor;
            var load = await _loader
                .LoadAsync(effectiveDescriptor, metadata, cancellationToken)
                .ConfigureAwait(false);

            if (!load.IsSuccess
                || load.Data!.Status == TableDocumentStatus.Malformed
                || load.Data.Document is not TableDocument document)
            {
                failedTables++;
                progress?.Report(new DatasetScanProgress(index + 1, tables.Count, descriptor.TableName));
                continue;
            }

            searchedTables++;
            var tableMatches = 0;
            SearchDocument(
                document,
                descriptor.TableName,
                metadata,
                languageCode: null,
                trimmedTerm,
                comparison,
                hits,
                ref tableMatches,
                ref totalMatches);

            if (metadata is not null)
            {
                foreach (var overlay in descriptor.Overlays)
                {
                    var overlayDocument = await LoadOverlayAsync(overlay, metadata, cancellationToken)
                        .ConfigureAwait(false);
                    if (overlayDocument is null)
                    {
                        continue;
                    }

                    SearchDocument(
                        overlayDocument,
                        descriptor.TableName,
                        metadata,
                        overlay.Language.Code,
                        trimmedTerm,
                        comparison,
                        hits,
                        ref tableMatches,
                        ref totalMatches);
                }
            }

            if (tableMatches > 0)
            {
                matchedTables.Add(descriptor.TableName);
            }

            progress?.Report(new DatasetScanProgress(index + 1, tables.Count, descriptor.TableName));
        }

        return new DatasetSearchReport(
            trimmedTerm,
            matchCase,
            hits,
            totalMatches,
            matchedTables,
            searchedTables,
            failedTables,
            totalMatches - hits.Count);
    }

    private void SearchDocument(
        TableDocument document,
        TableName tableName,
        TableMetadata? metadata,
        string? languageCode,
        string term,
        StringComparison comparison,
        List<DatasetSearchHit> hits,
        ref int tableMatches,
        ref int totalMatches)
    {
        var root = document.Xml.Root;
        if (root is null)
        {
            return;
        }

        var records = BuildRecordMap(document, metadata);
        SearchElement(
            root,
            string.Empty,
            string.Empty,
            records,
            tableName,
            languageCode,
            term,
            comparison,
            hits,
            ref tableMatches,
            ref totalMatches);
    }

    private static Dictionary<XElement, string> BuildRecordMap(TableDocument document, TableMetadata? metadata)
    {
        var records = new Dictionary<XElement, string>();
        if (metadata is not null)
        {
            foreach (var record in new OverlayKeyResolver(metadata).ResolvePrimary(document.Xml))
            {
                records[record.Element] = record.Key ?? string.Empty;
            }

            return records;
        }

        foreach (var record in document.Records)
        {
            records[record.Element] = record.GetKey() ?? string.Empty;
        }

        return records;
    }

    private static void SearchElement(
        XElement parent,
        string prefix,
        string recordKey,
        Dictionary<XElement, string> records,
        TableName tableName,
        string? languageCode,
        string term,
        StringComparison comparison,
        List<DatasetSearchHit> hits,
        ref int tableMatches,
        ref int totalMatches)
    {
        foreach (var child in parent.Elements())
        {
            if (records.TryGetValue(child, out var key))
            {
                // A record restarts the path and carries its own key.
                SearchElement(
                    child,
                    string.Empty,
                    key,
                    records,
                    tableName,
                    languageCode,
                    term,
                    comparison,
                    hits,
                    ref tableMatches,
                    ref totalMatches);
                continue;
            }

            var path = prefix.Length == 0
                ? child.Name.LocalName
                : string.Concat(prefix, "/", child.Name.LocalName);
            if (child.HasElements)
            {
                SearchElement(
                    child,
                    path,
                    recordKey,
                    records,
                    tableName,
                    languageCode,
                    term,
                    comparison,
                    hits,
                    ref tableMatches,
                    ref totalMatches);
                continue;
            }

            var value = child.Value;
            if (value.Length == 0
                || string.IsNullOrWhiteSpace(value)
                || !value.Contains(term, comparison))
            {
                continue;
            }

            tableMatches++;
            totalMatches++;
            if (hits.Count >= MaxHits || tableMatches > MaxHitsPerTable)
            {
                continue;
            }

            hits.Add(new DatasetSearchHit(
                tableName,
                recordKey,
                path,
                Truncate(value),
                languageCode));
        }
    }

    private async Task<TableDocument?> LoadOverlayAsync(
        OverlayDescriptor overlay,
        TableMetadata metadata,
        CancellationToken cancellationToken)
    {
        var overlayDescriptor = new TableDescriptor(
            metadata.TableName,
            overlay.OverlayFilePath,
            Path.GetFileName(overlay.OverlayFilePath),
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await _loader
            .LoadOverlayAsync(overlayDescriptor, metadata, cancellationToken)
            .ConfigureAwait(false);
        return load.IsSuccess && load.Data!.Status == TableDocumentStatus.Loaded
            ? load.Data.GetDocument<TableDocument>()
            : null;
    }

    private async Task<TableMetadata?> GetMetadataAsync(
        TableDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        if (!descriptor.IsMapped || _metadataProvider is null)
        {
            return null;
        }

        var result = await _metadataProvider
            .GetTableAsync(descriptor.TableName, cancellationToken)
            .ConfigureAwait(false);
        return result.IsSuccess && result.ErrorCode != ErrorCodes.MetadataNotFound ? result.Data : null;
    }

    private static string Truncate(string value) =>
        value.Length <= MaxValueLength
            ? value
            : string.Concat(value.AsSpan(0, MaxValueLength - 1), "\u2026");
}
