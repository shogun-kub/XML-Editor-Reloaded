using System.Xml.Linq;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// M5 tests for overlay merge, translation writes, routing, and translation analysis.
/// </summary>
public class OverlayTests
{
    /// <summary>
    /// Base and translated values are merged by primary key.
    /// </summary>
    [Fact]
    public async Task Merge_Items_CombinesBaseAndOverlayValues()
    {
        using var workspace = new TestWorkspace();
        var (basePath, overlayPath, metadata) = CopyItems(workspace);
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);

        var merged = new OverlayMerger(metadata).Merge(baseDocument, overlay.Document);

        Assert.Equal(3, merged.Records.Count);
        var translated = Assert.Single(merged.Records, record => record.Key == "1");
        var name = Assert.Single(translated.Cells, cell => cell.FieldPath == "szItemName");
        Assert.Equal("Glock 17", name.BaseValue);
        Assert.Equal("Глок 17", name.OverlayValue);
        Assert.Equal("Глок 17", name.Value);
        Assert.True(name.IsTranslated);

        var untranslated = Assert.Single(merged.Records, record => record.Key == "0");
        var baseOnly = Assert.Single(untranslated.Cells, cell => cell.FieldPath == "szItemName");
        Assert.Null(baseOnly.OverlayValue);
        Assert.False(baseOnly.IsTranslated);
    }

    /// <summary>
    /// Saving a translation changes the overlay but leaves the base file byte-identical.
    /// </summary>
    [Fact]
    public async Task Save_Translation_DoesNotTouchBaseFile()
    {
        using var workspace = new TestWorkspace();
        var (basePath, overlayPath, metadata) = CopyItems(workspace);
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);
        var beforeBase = await File.ReadAllBytesAsync(basePath);
        var baseWriteTime = File.GetLastWriteTimeUtc(basePath);
        var session = new TableEditSession(baseDocument, metadata, overlay);

        var edit = session.SetValue("1", FieldPath.Parse("szItemName"), "Глок 18");
        var save = await session.SaveAsync();

        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.True(save.IsSuccess, save.ErrorText);
        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(basePath));
        Assert.Equal(baseWriteTime, File.GetLastWriteTimeUtc(basePath));
        var saved = XDocument.Load(overlayPath);
        var item = Assert.Single(saved.Root!.Elements("ITEM"), element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal("Глок 18", item.Element("szItemName")?.Value);
    }

    /// <summary>
    /// A new overlay record contains only the key and the edited translated field.
    /// </summary>
    [Fact]
    public async Task Save_NewTranslation_AddsOnlyKeyAndField()
    {
        using var workspace = new TestWorkspace();
        var (basePath, overlayPath, metadata) = CopyItems(workspace);
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);
        var session = new TableEditSession(baseDocument, metadata, overlay);

        Assert.True(session.SetValue("2", FieldPath.Parse("szItemName"), "Нож").IsSuccess);
        Assert.True((await session.SaveAsync()).IsSuccess);

        var saved = XDocument.Load(overlayPath);
        var item = Assert.Single(saved.Root!.Elements("ITEM"), element => element.Element("uiIndex")?.Value == "2");
        Assert.Equal(2, item.Elements().Count());
        Assert.Equal("Нож", item.Element("szItemName")?.Value);
    }

    /// <summary>
    /// Editing a pocket popup name preserves the structural popup data in the overlay.
    /// </summary>
    [Fact]
    public async Task Save_PocketPopup_PreservesStructuralNodes()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/pockets-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/pockets-ru.xml");
        var metadata = OverlayTestData.PocketsMetadata();
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);
        var beforeBase = await File.ReadAllBytesAsync(basePath);
        var session = new TableEditSession(baseDocument, metadata, overlay);

        Assert.True(session.SetValue("1", FieldPath.Parse("pName"), "Ремень").IsSuccess);
        Assert.True((await session.SaveAsync()).IsSuccess);

        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(basePath));
        var saved = XDocument.Load(overlayPath);
        var pocket = Assert.Single(saved.Root!.Elements("POCKET"));
        Assert.Equal("Ремень", pocket.Element("pName")?.Value);
        Assert.Equal("addWeaponGroups", pocket.Element("popup")?.Element("generator")?.Element("id")?.Value);
    }

    /// <summary>
    /// Nested positional gear-kit translations are merged and written by position.
    /// </summary>
    [Fact]
    public async Task Save_NestedGearKit_UpdatesSelectedKit()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/mercgear-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/mercgear-ru.xml");
        var metadata = OverlayTestData.MercGearMetadata();
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);
        var beforeBase = await File.ReadAllBytesAsync(basePath);

        var merged = new OverlayMerger(metadata).Merge(baseDocument, overlay.Document);
        var record = Assert.Single(merged.Records, item => item.Key == "0");
        Assert.Equal("Стандарт", Assert.Single(record.Cells, cell => cell.FieldPath == "GEARKIT[0]/mGearKitName").OverlayValue);
        Assert.Null(Assert.Single(record.Cells, cell => cell.FieldPath == "GEARKIT[2]/mGearKitName").OverlayValue);

        var session = new TableEditSession(baseDocument, metadata, overlay);
        var key = new OverlayRecordKey("0", new[] { "1" });
        Assert.True(session.SetNestedValue(key, FieldPath.Parse("GEARKIT/mGearKitName"), "Техник").IsSuccess);
        Assert.True((await session.SaveAsync()).IsSuccess);

        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(basePath));
        var saved = XDocument.Load(overlayPath);
        var merc = Assert.Single(saved.Root!.Elements("MERCGEAR"), element => element.Element("mIndex")?.Value == "0");
        var kits = merc.Elements("GEARKIT").ToArray();
        Assert.Equal("Техник", kits[1].Element("mGearKitName")?.Value);
    }

    /// <summary>
    /// City names and the country singleton are merged and written to their nested locations.
    /// </summary>
    [Fact]
    public async Task Save_Cities_UpdatesCountryAndCityName()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/cities-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/cities-ru.xml");
        var metadata = OverlayTestData.CitiesMetadata();
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);
        var beforeBase = await File.ReadAllBytesAsync(basePath);

        var merged = new OverlayMerger(metadata).Merge(baseDocument, overlay.Document);
        Assert.Equal("Арулько", Assert.Single(merged.RootCells, cell => cell.FieldPath == "COUNTRY/countryName").OverlayValue);
        Assert.Equal("Омерта", Assert.Single(merged.Records, record => record.Key == "1").Cells
            .Single(cell => cell.FieldPath == "townName").OverlayValue);
        Assert.Null(Assert.Single(merged.Records, record => record.Key == "2").Cells
            .Single(cell => cell.FieldPath == "townName").OverlayValue);

        var session = new TableEditSession(baseDocument, metadata, overlay);
        Assert.True(session.SetRootValue(FieldPath.Parse("COUNTRY/countryName"), "Арулько 2").IsSuccess);
        Assert.True(session.SetValue("1", FieldPath.Parse("townName"), "Омерта 2").IsSuccess);
        Assert.True((await session.SaveAsync()).IsSuccess);

        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(basePath));
        var saved = XDocument.Load(overlayPath);
        Assert.Equal("Арулько 2", saved.Root!.Element("COUNTRY")?.Element("countryName")?.Value);
        var city = Assert.Single(saved.Root.Element("CITYLIST")!.Elements("CITY"), element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal("Омерта 2", city.Element("townName")?.Value);
    }

    /// <summary>
    /// The analyzer reports missing, orphaned, duplicate, empty, identical, and unresolvable keys.
    /// </summary>
    [Fact]
    public async Task Analyze_ReportsAllTranslationIssueKinds()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/items-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/items-ru-problems.xml");
        var metadata = OverlayTestData.ItemsMetadata();
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);

        var report = new TranslationAnalyzer().Analyze(baseDocument, overlay.Document);

        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.MissingTranslation && issue.RecordKey == "2");
        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.OrphanKey && issue.RecordKey == "99");
        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.DuplicateKey && issue.RecordKey == "1");
        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.EmptyValue && issue.RecordKey == "1");
        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.IdenticalToBase && issue.RecordKey == "0");
        Assert.Contains(report.Issues, issue => issue.Kind == TranslationIssueKind.UnresolvableKey);
    }

    /// <summary>
    /// The analyzer reports a gear-kit count mismatch between base and overlay.
    /// </summary>
    [Fact]
    public async Task Analyze_ReportsNestedKeyCountMismatch()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/mercgear-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/mercgear-ru.xml");
        var metadata = OverlayTestData.MercGearMetadata();
        var baseDocument = await OverlayTestData.LoadBaseAsync(basePath, metadata);
        var overlay = await OverlayTestData.LoadOverlayAsync(basePath, overlayPath, metadata);

        var report = new TranslationAnalyzer().Analyze(baseDocument, overlay.Document);

        Assert.Contains(report.Issues, issue =>
            issue.Kind == TranslationIssueKind.KeyCountMismatch
            && issue.RecordKey == "0"
            && issue.FieldPath == "GEARKIT");
    }

    private static (string BasePath, string OverlayPath, TableMetadata Metadata) CopyItems(TestWorkspace workspace) =>
        (workspace.CopyFixture("Overlays/items-base.xml"),
            workspace.CopyFixture("Overlays/items-ru.xml"),
            OverlayTestData.ItemsMetadata());
}
