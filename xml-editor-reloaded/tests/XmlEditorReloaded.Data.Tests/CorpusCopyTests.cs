using System.Security.Cryptography;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Copy;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated cross-data-set copy over two real <c>TableData</c> directories. Set
/// <c>XER_COPY_SOURCE_DIR</c> and <c>XER_COPY_TARGET_DIR</c> to v4 and v5 <c>TableData</c>
/// directories to enable the test; the <c>Items</c> and <c>Lookup</c> folders of both directories are
/// copied to a temporary workspace first.
/// </summary>
public class CorpusCopyTests
{
    private static readonly TableName Items = TableName.Parse("Items.Items");

    /// <summary>
    /// Copies a weapon item with new keys, rewrites its related rows and caliber reference, keeps the
    /// source byte-identical, and leaves a readable target table.
    /// </summary>
    [Fact]
    public async Task Copy_WeaponItemFromSourceToTarget_RemapsAndKeepsSourceReadOnly()
    {
        var sourceDirectory = Environment.GetEnvironmentVariable("XER_COPY_SOURCE_DIR");
        var targetDirectory = Environment.GetEnvironmentVariable("XER_COPY_TARGET_DIR");
        if (string.IsNullOrWhiteSpace(sourceDirectory)
            || string.IsNullOrWhiteSpace(targetDirectory)
            || !Directory.Exists(sourceDirectory)
            || !Directory.Exists(targetDirectory))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var source = workspace.PathOf("source");
        var target = workspace.PathOf("target");
        foreach (var folder in new[] { "Items", "Lookup" })
        {
            var sourceFolder = Path.Combine(sourceDirectory, folder);
            var targetFolder = Path.Combine(targetDirectory, folder);
            if (Directory.Exists(sourceFolder))
            {
                CopyDirectory(sourceFolder, Path.Combine(source, folder));
            }

            if (Directory.Exists(targetFolder))
            {
                CopyDirectory(targetFolder, Path.Combine(target, folder));
            }
        }

        var sourceItemsPath = Path.Combine(source, "Items", "Items.xml");
        var sourceWeaponsPath = Path.Combine(source, "Items", "Weapons.xml");
        var sourceAttachmentsPath = Path.Combine(source, "Items", "Attachments.xml");
        if (!File.Exists(sourceItemsPath)
            || !File.Exists(sourceWeaponsPath)
            || !File.Exists(sourceAttachmentsPath))
        {
            return;
        }

        var sourceItems = XDocument.Load(sourceItemsPath).Root!;
        var sourceWeapons = XDocument.Load(sourceWeaponsPath).Root!;
        var sourceAttachments = XDocument.Load(sourceAttachmentsPath).Root!;
        var candidate = FindCandidateItem(sourceItems, sourceWeapons, sourceAttachments);
        Assert.NotNull(candidate);

        var sourceKey = candidate!.Element("uiIndex")!.Value;
        var sourceName = candidate.Element("szItemName")!.Value;
        var sourceWeapon = sourceWeapons.Elements("WEAPON")
            .Single(element => element.Element("uiIndex")?.Value == sourceKey);
        var sourceCaliber = sourceWeapon.Element("ubCalibre")!.Value;
        var sourceRows = sourceAttachments.Elements("ATTACHMENT")
            .Where(element => element.Element("itemIndex")?.Value == sourceKey)
            .ToArray();

        var sourceHashes = HashFiles(source);
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var service = new TableCopyService(catalog, provider);
        var request = new TableCopyRequest(
            source,
            target,
            Items,
            new[] { sourceKey },
            TableCopyConflictPolicy.Skip,
            TableCopyKeyMode.Allocate,
            CopyRelatedRecords: true);

        var preview = await service.PreviewAsync(request);
        Assert.True(preview.IsSuccess, preview.ErrorText);
        var plan = Assert.Single(preview.Data!.Records);
        Assert.Equal(TableCopyAction.Add, plan.Action);
        var targetKey = Assert.IsType<string>(plan.TargetKey);
        Assert.NotEqual(sourceKey, targetKey);
        var related = plan.Related.ToDictionary(
            static item => item.Table.Value,
            StringComparer.Ordinal);
        Assert.Equal(1, related["Items.Weapons"].AddCount);
        Assert.Equal(sourceRows.Length, related["Items.Attachments"].AddCount);

        var applied = await service.ApplyAsync(request);
        Assert.True(applied.IsSuccess, applied.ErrorText);
        var savedFiles = applied.Data!.SavedFiles
            .Select(Path.GetFileName)
            .ToArray();
        Assert.Contains("Items.xml", savedFiles);
        Assert.Contains("Weapons.xml", savedFiles);
        Assert.Contains("Attachments.xml", savedFiles);

        var targetItems = XDocument.Load(Path.Combine(target, "Items", "Items.xml")).Root!;
        var copiedItem = targetItems.Elements("ITEM")
            .SingleOrDefault(element => element.Element("uiIndex")?.Value == targetKey);
        Assert.NotNull(copiedItem);
        Assert.Equal(sourceName, copiedItem!.Element("szItemName")?.Value);
        Assert.Equal(
            candidate.Elements().Select(static element => element.Name.LocalName).ToHashSet(),
            copiedItem.Elements().Select(static element => element.Name.LocalName).ToHashSet());

        var targetWeapons = XDocument.Load(Path.Combine(target, "Items", "Weapons.xml")).Root!;
        var copiedWeapon = targetWeapons.Elements("WEAPON")
            .SingleOrDefault(element => element.Element("uiIndex")?.Value == targetKey);
        Assert.NotNull(copiedWeapon);
        var targetCaliber = copiedWeapon!.Element("ubCalibre")!.Value;
        Assert.Equal(
            ReadCaliber(source, sourceCaliber),
            ReadCaliber(target, targetCaliber));

        var targetAttachments = XDocument.Load(Path.Combine(target, "Items", "Attachments.xml")).Root!;
        var copiedRows = targetAttachments.Elements("ATTACHMENT")
            .Where(element => element.Element("itemIndex")?.Value == targetKey)
            .ToArray();
        Assert.Equal(sourceRows.Length, copiedRows.Length);
        Assert.Equal(
            sourceRows.Select(static element => element.Element("APCost")?.Value).OrderBy(static value => value),
            copiedRows.Select(static element => element.Element("APCost")?.Value).OrderBy(static value => value));

        foreach (var (path, hash) in sourceHashes)
        {
            Assert.Equal(hash, HashFile(path));
        }
    }

    private static XElement? FindCandidateItem(
        XElement items,
        XElement weapons,
        XElement attachments)
    {
        foreach (var item in items.Elements("ITEM"))
        {
            var key = item.Element("uiIndex")?.Value;
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var weapon = weapons.Elements("WEAPON")
                .FirstOrDefault(element => element.Element("uiIndex")?.Value == key);
            var caliber = weapon?.Element("ubCalibre")?.Value;
            if (string.IsNullOrWhiteSpace(caliber) || caliber == "0")
            {
                continue;
            }

            var hasAttachments = attachments.Elements("ATTACHMENT")
                .Any(element => element.Element("itemIndex")?.Value == key);
            if (hasAttachments && item.Element("szItemName")?.Value is { Length: > 0 })
            {
                return item;
            }
        }

        return null;
    }

    private static string? ReadCaliber(string tableDataDirectory, string value)
    {
        var path = Path.Combine(tableDataDirectory, "Items", "AmmoStrings.xml");
        if (!File.Exists(path))
        {
            return null;
        }

        return XDocument.Load(path).Root!
            .Elements("AMMO")
            .FirstOrDefault(element => element.Element("uiIndex")?.Value == value)
            ?.Element("AmmoCaliber")
            ?.Value;
    }

    private static Dictionary<string, string> HashFiles(string directory) =>
        Directory.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
            .ToDictionary(
                static path => path,
                HashFile,
                StringComparer.OrdinalIgnoreCase);

    private static string HashFile(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var directory in Directory.EnumerateDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}
