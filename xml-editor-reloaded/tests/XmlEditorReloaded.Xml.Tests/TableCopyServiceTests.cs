using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Copy;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableCopyService"/>: conflict policies, key allocation, display-text remapping,
/// related rows, and the read-only guarantee for the source data set.
/// </summary>
public class TableCopyServiceTests
{
    private const string SourceItems =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
        + "<ITEM><uiIndex>5</uiIndex><szItemName>Glock 17</szItemName><usItemClass>2</usItemClass>"
        + "<ubLinkedItem>6</ubLinkedItem><ubSourceOnly>77</ubSourceOnly></ITEM>"
        + "<ITEM><uiIndex>6</uiIndex><szItemName>Clip</szItemName><usItemClass>4</usItemClass></ITEM>"
        + "<ITEM><uiIndex>7</uiIndex><szItemName>Knife</szItemName><usItemClass>4</usItemClass></ITEM>"
        + "</ITEMLIST>";

    private const string TargetItems =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
        + "<ITEM><uiIndex>5</uiIndex><szItemName>Knife</szItemName><usItemClass>4</usItemClass>"
        + "<ubTargetOnly>9</ubTargetOnly></ITEM>"
        + "<ITEM><uiIndex>10</uiIndex><szItemName>Glock 17</szItemName><usItemClass>20</usItemClass></ITEM>"
        + "<ITEM><uiIndex>11</uiIndex><szItemName>Clip</szItemName><usItemClass>40</usItemClass></ITEM>"
        + "</ITEMLIST>";

    private const string SourceWeapons =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><WEAPONLIST>"
        + "<WEAPON><uiIndex>5</uiIndex><ubCalibre>1</ubCalibre></WEAPON>"
        + "</WEAPONLIST>";

    private const string SourceAttachments =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>"
        + "<ATTACHMENT><attachmentIndex>100</attachmentIndex><itemIndex>5</itemIndex><APCost>20</APCost></ATTACHMENT>"
        + "<ATTACHMENT><attachmentIndex>101</attachmentIndex><itemIndex>5</itemIndex><APCost>30</APCost></ATTACHMENT>"
        + "<ATTACHMENT><attachmentIndex>102</attachmentIndex><itemIndex>7</itemIndex><APCost>40</APCost></ATTACHMENT>"
        + "</ATTACHMENTLIST>";

    /// <summary>
    /// A conflicting record is planned as skipped and the preview writes nothing.
    /// </summary>
    [Fact]
    public async Task Preview_SkipPolicy_DoesNotWrite()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var sourceBefore = await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath);
        var targetBefore = await File.ReadAllBytesAsync(data.TargetDescriptor.FilePath);

        var result = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Skip));

        Assert.True(result.IsSuccess, result.ErrorText);
        var preview = result.Data!;
        var record = Assert.Single(preview.Records);
        Assert.Equal(TableCopyAction.Skip, record.Action);
        Assert.Equal("5", record.SourceKey);
        Assert.Equal("5", record.TargetKey);
        Assert.Equal(0, preview.AddCount);
        Assert.False(preview.HasChanges);
        Assert.Equal(sourceBefore, await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath));
        Assert.Equal(targetBefore, await File.ReadAllBytesAsync(data.TargetDescriptor.FilePath));
    }

    /// <summary>
    /// A table that exists only in the source data set cannot be copied into a data set without it: both
    /// the preview and the apply fail with the dedicated "target table missing" error, and nothing is
    /// written into either data set.
    /// </summary>
    [Fact]
    public async Task TargetTableMissing_FailsWithoutWriting()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        File.Delete(data.TargetDescriptor.FilePath);
        var sourceBefore = await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath);
        var targetFilesBefore = SnapshotFiles(data.TargetDirectory);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var request = new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Skip);

        var preview = await service.PreviewAsync(request);

        Assert.False(preview.IsSuccess);
        Assert.Equal(ErrorCodes.CopyTargetTableMissing, preview.ErrorCode);
        Assert.Contains("Items.Items", preview.ErrorText, StringComparison.Ordinal);

        var apply = await service.ApplyAsync(request);

        Assert.False(apply.IsSuccess);
        Assert.Equal(ErrorCodes.CopyTargetTableMissing, apply.ErrorCode);
        Assert.False(File.Exists(data.TargetDescriptor.FilePath));
        Assert.Equal(targetFilesBefore, SnapshotFiles(data.TargetDirectory));
        Assert.Equal(sourceBefore, await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath));
    }

    /// <summary>
    /// A table that exists only in the target data set is not a source table either: the request fails
    /// with a file-not-found error and the target keeps its bytes.
    /// </summary>
    [Fact]
    public async Task SourceTableMissing_FailsWithoutWriting()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        File.Delete(data.SourceDescriptor.FilePath);
        var targetBefore = await File.ReadAllBytesAsync(data.TargetDescriptor.FilePath);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var preview = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Skip));

        Assert.False(preview.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, preview.ErrorCode);
        Assert.False(File.Exists(data.SourceDescriptor.FilePath));
        Assert.Equal(targetBefore, await File.ReadAllBytesAsync(data.TargetDescriptor.FilePath));
    }

    /// <summary>
    /// Snapshot of the file names and bytes of a data directory.
    /// </summary>
    /// <param name="directory">Directory to snapshot.</param>
    /// <returns>Relative path and content pairs in ordinal order.</returns>
    private static List<string> SnapshotFiles(string directory) =>
        Directory.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(path => $"{Path.GetRelativePath(directory, path)}|{Convert.ToHexString(File.ReadAllBytes(path))}")
            .ToList();

    /// <summary>
    /// Overwrite copies every source field, keeps target-only fields, and remaps lookup and relation
    /// references by display text without touching the source.
    /// </summary>
    [Fact]
    public async Task Apply_Overwrite_RemapsReferencesAndKeepsTargetOnlyFields()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var sourceBefore = await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath);

        var result = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Overwrite));

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(1, Assert.Single(result.Data!.Tables).Updated);
        var item = LoadRecord(data.TargetDescriptor.FilePath, "uiIndex", "5");
        Assert.Equal("Glock 17", item.Element("szItemName")?.Value);
        Assert.Equal("20", item.Element("usItemClass")?.Value);
        Assert.Equal("11", item.Element("ubLinkedItem")?.Value);
        Assert.Equal("77", item.Element("ubSourceOnly")?.Value);
        Assert.Equal("9", item.Element("ubTargetOnly")?.Value);
        Assert.Equal(sourceBefore, await File.ReadAllBytesAsync(data.SourceDescriptor.FilePath));
    }

    /// <summary>
    /// Lookup identifiers above 32 bits are remapped by display text instead of a truncated value.
    /// </summary>
    [Fact]
    public async Task Apply_Overwrite_RemapsLookupIdAbove32Bits()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        WriteTable(
            data.SourceDirectory,
            "Lookup/ItemClass.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>8589934592</id><name>Gun</name></ItemClass>"
            + "<ItemClass><id>4</id><name>Misc</name></ItemClass>"
            + "</JA2Data>");
        var sourcePath = data.SourceDescriptor.FilePath;
        await File.WriteAllTextAsync(
            sourcePath,
            (await File.ReadAllTextAsync(sourcePath)).Replace(
                "<usItemClass>2</usItemClass>",
                "<usItemClass>8589934592</usItemClass>",
                StringComparison.Ordinal));
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var result = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Overwrite));

        Assert.True(result.IsSuccess, result.ErrorText);
        var item = LoadRecord(data.TargetDescriptor.FilePath, "uiIndex", "5");
        Assert.Equal("20", item.Element("usItemClass")?.Value);
    }

    /// <summary>
    /// Merge fills only fields that are absent from the target record and keeps existing target values.
    /// </summary>
    [Fact]
    public async Task Apply_Merge_KeepsExistingTargetValues()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var result = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Merge));

        Assert.True(result.IsSuccess, result.ErrorText);
        var item = LoadRecord(data.TargetDescriptor.FilePath, "uiIndex", "5");
        Assert.Equal("Knife", item.Element("szItemName")?.Value);
        Assert.Equal("4", item.Element("usItemClass")?.Value);
        Assert.Equal("11", item.Element("ubLinkedItem")?.Value);
        Assert.Equal("77", item.Element("ubSourceOnly")?.Value);
        Assert.Equal("9", item.Element("ubTargetOnly")?.Value);
    }

    /// <summary>
    /// Allocating keys copies an item together with its weapon and attachment rows, rewrites the
    /// parent links, and leaves the source files byte-identical.
    /// </summary>
    [Fact]
    public async Task Apply_AllocateKeys_CopiesItemAndRelatedRows()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var sourceFiles = data.SourceFiles;
        var sourceBefore = sourceFiles.ToDictionary(
            static path => path,
            static path => File.ReadAllBytes(path),
            StringComparer.OrdinalIgnoreCase);

        var result = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5" },
            TableCopyConflictPolicy.Skip,
            TableCopyKeyMode.Allocate,
            CopyRelatedRecords: true));

        Assert.True(result.IsSuccess, result.ErrorText);
        var tables = result.Data!.Tables.ToDictionary(static table => table.Table.Value, StringComparer.Ordinal);
        Assert.Equal(1, tables["Items.Items"].Added);
        Assert.Equal(1, tables["Items.Weapons"].Added);
        Assert.Equal(2, tables["Items.Attachments"].Added);

        var item = LoadRecord(data.TargetDescriptor.FilePath, "uiIndex", "12");
        Assert.Equal("Glock 17", item.Element("szItemName")?.Value);
        Assert.Equal("20", item.Element("usItemClass")?.Value);
        Assert.Equal("11", item.Element("ubLinkedItem")?.Value);

        var weaponPath = Path.Combine(data.TargetDirectory, "Items", "Weapons.xml");
        var weapon = LoadRecord(weaponPath, "uiIndex", "12");
        Assert.Equal("1", weapon.Element("ubCalibre")?.Value);

        var attachmentPath = Path.Combine(data.TargetDirectory, "Items", "Attachments.xml");
        var attachments = XDocument.Load(attachmentPath).Root!
            .Elements("ATTACHMENT")
            .Where(element => element.Element("itemIndex")?.Value == "12")
            .ToArray();
        Assert.Equal(2, attachments.Length);
        Assert.All(attachments, element => Assert.Equal("12", element.Element("itemIndex")?.Value));
        Assert.Equal(
            new[] { "20", "30" },
            attachments.Select(element => element.Element("APCost")!.Value).OrderBy(static value => value));

        foreach (var (path, bytes) in sourceBefore)
        {
            Assert.Equal(bytes, await File.ReadAllBytesAsync(path));
        }
    }

    /// <summary>
    /// Copying a data set onto itself is refused so the source stays read-only.
    /// </summary>
    [Fact]
    public async Task Apply_SameDirectory_IsRejected()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var result = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.SourceDirectory,
            TableName.Parse("Items.Items")));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.CopySameDirectory, result.ErrorCode);
    }

    /// <summary>
    /// A table that exists only in the source reports a dedicated missing-target error.
    /// </summary>
    [Fact]
    public async Task Preview_TargetWithoutTable_ReportsMissingTable()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        File.Delete(Path.Combine(data.TargetDirectory, "Items", "Items.xml"));
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var result = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items")));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.CopyTargetTableMissing, result.ErrorCode);
    }

    /// <summary>
    /// A requested key that is absent from the source is reported as a warning, and the remaining
    /// records are still copied.
    /// </summary>
    [Fact]
    public async Task Preview_MissingRequestedKey_WarnsAndContinues()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());

        var result = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Items.Items"),
            new[] { "5", "404" },
            TableCopyConflictPolicy.Skip));

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Single(result.Data!.Records);
        Assert.Contains(result.Data.Warnings, warning => warning.Contains("404", StringComparison.Ordinal));
    }

    /// <summary>
    /// A compound-key table preserves every key component, shows display keys in the preview, and
    /// rejects automatic key allocation.
    /// </summary>
    [Fact]
    public async Task CompoundKeyTable_PreservesKeysAndRejectsAllocation()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider(compoundAttachments: true));
        var tableName = TableName.Parse("Items.Attachments");

        var preview = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName));

        Assert.True(preview.IsSuccess, preview.ErrorText);
        Assert.Equal(3, preview.Data!.AddCount);
        Assert.Equal(
            new[] { "100 | 5", "101 | 5", "102 | 7" },
            preview.Data.Records.Select(static record => record.SourceKey));
        Assert.All(
            preview.Data.Records,
            record => Assert.Equal(record.SourceKey, record.TargetKey));

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        var attachmentPath = Path.Combine(data.TargetDirectory, "Items", "Attachments.xml");
        var rows = XDocument.Load(attachmentPath).Root!.Elements("ATTACHMENT").ToArray();
        Assert.Equal(3, rows.Length);
        Assert.Contains(rows, row => row.Element("attachmentIndex")?.Value == "100"
            && row.Element("itemIndex")?.Value == "5");
        Assert.Contains(rows, row => row.Element("attachmentIndex")?.Value == "101"
            && row.Element("itemIndex")?.Value == "5");
        Assert.All(
            rows.Where(row => row.Element("itemIndex")?.Value == "5"),
            row => Assert.Equal("5", row.Element("itemIndex")?.Value));

        var allocate = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            KeyMode: TableCopyKeyMode.Allocate));
        Assert.False(allocate.IsSuccess);
        Assert.Equal(ErrorCodes.CopyAllocateUnsupported, allocate.ErrorCode);
    }

    /// <summary>
    /// A lookup row whose target id is absent but whose display name already exists is skipped under the
    /// skip policy, and the target file keeps exactly one row with that name.
    /// </summary>
    [Fact]
    public async Task LookupCopy_SkipPolicy_SkipsExistingNameRow()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var targetPath = Path.Combine(data.TargetDirectory, "Lookup", "ItemClass.xml");
        var targetBefore = await File.ReadAllBytesAsync(targetPath);
        var tableName = TableName.Parse("Lookup.ItemClass");

        var preview = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(preview.IsSuccess, preview.ErrorText);
        var planned = Assert.Single(preview.Data!.Records);
        Assert.Equal(TableCopyAction.Skip, planned.Action);
        Assert.Equal("20", planned.TargetKey);
        Assert.False(preview.Data.HasChanges);

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        Assert.Equal(1, Assert.Single(apply.Data!.Tables).Skipped);
        var rows = XDocument.Load(targetPath).Root!.Elements("ItemClass").ToArray();
        Assert.Single(rows, row => row.Element("name")?.Value == "Gun");
        Assert.Equal(targetBefore, await File.ReadAllBytesAsync(targetPath));
    }

    /// <summary>
    /// The overwrite policy rewrites the target row that already carries the source display name and keeps
    /// that row's own key, so no duplicate name row is appended.
    /// </summary>
    [Fact]
    public async Task LookupCopy_OverwritePolicy_UpdatesExistingNameRow()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var targetPath = Path.Combine(data.TargetDirectory, "Lookup", "ItemClass.xml");

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Lookup.ItemClass"),
            new[] { "2" },
            TableCopyConflictPolicy.Overwrite));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        var table = Assert.Single(apply.Data!.Tables);
        Assert.Equal(1, table.Updated);
        Assert.Equal(0, table.Added);

        var rows = XDocument.Load(targetPath).Root!.Elements("ItemClass").ToArray();
        Assert.Equal(2, rows.Length);
        var gun = Assert.Single(rows, row => row.Element("name")?.Value == "Gun");
        Assert.Equal("20", gun.Element("id")?.Value);
        Assert.DoesNotContain(rows, row => row.Element("id")?.Value == "2");
    }

    /// <summary>
    /// Allocate mode still resolves an absent id by display name, so it cannot append a duplicate name;
    /// the skip policy leaves the target file unchanged.
    /// </summary>
    [Fact]
    public async Task LookupCopy_AllocateMode_SkipsExistingNameRow()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var targetPath = Path.Combine(data.TargetDirectory, "Lookup", "ItemClass.xml");
        var targetBefore = await File.ReadAllBytesAsync(targetPath);

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            TableName.Parse("Lookup.ItemClass"),
            new[] { "2" },
            TableCopyConflictPolicy.Skip,
            TableCopyKeyMode.Allocate));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        var table = Assert.Single(apply.Data!.Tables);
        Assert.Equal(0, table.Added);
        Assert.Equal(1, table.Skipped);
        Assert.Equal(targetBefore, await File.ReadAllBytesAsync(targetPath));
    }

    /// <summary>
    /// When the source display name is absent from the target, the alternate search finds no row, so the
    /// record is added and the source key is preserved.
    /// </summary>
    [Fact]
    public async Task LookupCopy_AbsentDisplayName_AddsRowWithSourceKey()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        await File.WriteAllTextAsync(
            Path.Combine(data.SourceDirectory, "Lookup", "ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>2</id><name>Rifle</name></ItemClass>"
            + "</JA2Data>");
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var targetPath = Path.Combine(data.TargetDirectory, "Lookup", "ItemClass.xml");
        var tableName = TableName.Parse("Lookup.ItemClass");

        var preview = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(preview.IsSuccess, preview.ErrorText);
        var planned = Assert.Single(preview.Data!.Records);
        Assert.Equal(TableCopyAction.Add, planned.Action);
        Assert.Equal("2", planned.TargetKey);
        Assert.Empty(planned.Warnings);

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        var table = Assert.Single(apply.Data!.Tables);
        Assert.Equal(1, table.Added);
        Assert.Equal(0, table.Updated);

        var added = LoadRecord(targetPath, "name", "Rifle");
        Assert.Equal("2", added.Element("id")?.Value);
        Assert.Equal(3, XDocument.Load(targetPath).Root!.Elements("ItemClass").Count());
    }

    /// <summary>
    /// Two target rows with the same display name cannot be resolved unambiguously: the row is added, the
    /// plan carries a warning that names the comparison field, and the existing rows stay unchanged.
    /// </summary>
    [Fact]
    public async Task LookupCopy_AmbiguousDisplayName_AddsRowAndWarns()
    {
        using var workspace = new TestWorkspace();
        var data = CreateData(workspace);
        var targetPath = Path.Combine(data.TargetDirectory, "Lookup", "ItemClass.xml");
        await File.WriteAllTextAsync(
            targetPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>20</id><name>Gun</name></ItemClass>"
            + "<ItemClass><id>40</id><name>Gun</name></ItemClass>"
            + "</JA2Data>");
        var service = new TableCopyService(new DirectoryCatalog(), new TestMetadataProvider());
        var tableName = TableName.Parse("Lookup.ItemClass");

        var preview = await service.PreviewAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(preview.IsSuccess, preview.ErrorText);
        var planned = Assert.Single(preview.Data!.Records);
        Assert.Equal(TableCopyAction.Add, planned.Action);
        Assert.Contains(
            planned.Warnings,
            warning => warning.Contains("'name'", StringComparison.Ordinal)
                && warning.Contains("2 target records", StringComparison.Ordinal));

        var apply = await service.ApplyAsync(new TableCopyRequest(
            data.SourceDirectory,
            data.TargetDirectory,
            tableName,
            new[] { "2" },
            TableCopyConflictPolicy.Skip));

        Assert.True(apply.IsSuccess, apply.ErrorText);
        var table = Assert.Single(apply.Data!.Tables);
        Assert.Equal(1, table.Added);
        Assert.Equal(0, table.Updated);
        Assert.Contains(
            apply.Data.Warnings,
            warning => warning.Contains("'name'", StringComparison.Ordinal)
                && warning.Contains("2 target records", StringComparison.Ordinal));

        var rows = XDocument.Load(targetPath).Root!.Elements("ItemClass").ToArray();
        Assert.Equal(3, rows.Length);
        Assert.Contains(rows, row => row.Element("id")?.Value == "20" && row.Element("name")?.Value == "Gun");
        Assert.Contains(rows, row => row.Element("id")?.Value == "40" && row.Element("name")?.Value == "Gun");
        var added = Assert.Single(rows, row => row.Element("id")?.Value == "2");
        Assert.Equal("Gun", added.Element("name")?.Value);
    }

    private static CopyData CreateData(TestWorkspace workspace)
    {
        var source = Path.Combine(workspace.DirectoryPath, "source");
        var target = Path.Combine(workspace.DirectoryPath, "target");
        WriteTable(source, "Items/Items.xml", SourceItems);
        WriteTable(source, "Items/Weapons.xml", SourceWeapons);
        WriteTable(source, "Items/Attachments.xml", SourceAttachments);
        WriteTable(
            source,
            "Lookup/ItemClass.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>2</id><name>Gun</name></ItemClass>"
            + "<ItemClass><id>4</id><name>Misc</name></ItemClass>"
            + "</JA2Data>");

        WriteTable(target, "Items/Items.xml", TargetItems);
        WriteTable(
            target,
            "Items/Weapons.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><WEAPONLIST></WEAPONLIST>");
        WriteTable(
            target,
            "Items/Attachments.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST></ATTACHMENTLIST>");
        WriteTable(
            target,
            "Lookup/ItemClass.xml",
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>20</id><name>Gun</name></ItemClass>"
            + "<ItemClass><id>40</id><name>Misc</name></ItemClass>"
            + "</JA2Data>");

        return new CopyData(
            source,
            target,
            new TableDescriptor(
                TableName.Parse("Items.Items"),
                Path.Combine(source, "Items", "Items.xml"),
                "Items/Items.xml",
                true,
                Array.Empty<OverlayDescriptor>()),
            new TableDescriptor(
                TableName.Parse("Items.Items"),
                Path.Combine(target, "Items", "Items.xml"),
                "Items/Items.xml",
                true,
                Array.Empty<OverlayDescriptor>()));
    }

    private static void WriteTable(string root, string relativePath, string content)
    {
        var path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    private static XElement LoadRecord(string path, string field, string value) =>
        XDocument.Load(path).Root!
            .Elements()
            .Single(element => element.Element(field)?.Value == value);

    private sealed record CopyData(
        string SourceDirectory,
        string TargetDirectory,
        TableDescriptor SourceDescriptor,
        TableDescriptor TargetDescriptor)
    {
        public IReadOnlyList<string> SourceFiles { get; } = Directory
            .EnumerateFiles(SourceDirectory, "*.xml", SearchOption.AllDirectories)
            .ToArray();
    }

    private sealed class DirectoryCatalog : ITableCatalog
    {
        public Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
            string tableDataDirectory,
            CancellationToken cancellationToken = default)
        {
            var descriptors = new List<TableDescriptor>();
            foreach (var file in Directory.EnumerateFiles(tableDataDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(tableDataDirectory, file).Replace('\\', '/');
                if (!TableName.TryFromRelativePath(relativePath, out var tableName))
                {
                    continue;
                }

                descriptors.Add(new TableDescriptor(
                    tableName,
                    file,
                    relativePath,
                    IsMapped: true,
                    Array.Empty<OverlayDescriptor>()));
            }

            descriptors.Sort(static (left, right) => left.TableName.CompareTo(right.TableName));
            return Task.FromResult(OperationResult<IReadOnlyList<TableDescriptor>>.Success(descriptors));
        }
    }

    private sealed class TestMetadataProvider : ITableMetadataProvider
    {
        private readonly Dictionary<TableName, TableMetadata> _tables;
        private readonly Dictionary<TableName, IReadOnlyList<TableRelation>> _relations;

        public TestMetadataProvider(bool compoundAttachments = false)
        {
            var items = TableName.Parse("Items.Items");
            var weapons = TableName.Parse("Items.Weapons");
            var attachments = TableName.Parse("Items.Attachments");
            _tables = new[]
            {
                new TableMetadata(
                    items,
                    "ITEMLIST",
                    "ITEM",
                    isLookup: false,
                    new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
                    columns: new[]
                    {
                        new ColumnMetadata("uiIndex", ColumnDataType.Integer),
                        new ColumnMetadata("szItemName", ColumnDataType.String),
                        new ColumnMetadata("usItemClass", ColumnDataType.Lookup, ColumnEditorKind.LookupCombo)
                        {
                            LookupTable = "Lookup.ItemClass",
                        },
                        new ColumnMetadata("ubLinkedItem", ColumnDataType.Integer),
                    },
                    columnHints: new[] { new ColumnHint("usItemClass") { LookupTable = "Lookup.ItemClass" } }),
                new TableMetadata(
                    weapons,
                    "WEAPONLIST",
                    "WEAPON",
                    isLookup: false,
                    new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
                    columns: new[]
                    {
                        new ColumnMetadata("uiIndex", ColumnDataType.Integer),
                        new ColumnMetadata("ubCalibre", ColumnDataType.Integer),
                    }),
                compoundAttachments
                    ? new TableMetadata(
                        attachments,
                        "ATTACHMENTLIST",
                        "ATTACHMENT",
                        isLookup: false,
                        columns: new[]
                        {
                            new ColumnMetadata("attachmentIndex", ColumnDataType.Integer),
                            new ColumnMetadata("itemIndex", ColumnDataType.Integer),
                            new ColumnMetadata("APCost", ColumnDataType.Integer),
                        },
                        recordKeyFields: new[] { "attachmentIndex", "itemIndex" })
                    : new TableMetadata(
                        attachments,
                        "ATTACHMENTLIST",
                        "ATTACHMENT",
                        isLookup: false,
                        new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "attachmentIndex") }),
                        columns: new[]
                        {
                            new ColumnMetadata("attachmentIndex", ColumnDataType.Integer),
                            new ColumnMetadata("itemIndex", ColumnDataType.Integer),
                            new ColumnMetadata("APCost", ColumnDataType.Integer),
                        }),
                new TableMetadata(
                    TableName.Parse("Lookup.ItemClass"),
                    "JA2Data",
                    "ItemClass",
                    isLookup: true,
                    columnHints: new[] { new ColumnHint("name") { Flags = ColumnFlags.ComparisonField } }),
            }.ToDictionary(static table => table.TableName);

            _relations = new Dictionary<TableName, IReadOnlyList<TableRelation>>
            {
                [items] = new[]
                {
                    new TableRelation(items, "uiIndex", weapons, TableRelationKind.SubTable),
                    new TableRelation(
                        items,
                        "uiIndex",
                        attachments,
                        TableRelationKind.ChildRows,
                        "itemIndex",
                        "attachmentIndex"),
                    new TableRelation(
                        items,
                        "ubLinkedItem",
                        items,
                        TableRelationKind.Reference,
                        "uiIndex",
                        "szItemName")
                    {
                        RemapByText = true,
                    },
                },
            };
        }

        public Task<OperationResult<TableMetadata>> GetTableAsync(
            TableName tableName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_tables.TryGetValue(tableName, out var table)
                ? OperationResult<TableMetadata>.Success(table)
                : OperationResult<TableMetadata>.Failure(
                    ErrorCodes.MetadataNotFound,
                    $"Table '{tableName}' is not present in the metadata store."));

        public Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableMetadata>>.Success(_tables.Values.ToArray()));

        public Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(
                Array.Empty<LanguageDescriptor>()));

        public Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
            TableName sourceTable,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableRelation>>.Success(
                _relations.GetValueOrDefault(sourceTable) ?? Array.Empty<TableRelation>()));

        public Task<OperationResult<IReadOnlyList<LookupEntry>>> GetLookupValuesAsync(
            TableName lookupName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LookupEntry>>.Success(
                Array.Empty<LookupEntry>()));
    }
}
