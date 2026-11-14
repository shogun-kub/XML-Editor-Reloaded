using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Compares the content of the language overlays of two loaded tables. Overlay files are read without
/// being written; a malformed or unreadable overlay is reported as a side state, not as an operation
/// failure. Cell-level differences are reported only when both overlay files parsed.
/// </summary>
internal static class OverlayFileComparer
{
    /// <summary>
    /// Compares the overlays of both sides, one entry per language present on either side.
    /// </summary>
    /// <param name="left">Baseline document, or <see langword="null"/> when the table is absent.</param>
    /// <param name="right">Candidate document, or <see langword="null"/> when the table is absent.</param>
    /// <param name="metadata">Table metadata used to load and identify overlay records.</param>
    /// <param name="loader">Loader used to parse overlay files.</param>
    /// <param name="cancellationToken">Token to cancel the comparison.</param>
    /// <returns>Overlay comparisons ordered by language code.</returns>
    public static async Task<IReadOnlyList<OverlayComparison>> CompareAsync(
        TableDocument? left,
        TableDocument? right,
        TableMetadata? metadata,
        TableFileLoader loader,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(loader);

        var languages = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddLanguages(left, languages, seen);
        AddLanguages(right, languages, seen);
        if (languages.Count == 0)
        {
            return Array.Empty<OverlayComparison>();
        }

        var recordElementName = left?.RecordElementName ?? right?.RecordElementName;
        var results = new List<OverlayComparison>(languages.Count);
        foreach (var language in languages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var leftSide = await LoadSideAsync(
                    left,
                    language,
                    metadata,
                    loader,
                    recordElementName,
                    cancellationToken)
                .ConfigureAwait(false);
            var rightSide = await LoadSideAsync(
                    right,
                    language,
                    metadata,
                    loader,
                    recordElementName,
                    cancellationToken)
                .ConfigureAwait(false);
            results.Add(Compare(language, leftSide, rightSide));
        }

        return results;
    }

    private static void AddLanguages(
        TableDocument? document,
        List<string> languages,
        HashSet<string> seen)
    {
        if (document is null)
        {
            return;
        }

        foreach (var overlay in document.Descriptor.Overlays)
        {
            if (seen.Add(overlay.Language.Code))
            {
                languages.Add(overlay.Language.Code);
            }
        }
    }

    private static async Task<LoadedOverlay> LoadSideAsync(
        TableDocument? document,
        string language,
        TableMetadata? metadata,
        TableFileLoader loader,
        string? recordElementName,
        CancellationToken cancellationToken)
    {
        if (document is null)
        {
            return new LoadedOverlay(OverlaySide.Missing, null);
        }

        var descriptor = document.Descriptor.Overlays.FirstOrDefault(candidate =>
            string.Equals(candidate.Language.Code, language, StringComparison.OrdinalIgnoreCase));
        if (descriptor is null)
        {
            return new LoadedOverlay(OverlaySide.Missing, null);
        }

        var path = descriptor.OverlayFilePath;
        try
        {
            if (metadata is not null)
            {
                var overlayDescriptor = new TableDescriptor(
                    document.Descriptor.TableName,
                    path,
                    document.Descriptor.RelativePath,
                    IsMapped: true,
                    Array.Empty<OverlayDescriptor>());
                var load = await loader
                    .LoadOverlayAsync(overlayDescriptor, metadata, cancellationToken)
                    .ConfigureAwait(false);
                if (!load.IsSuccess)
                {
                    return new LoadedOverlay(
                        new OverlaySide(path, OverlayFileStatus.Unreadable, load.ErrorText, 0),
                        null);
                }

                var result = load.Data!;
                if (result.Status == TableDocumentStatus.Malformed)
                {
                    return new LoadedOverlay(
                        new OverlaySide(path, OverlayFileStatus.Malformed, FirstDiagnosticMessage(result.Diagnostics), 0),
                        null);
                }

                var overlayDocument = result.GetDocument<TableDocument>();
                if (overlayDocument is null)
                {
                    return new LoadedOverlay(
                        new OverlaySide(
                            path,
                            OverlayFileStatus.Unreadable,
                            "The overlay load result has no analyzable document.",
                            0),
                        null);
                }

                var cells = TableCellsReader.Read(overlayDocument);
                return new LoadedOverlay(
                    new OverlaySide(path, OverlayFileStatus.Loaded, null, cells.CellCount),
                    cells);
            }

            var xml = await ReadOverlayXmlAsync(path, cancellationToken).ConfigureAwait(false);
            var inferred = recordElementName
                ?? xml.Root?.Elements().FirstOrDefault()?.Name.LocalName
                ?? xml.Root?.Name.LocalName
                ?? string.Empty;
            var documentCells = TableCellsReader.Read(xml, inferred);
            return new LoadedOverlay(
                new OverlaySide(path, OverlayFileStatus.Loaded, null, documentCells.CellCount),
                documentCells);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (XmlException exception)
        {
            return new LoadedOverlay(
                new OverlaySide(path, OverlayFileStatus.Malformed, exception.Message, 0),
                null);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return new LoadedOverlay(
                new OverlaySide(path, OverlayFileStatus.Unreadable, exception.Message, 0),
                null);
        }
    }

    private static async Task<XDocument> ReadOverlayXmlAsync(string path, CancellationToken cancellationToken)
    {
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        using var stream = new MemoryStream(bytes, writable: false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreWhitespace = false,
            IgnoreComments = false,
            CloseInput = false,
            ConformanceLevel = ConformanceLevel.Document,
        });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }

    private static string? FirstDiagnosticMessage(IReadOnlyList<LoadDiagnostic> diagnostics) =>
        diagnostics.Count == 0 ? null : diagnostics[0].Message;

    private static OverlayComparison Compare(
        string language,
        LoadedOverlay left,
        LoadedOverlay right)
    {
        var status = left.Side.Status == OverlayFileStatus.Missing
            ? OverlayComparisonStatus.RightOnly
            : right.Side.Status == OverlayFileStatus.Missing
                ? OverlayComparisonStatus.LeftOnly
                : OverlayComparisonStatus.Both;

        if (status != OverlayComparisonStatus.Both
            || left.Side.Status != OverlayFileStatus.Loaded
            || right.Side.Status != OverlayFileStatus.Loaded
            || left.Cells is null
            || right.Cells is null)
        {
            return new OverlayComparison(
                language,
                status,
                left.Side,
                right.Side,
                0,
                0,
                0,
                false,
                Array.Empty<OverlayCellChange>());
        }

        var leftCells = Flatten(left.Cells);
        var rightCells = Flatten(right.Cells);
        var leftByKey = leftCells.ToDictionary(static cell => cell.Identity, StringComparer.Ordinal);
        var rightByKey = rightCells.ToDictionary(static cell => cell.Identity, StringComparer.Ordinal);

        var added = 0;
        var removed = 0;
        var changed = 0;
        var truncated = false;
        var details = new List<OverlayCellChange>();

        foreach (var identity in UnionIdentities(leftCells, rightCells))
        {
            leftByKey.TryGetValue(identity, out var leftCell);
            rightByKey.TryGetValue(identity, out var rightCell);
            var sample = leftCell ?? rightCell;

            if (leftCell is null)
            {
                added++;
                AddDetail(new OverlayCellChange(
                    sample!.Key,
                    sample.Path,
                    ComparisonDifferenceKind.RightOnly,
                    null,
                    sample.Value));
                continue;
            }

            if (rightCell is null)
            {
                removed++;
                AddDetail(new OverlayCellChange(
                    sample!.Key,
                    sample.Path,
                    ComparisonDifferenceKind.LeftOnly,
                    sample.Value,
                    null));
                continue;
            }

            if (string.Equals(leftCell.Value, rightCell.Value, StringComparison.Ordinal))
            {
                continue;
            }

            changed++;
            AddDetail(new OverlayCellChange(
                sample!.Key,
                sample.Path,
                ComparisonDifferenceKind.Changed,
                leftCell.Value,
                rightCell.Value));
        }

        return new OverlayComparison(
            language,
            status,
            left.Side,
            right.Side,
            added,
            removed,
            changed,
            truncated,
            details);

        void AddDetail(OverlayCellChange change)
        {
            if (truncated)
            {
                return;
            }

            if (details.Count >= TableComparisonLimits.MaxOverlayCellDetailsPerLanguage)
            {
                truncated = true;
                return;
            }

            details.Add(change);
        }
    }

    private static List<string> UnionIdentities(
        IReadOnlyList<FlatCell> left,
        IReadOnlyList<FlatCell> right)
    {
        var identities = new List<string>(left.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var cell in left)
        {
            if (seen.Add(cell.Identity))
            {
                identities.Add(cell.Identity);
            }
        }

        foreach (var cell in right)
        {
            if (seen.Add(cell.Identity))
            {
                identities.Add(cell.Identity);
            }
        }

        return identities;
    }

    private static List<FlatCell> Flatten(TableCells cells)
    {
        var result = new List<FlatCell>(cells.CellCount);
        var recordCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var record in cells.Records)
        {
            recordCounts[record.Identity] = recordCounts.GetValueOrDefault(record.Identity) + 1;
        }

        var recordOccurrences = new Dictionary<string, int>(StringComparer.Ordinal);
        var fieldOccurrences = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var record in cells.Records)
        {
            var occurrence = recordOccurrences.GetValueOrDefault(record.Identity) + 1;
            recordOccurrences[record.Identity] = occurrence;
            var display = recordCounts[record.Identity] > 1
                ? string.Concat(record.Display, " #", occurrence.ToString(System.Globalization.CultureInfo.InvariantCulture))
                : record.Display;

            foreach (var field in record.Fields)
            {
                var baseIdentity = string.Concat(record.Identity, "\u0000", field.Name);
                var fieldOccurrence = fieldOccurrences.GetValueOrDefault(baseIdentity) + 1;
                fieldOccurrences[baseIdentity] = fieldOccurrence;
                var identity = string.Concat(
                    baseIdentity,
                    "\u0000",
                    fieldOccurrence.ToString(System.Globalization.CultureInfo.InvariantCulture));
                result.Add(new FlatCell(identity, display, field.Name, field.Value));
            }
        }

        foreach (var root in cells.RootFields)
        {
            var separator = root.Name.IndexOf('/', StringComparison.Ordinal);
            var key = separator < 0 ? string.Empty : root.Name[..separator];
            var path = separator < 0 ? root.Name : root.Name[(separator + 1)..];
            result.Add(new FlatCell(string.Concat("root\u0000", root.Name), key, path, root.Value));
        }

        return result;
    }

    private sealed record LoadedOverlay(OverlaySide Side, TableCells? Cells);

    private sealed record FlatCell(string Identity, string Key, string Path, string Value);
}
