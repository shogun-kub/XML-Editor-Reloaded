using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Environment-gated test over a complete <c>TableData</c> corpus.
/// Set <c>XER_CORPUS_DIR</c> to a copy of a game data directory to enable it.
/// </summary>
public class CorpusRoundTripTests
{
    /// <summary>
    /// Loads and saves every XML table in the corpus without edits and requires that no file is rewritten.
    /// Malformed files are allowed only at the known <c>Lookup/AttachmentPoint.xml</c> location.
    /// </summary>
    [Fact]
    public async Task Corpus_UnmodifiedFiles_AreNeverRewritten()
    {
        var corpusDirectory = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpusDirectory) || !Directory.Exists(corpusDirectory))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var copy = Path.Combine(workspace.DirectoryPath, "TableData");
        TestWorkspace.CopyDirectory(corpusDirectory, copy);

        var loader = new TableFileLoader();
        var saver = new TableFileSaver();
        var failures = new List<string>();
        var checkedFiles = 0;

        foreach (var file in Directory.EnumerateFiles(copy, "*.xml", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(copy, file).Replace('\\', '/');
            if (!TableName.TryFromRelativePath(relativePath, out var tableName))
            {
                continue;
            }

            checkedFiles++;
            var descriptor = new TableDescriptor(
                tableName,
                file,
                relativePath,
                IsMapped: false,
                Array.Empty<OverlayDescriptor>());

            var load = await loader.LoadAsync(descriptor);
            if (!load.IsSuccess)
            {
                failures.Add($"{relativePath}: load failed: {load.ErrorCode} {load.ErrorText}");
                continue;
            }

            if (load.Data!.Status == TableDocumentStatus.Malformed)
            {
                if (relativePath != "Lookup/AttachmentPoint.xml")
                {
                    failures.Add($"{relativePath}: unexpected malformed document");
                }

                continue;
            }

            var document = load.Data.GetDocument<TableDocument>();
            if (document is null)
            {
                failures.Add($"{relativePath}: no document returned");
                continue;
            }

            var before = await File.ReadAllBytesAsync(file);
            var lastWrite = File.GetLastWriteTimeUtc(file);
            var save = await saver.SaveAsync(document);
            if (!save.IsSuccess)
            {
                failures.Add($"{relativePath}: save failed: {save.ErrorCode} {save.ErrorText}");
                continue;
            }

            var after = await File.ReadAllBytesAsync(file);
            if (!before.AsSpan().SequenceEqual(after))
            {
                failures.Add($"{relativePath}: an unmodified file was rewritten");
            }
            else if (File.GetLastWriteTimeUtc(file) != lastWrite)
            {
                failures.Add($"{relativePath}: an unmodified file's timestamp changed");
            }
        }

        Assert.True(checkedFiles > 0, "The corpus did not contain any XML table files.");
        Assert.True(
            failures.Count == 0,
            $"Corpus integrity failures ({failures.Count} of {checkedFiles}):{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(25))}");
    }
}
