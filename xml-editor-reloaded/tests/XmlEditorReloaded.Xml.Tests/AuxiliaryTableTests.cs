using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for auxiliary table shapes introduced with M20: text-only records with positional keys and
/// repeated gear kits addressed by occurrence.
/// </summary>
public class AuxiliaryTableTests
{
    /// <summary>
    /// A text-only table exposes each record text as a virtual column, identifies records by position,
    /// and writes an edit back as record text instead of adding a child element.
    /// </summary>
    [Fact]
    public async Task TextOnlyTable_UsesPositionalKeysAndEditableRecordText()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Sounds.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><SOUNDLIST>\r\n"
            + "\t<SOUND>SOUNDS\\A.wav</SOUND>\r\n"
            + "\t<SOUND>SOUNDS\\B.wav</SOUND>\r\n"
            + "</SOUNDLIST>\r\n");
        var metadata = SoundsMetadata();
        var load = await TableWorkspace.LoadAsync(Descriptor(path, metadata), metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.Equal("0", table.FindRecord("0")!.GetKey());
        Assert.Equal("1", table.FindRecord("1")!.GetKey());
        Assert.Equal("SOUNDS\\B.wav", table.FindRecord("1")!.GetRawValue(RecordFields.RecordText));

        var schema = new TableSchemaBuilder().Build(table.BaseDocument);
        Assert.Equal(new[] { RecordFields.RecordText }, schema.Columns.Select(static column => column.XmlFieldName));
        Assert.Null(schema.KeyColumn);
        Assert.Equal(2, schema.FindColumn(RecordFields.RecordText)!.PresenceCount);

        var edit = table.SetValue("1", RecordFields.RecordText, "SOUNDS\\C.wav");
        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.Equal("SOUNDS\\C.wav", table.FindRecord("1")!.GetRawValue(RecordFields.RecordText));
        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("SOUNDS\\B.wav", table.FindRecord("1")!.GetRawValue(RecordFields.RecordText));

        Assert.True(table.SetValue("1", RecordFields.RecordText, "SOUNDS\\C.wav").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        var saved = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        Assert.Equal(
            new[] { "SOUNDS\\A.wav", "SOUNDS\\C.wav" },
            saved.Root!.Elements("SOUND").Select(static element => element.Value));
        Assert.All(saved.Root.Elements("SOUND"), static element => Assert.False(element.HasElements));
    }

    /// <summary>
    /// An occurrence-indexed edit changes only the selected gear kit, keeps the other kits untouched,
    /// and a missing occurrence is rejected instead of being created.
    /// </summary>
    [Fact]
    public async Task MercGear_OccurrenceEditTargetsOneKit()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("Overlays/mercgear-base.xml");
        var metadata = OverlayTestData.MercGearMetadata();
        var load = await TableWorkspace.LoadAsync(Descriptor(path, metadata), metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var record = table.FindRecord("0")!;

        Assert.Equal("6", record.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));

        var edit = table.SetValue("0", "GEARKIT/mWeapon", 1, "60");
        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.Equal("5", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 0));
        Assert.Equal("60", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));
        Assert.Equal("7", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 2));
        var change = Assert.IsType<FieldValueChanged>(
            Assert.Single(table.BaseDocument.ChangeTracker.Changes));
        Assert.Equal("GEARKIT[1]/mWeapon", change.FieldPath);

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("6", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));
        Assert.True(table.Redo().IsSuccess);
        Assert.Equal("60", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));

        var missing = table.SetValue("0", "GEARKIT/mWeapon", 5, "1", addIfMissing: true);
        Assert.False(missing.IsSuccess);
        Assert.Equal(ErrorCodes.KeyNotFound, missing.ErrorCode);
        Assert.Equal("60", table.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));

        Assert.True((await table.SaveAsync()).IsSuccess);
        var saved = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var kits = saved.Root!.Elements("MERCGEAR")
            .First(static element => element.Element("mIndex")?.Value == "0")
            .Elements("GEARKIT")
            .ToArray();
        Assert.Equal(new[] { "5", "60", "7" }, kits.Select(static kit => kit.Element("mWeapon")!.Value));
        Assert.Equal(new[] { "Standard", "Mechanic", "Fighter" }, kits.Select(static kit => kit.Element("mGearKitName")!.Value));
    }

    /// <summary>
    /// With an overlay selected, an occurrence-indexed kit name must be written through the nested
    /// overlay key so the base table and the other kits stay unchanged.
    /// </summary>
    [Fact]
    public async Task MercGear_TranslatedKitName_RequiresNestedOverlayKey()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/mercgear-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/mercgear-ru.xml");
        var metadata = OverlayTestData.MercGearMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Inventory/MercStartingGear.xml",
            IsMapped: true,
            new[] { new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath) });
        var beforeBase = await File.ReadAllBytesAsync(basePath);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var wrong = table.SetValue("0", "GEARKIT/mGearKitName", 1, "Техник");
        Assert.False(wrong.IsSuccess);
        Assert.Equal(ErrorCodes.OverlayUnresolvableKey, wrong.ErrorCode);
        Assert.False(table.IsDirty);

        var nested = table.SetNestedValue(
            new OverlayRecordKey("0", new[] { "1" }),
            FieldPath.Parse("GEARKIT/mGearKitName"),
            "Техник");
        Assert.True(nested.IsSuccess, nested.ErrorText);
        Assert.True(table.IsDirty);
        Assert.True((await table.SaveAsync()).IsSuccess);

        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(basePath));
        var saved = XDocument.Load(overlayPath);
        var kits = saved.Root!.Elements("MERCGEAR")
            .First(static element => element.Element("mIndex")?.Value == "0")
            .Elements("GEARKIT")
            .ToArray();
        Assert.Equal("Техник", kits[1].Element("mGearKitName")?.Value);
        Assert.Equal("Стандарт", kits[0].Element("mGearKitName")?.Value);
    }

    private static TableMetadata SoundsMetadata() => new(
        TableName.Parse("Sounds.Sounds"),
        "SOUNDLIST",
        "SOUND",
        isLookup: false,
        new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Positional) }));

    private static TableDescriptor Descriptor(string path, TableMetadata metadata) =>
        new(
            metadata.TableName,
            path,
            "Sounds/Sounds.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
}
