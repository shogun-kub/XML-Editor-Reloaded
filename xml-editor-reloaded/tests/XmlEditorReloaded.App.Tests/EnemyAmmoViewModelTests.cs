using System.Globalization;
using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the seven-row enemy ammunition editor. The row values are ammunition types
/// (<c>Items.AmmoTypes</c>), not item identifiers.
/// </summary>
public class EnemyAmmoViewModelTests
{
    private static readonly string[] ChoiceFiles =
    {
        "EnemyItemChoices",
        "ItemChoices_Enemy_Admin",
        "ItemChoices_Enemy_Regular",
        "ItemChoices_Enemy_Elite",
        "ItemChoices_Militia_Green",
        "ItemChoices_Militia_Regular",
        "ItemChoices_Militia_Elite",
    };

    /// <summary>
    /// Loading builds seven rows whose first cells display ammunition names, not item names.
    /// </summary>
    [Fact]
    public async Task Initialize_LoadsSevenRowsWithAmmunitionNames()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateFixture(workspace);
        var (viewModel, _) = await CreateViewModelAsync(data13);

        Assert.Equal(7, viewModel.Rows.Count);
        Assert.Equal("Default Enemy Ammo", viewModel.Rows[0].ChoiceName);
        Assert.Equal("Elite Militia Ammo", viewModel.Rows[6].ChoiceName);
        Assert.All(viewModel.Rows, row => Assert.Equal(50, row.Cells.Count));
        Assert.Equal("Ball", viewModel.Rows[0].Cells[0].Display);
        Assert.Equal("HP", viewModel.Rows[0].Cells[1].Display);
        Assert.DoesNotContain("Glock", viewModel.Rows[0].Cells[0].Display, StringComparison.Ordinal);
        Assert.Empty(viewModel.Warnings);
    }

    /// <summary>
    /// Editing a cell writes the ammunition-type identifier to the <c>uiIndex=19</c> row of its own file,
    /// creates a missing field on demand, rejects invalid text, and leaves the other six files untouched.
    /// </summary>
    [Fact]
    public async Task EditingCell_SavesOnlyTheOwningFileAndCreatesMissingField()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateFixture(workspace);
        var (viewModel, paths) = await CreateViewModelAsync(data13);
        var otherBytes = await File.ReadAllBytesAsync(paths[^1]);

        var cell = viewModel.Rows[0].Cells[0];
        cell.Value = "2";
        Assert.Equal("AP", cell.Display);
        Assert.True(viewModel.IsDirty);

        var missing = viewModel.Rows[0].Cells[2];
        Assert.Equal(string.Empty, missing.Value);
        missing.Value = "1";
        Assert.Equal("HP", missing.Display);

        cell.Value = "abc";
        Assert.Equal("2", cell.Value);
        Assert.StartsWith("Error", viewModel.StatusText, StringComparison.Ordinal);

        Assert.True(await viewModel.SaveAsync());
        Assert.False(viewModel.IsDirty);

        var document = XDocument.Load(paths[0]);
        var record = document.Root!
            .Elements("ENEMYITEMCHOICES")
            .Single(element => element.Element("uiIndex")?.Value == "19");
        Assert.Equal("2", record.Element("bItemNo1")?.Value);
        Assert.Equal("1", record.Element("bItemNo3")?.Value);

        Assert.Equal(otherBytes, await File.ReadAllBytesAsync(paths[^1]));
        Assert.Equal(
            "Default Enemy Ammo",
            viewModel.Rows[0].ChoiceName);
    }

    /// <summary>
    /// A missing item-choice file becomes a warning and does not prevent the other rows from loading.
    /// </summary>
    [Fact]
    public async Task Initialize_ReportsMissingChoiceTable()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateFixture(workspace);
        File.Delete(Path.Combine(data13, "TableData", "Inventory", ChoiceFiles[^1] + ".xml"));
        var (viewModel, _) = await CreateViewModelAsync(data13);

        Assert.Equal(6, viewModel.Rows.Count);
        Assert.True(viewModel.HasWarnings);
        Assert.Contains(viewModel.Warnings, warning => warning.Contains(ChoiceFiles[^1], StringComparison.Ordinal));
    }

    /// <summary>
    /// On the real v4/v5 data every <c>bItemNoN</c> of the ammunition row resolves to an
    /// <c>Items.AmmoTypes</c> name, never to an item name with the same numeric id.
    /// </summary>
    [Fact]
    public async Task Corpus_EnemyAmmoCellsResolveToAmmunitionTypes()
    {
        var corpus = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpus) || !Directory.Exists(corpus))
        {
            return;
        }

        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = (await new DataSetSessionFactory(provider, catalog, repository)
            .OpenAsync(corpus)).Data!;
        var viewModel = new EnemyAmmoViewModel(session);

        await viewModel.InitializeAsync();

        Assert.Equal(7, viewModel.Rows.Count);
        var ammoNames = XDocument
            .Load(Path.Combine(corpus, "Items", "AmmoTypes.xml"))
            .Root!
            .Elements("AMMOTYPE")
            .ToDictionary(
                static element => element.Element("uiIndex")!.Value.Trim(),
                static element => element.Element("name")!.Value.Trim(),
                StringComparer.Ordinal);
        var filled = viewModel.Rows
            .SelectMany(static row => row.Cells)
            .Where(static cell => !string.IsNullOrEmpty(cell.Value))
            .ToArray();
        Assert.NotEmpty(filled);
        Assert.All(filled, cell =>
        {
            Assert.True(ammoNames.TryGetValue(cell.Value, out var name), $"Unknown ammo type {cell.Value}.");
            Assert.Equal(name, cell.Display);
        });
    }

    private static async Task<(EnemyAmmoViewModel ViewModel, string[] Paths)> CreateViewModelAsync(string data13)
    {
        var databasePath = Path.Combine(data13, "metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = (await new DataSetSessionFactory(provider, catalog, repository)
            .OpenAsync(data13)).Data!;
        var viewModel = new EnemyAmmoViewModel(session);
        await viewModel.InitializeAsync();

        var paths = ChoiceFiles
            .Select(name => Path.Combine(data13, "TableData", "Inventory", name + ".xml"))
            .ToArray();
        return (viewModel, paths);
    }

    private static string CreateFixture(TestWorkspace workspace)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        var inventoryDirectory = Path.Combine(tableData, "Inventory");
        var lookupDirectory = Path.Combine(tableData, "Lookup");
        Directory.CreateDirectory(itemsDirectory);
        Directory.CreateDirectory(inventoryDirectory);
        Directory.CreateDirectory(lookupDirectory);

        File.WriteAllText(
            Path.Combine(itemsDirectory, "AmmoTypes.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><AMMOTYPELIST>
            <AMMOTYPE><uiIndex>0</uiIndex><name>Ball</name></AMMOTYPE>
            <AMMOTYPE><uiIndex>1</uiIndex><name>HP</name></AMMOTYPE>
            <AMMOTYPE><uiIndex>2</uiIndex><name>AP</name></AMMOTYPE>
            </AMMOTYPELIST>
            """);

        var choiceNames = new[]
        {
            (19, "Default Enemy Ammo"),
            (29, "Admin Enemy Ammo"),
            (39, "Regular Enemy Ammo"),
            (49, "Elite Enemy Ammo"),
            (59, "Green Militia Ammo"),
            (69, "Regular MilitiaAmmo"),
            (79, "Elite Militia Ammo"),
        };
        File.WriteAllText(
            Path.Combine(lookupDirectory, "AmmoChoices.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + string.Concat(choiceNames.Select(static choice =>
                $"<AmmoChoices><id>{choice.Item1}</id><name>{choice.Item2}</name></AmmoChoices>"))
            + "</JA2Data>");

        foreach (var file in ChoiceFiles)
        {
            File.WriteAllText(
                Path.Combine(inventoryDirectory, file + ".xml"),
                """
                <?xml version="1.0" encoding="utf-8"?><ENEMYITEMCHOICESLIST>
                <ENEMYITEMCHOICES><uiIndex>18</uiIndex><name>Items</name><ubChoices>2</ubChoices><bItemNo1>1</bItemNo1><bItemNo2>2</bItemNo2></ENEMYITEMCHOICES>
                <ENEMYITEMCHOICES><uiIndex>19</uiIndex><name>Ammo</name><ubChoices>2</ubChoices><bItemNo1>0</bItemNo1><bItemNo2>1</bItemNo2></ENEMYITEMCHOICES>
                </ENEMYITEMCHOICESLIST>
                """);
        }

        return data13;
    }
}
