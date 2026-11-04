using System.IO;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Validation;

/// <summary>
/// Validates every table of one <c>TableData</c> directory and returns a single report. The validator is
/// read-only: it loads documents, never mutates files, and never writes to the data set.
/// </summary>
public sealed class DatasetValidator
{
    /// <summary>
    /// Maximum number of issues retained per table. Severity counts stay exact when details are capped.
    /// </summary>
    public const int MaxIssuesPerTable = 1000;

    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly TableFileLoader _loader;
    private readonly TableValidator _validator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetValidator"/> class.
    /// </summary>
    /// <param name="metadataProvider">Optional metadata provider used for mapped tables.</param>
    /// <param name="loader">Optional table loader; a default loader is used when omitted.</param>
    public DatasetValidator(
        ITableMetadataProvider? metadataProvider = null,
        TableFileLoader? loader = null)
    {
        _metadataProvider = metadataProvider;
        _loader = loader ?? new TableFileLoader();
    }

    /// <summary>
    /// Validates the supplied tables in order.
    /// </summary>
    /// <param name="tableDataDirectory">Absolute <c>TableData</c> directory recorded in the report.</param>
    /// <param name="tables">Base tables discovered by the catalog; overlays are not passed separately.</param>
    /// <param name="lookups">Optional lookup repository used to validate lookup references.</param>
    /// <param name="languageCode">Optional language code for lookup overlays.</param>
    /// <param name="progress">Optional progress receiver, reported once per processed table.</param>
    /// <param name="cancellationToken">Token to cancel the scan.</param>
    /// <returns>
    /// A report with one entry per table. Malformed and unreadable files become error issues instead of
    /// aborting the scan, so one damaged file never hides the rest of the data set.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tableDataDirectory"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tables"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<DatasetValidationReport> ValidateAsync(
        string tableDataDirectory,
        IReadOnlyList<TableDescriptor> tables,
        ILookupRepository? lookups = null,
        string? languageCode = null,
        IProgress<DatasetScanProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        ArgumentNullException.ThrowIfNull(tables);

        var lookupCache = new Dictionary<string, IReadOnlyList<LookupEntry>>(StringComparer.Ordinal);
        var summaries = new List<TableValidationSummary>(tables.Count);
        for (var index = 0; index < tables.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = tables[index];
            summaries.Add(await ValidateTableAsync(
                    descriptor,
                    lookups,
                    languageCode,
                    lookupCache,
                    cancellationToken)
                .ConfigureAwait(false));
            progress?.Report(new DatasetScanProgress(index + 1, tables.Count, descriptor.TableName));
        }

        return new DatasetValidationReport(tableDataDirectory, DateTime.UtcNow, summaries);
    }

    private async Task<TableValidationSummary> ValidateTableAsync(
        TableDescriptor descriptor,
        ILookupRepository? lookups,
        string? languageCode,
        Dictionary<string, IReadOnlyList<LookupEntry>> lookupCache,
        CancellationToken cancellationToken)
    {
        var issues = new IssueCollector();
        var metadata = await GetMetadataAsync(descriptor, cancellationToken).ConfigureAwait(false);
        var effectiveDescriptor = metadata is null ? descriptor with { IsMapped = false } : descriptor;
        var load = await _loader
            .LoadAsync(effectiveDescriptor, metadata, cancellationToken)
            .ConfigureAwait(false);

        if (!load.IsSuccess)
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Error,
                ValidationCodes.UnreadableFile,
                load.ErrorText ?? $"The table file '{descriptor.FilePath}' could not be read.",
                descriptor.TableName));
            return issues.ToSummary(descriptor, isMapped: false, TableValidationState.Unreadable, recordCount: 0);
        }

        var loadResult = load.Data!;
        if (loadResult.Status == TableDocumentStatus.Malformed || loadResult.Document is not TableDocument document)
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Error,
                ValidationCodes.MalformedFile,
                FormatDiagnostics(loadResult.Diagnostics, "The table file is not well-formed XML."),
                descriptor.TableName));
            return issues.ToSummary(descriptor, metadata is not null, TableValidationState.Malformed, recordCount: 0);
        }

        foreach (var diagnostic in loadResult.Diagnostics)
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Warning,
                ValidationCodes.LoadDiagnostic,
                $"{diagnostic.Code}: {diagnostic.Message}",
                descriptor.TableName));
        }

        if (metadata is not null)
        {
            foreach (var overlay in descriptor.Overlays)
            {
                await ValidateOverlayAsync(overlay, metadata, issues, cancellationToken).ConfigureAwait(false);
            }
        }

        var report = await _validator
            .ValidateAsync(
                document,
                lookups,
                languageCode,
                cancellationToken,
                relationTargetValues: null,
                lookupCache)
            .ConfigureAwait(false);
        issues.AddRange(report.Issues);

        return issues.ToSummary(
            descriptor,
            metadata is not null,
            TableValidationState.Validated,
            CountRecords(document, metadata));
    }

    private async Task ValidateOverlayAsync(
        OverlayDescriptor overlay,
        TableMetadata metadata,
        IssueCollector issues,
        CancellationToken cancellationToken)
    {
        var overlayName = Path.GetFileName(overlay.OverlayFilePath);
        var overlayDescriptor = new TableDescriptor(
            metadata.TableName,
            overlay.OverlayFilePath,
            overlayName,
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await _loader
            .LoadOverlayAsync(overlayDescriptor, metadata, cancellationToken)
            .ConfigureAwait(false);
        if (!load.IsSuccess)
        {
            issues.Add(new ValidationIssue(
                ValidationSeverity.Error,
                ValidationCodes.MalformedOverlay,
                $"{overlay.Language.Code} overlay could not be read: {load.ErrorText}",
                metadata.TableName,
                FieldPath: overlayName));
            return;
        }

        if (load.Data!.Status == TableDocumentStatus.Loaded)
        {
            return;
        }

        issues.Add(new ValidationIssue(
            ValidationSeverity.Error,
            ValidationCodes.MalformedOverlay,
            $"{overlay.Language.Code} overlay: "
            + FormatDiagnostics(load.Data.Diagnostics, "The overlay file is not usable."),
            metadata.TableName,
            FieldPath: overlayName));
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

    private static int CountRecords(TableDocument document, TableMetadata? metadata)
    {
        if (document.Records.Count > 0)
        {
            return document.Records.Count;
        }

        return metadata is null
            ? 0
            : new OverlayKeyResolver(metadata).ResolvePrimary(document.Xml).Count;
    }

    private static string FormatDiagnostics(IReadOnlyList<LoadDiagnostic> diagnostics, string fallback)
    {
        if (diagnostics.Count == 0)
        {
            return fallback;
        }

        var first = diagnostics[0];
        var location = first.Line is int line
            ? $" (line {line.ToString(System.Globalization.CultureInfo.InvariantCulture)})"
            : string.Empty;
        return $"{first.Code}: {first.Message}{location}";
    }

    /// <summary>
    /// Counts every issue and retains only the first <see cref="MaxIssuesPerTable"/> of them, so a
    /// damaged table cannot exhaust memory while the totals stay exact.
    /// </summary>
    private sealed class IssueCollector
    {
        private readonly List<ValidationIssue> _issues = new();

        public int InfoCount { get; private set; }

        public int WarningCount { get; private set; }

        public int ErrorCount { get; private set; }

        public int SuppressedCount { get; private set; }

        public void Add(ValidationIssue issue)
        {
            switch (issue.Severity)
            {
                case ValidationSeverity.Error:
                    ErrorCount++;
                    break;
                case ValidationSeverity.Warning:
                    WarningCount++;
                    break;
                default:
                    InfoCount++;
                    break;
            }

            if (_issues.Count < MaxIssuesPerTable)
            {
                _issues.Add(issue);
            }
            else
            {
                SuppressedCount++;
            }
        }

        public void AddRange(IEnumerable<ValidationIssue> issues)
        {
            foreach (var issue in issues)
            {
                Add(issue);
            }
        }

        public TableValidationSummary ToSummary(
            TableDescriptor descriptor,
            bool isMapped,
            TableValidationState state,
            int recordCount) =>
            new(
                descriptor.TableName,
                descriptor.RelativePath,
                isMapped,
                state,
                recordCount,
                _issues,
                InfoCount,
                WarningCount,
                ErrorCount,
                SuppressedCount);
    }
}
