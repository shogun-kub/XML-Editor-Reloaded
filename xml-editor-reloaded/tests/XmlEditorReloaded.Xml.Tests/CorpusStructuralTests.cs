using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Environment-gated structural edits over a real <c>Items.xml</c> corpus file.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class CorpusStructuralTests
{
    /// <summary>
    /// Adding a record and deleting it again restores the original file byte for byte.
    /// </summary>
    [Fact]
    public async Task Corpus_AddThenDeleteRecord_RestoresOriginalBytes()
    {
        var corpusDirectory = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpusDirectory) || !Directory.Exists(corpusDirectory))
        {
            return;
        }

        var source = Path.Combine(corpusDirectory, "Items", "Items.xml");
        if (!File.Exists(source))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        File.Copy(source, path, overwrite: true);
        var originalBytes = await File.ReadAllBytesAsync(path);
        var originalText = await File.ReadAllTextAsync(path);

        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var loader = new TableFileLoader();
        var saver = new TableFileSaver();
        var load = await loader.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var document = load.Data!.GetDocument<TableDocument>()!;

        var added = document.AddRecord();
        Assert.True(added.IsSuccess, added.ErrorText);
        var newKey = added.Data!.GetKey();
        Assert.False(string.IsNullOrEmpty(newKey));
        Assert.True((await saver.SaveAsync(document)).IsSuccess);

        var addedText = await File.ReadAllTextAsync(path);
        var insertPosition = originalText.LastIndexOf("</ITEMLIST>", StringComparison.Ordinal);
        Assert.True(insertPosition > 0);
        Assert.StartsWith(originalText[..insertPosition], addedText, StringComparison.Ordinal);
        Assert.EndsWith(originalText[insertPosition..], addedText, StringComparison.Ordinal);
        Assert.Contains($"<uiIndex>{newKey}</uiIndex>", addedText, StringComparison.Ordinal);

        Assert.True(document.DeleteRecord(newKey!).IsSuccess);
        Assert.True((await saver.SaveAsync(document)).IsSuccess);

        Assert.Equal(originalBytes, await File.ReadAllBytesAsync(path));
    }
}
