using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the mercenary starting-gear workspace.
/// </summary>
public class MercGearWorkspaceViewModelTests
{
    /// <summary>
    /// The workspace lists every mercenary, exposes each of their kits, resolves item pickers, and saves
    /// base edits for a selected kit without normalizing or dropping unrelated fields.
    /// </summary>
    [Fact]
    public async Task BaseEdits_SaveSelectedKitAndKeepSparseFields()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);
        var itemsBefore = await File.ReadAllBytesAsync(fixture.ItemsPath);
        var overlayBefore = await File.ReadAllBytesAsync(fixture.OverlayPath);

        Assert.True(viewModel.IsEditable);
        Assert.Equal(2, viewModel.Mercs.Count);
        Assert.Equal("Barry", viewModel.Mercs[0].Name);
        Assert.Equal(3, viewModel.Kits.Count);
        Assert.Equal("Standard", viewModel.Kits[0].Name);
        Assert.All(viewModel.Kits.SelectMany(static kit => kit.Slots), static slot => Assert.True(slot.HasRelation));

        var kit = viewModel.Kits[0];
        var propertyChanges = new List<string?>();
        viewModel.PropertyChanged += (_, args) => propertyChanges.Add(args.PropertyName);
        var helmet = Assert.Single(kit.Slots, static slot => slot.ItemField == "mHelmet");
        Assert.Contains(helmet.Relation!.Options, static entry => entry.Value == "3");
        var nothing = Assert.Single(helmet.Relation.Options, static entry => entry.Value == "0");
        Assert.Equal("Nothing", nothing.Display);
        helmet.ItemId = "3";
        helmet.Status = "90";
        kit.PriceMod = "5";
        var smallSlot = Assert.Single(kit.Slots, static slot => slot.ItemField == "mSmall0");
        Assert.Equal(string.Empty, smallSlot.ItemId);
        smallSlot.ItemId = "2";
        kit.Name = "Renamed";

        Assert.True(viewModel.IsDirty);
        Assert.Contains(nameof(MercGearWorkspaceViewModel.DisplayName), propertyChanges);
        Assert.Equal(viewModel.Title + " *", viewModel.DisplayName);
        Assert.True(viewModel.CanUndo);
        await viewModel.SaveAsync();
        Assert.False(viewModel.IsDirty);

        var saved = System.Xml.Linq.XDocument.Load(fixture.BasePath);
        var merc = saved.Root!.Elements("MERCGEAR").First(static element => element.Element("mIndex")?.Value == "0");
        var kits = merc.Elements("GEARKIT").ToArray();
        Assert.Equal("3", kits[0].Element("mHelmet")?.Value);
        Assert.Equal("90", kits[0].Element("mHelmetStatus")?.Value);
        Assert.Equal("5", kits[0].Element("mPriceMod")?.Value);
        Assert.Equal("2", kits[0].Element("mSmall0")?.Value);
        Assert.Equal("Renamed", kits[0].Element("mGearKitName")?.Value);
        Assert.Equal("keep-me", kits[0].Element("CustomKitFlag")?.Value);
        Assert.Equal("Mechanic", kits[1].Element("mGearKitName")?.Value);
        Assert.Equal("Fighter", kits[2].Element("mGearKitName")?.Value);
        Assert.Equal(itemsBefore, await File.ReadAllBytesAsync(fixture.ItemsPath));
        Assert.Equal(overlayBefore, await File.ReadAllBytesAsync(fixture.OverlayPath));
    }

    /// <summary>
    /// The kit tab header shows the stored kit name and falls back to the localized kit number when the
    /// name is cleared, matching the legacy page caption behavior; the name field keeps the legacy
    /// <c>MaxLength = 80</c>.
    /// </summary>
    [Fact]
    public async Task KitHeader_FollowsNameAndFallsBackToLocalizedNumber()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        var kit = viewModel.Kits[0];
        Assert.Equal("Standard", kit.Header);
        Assert.Equal(80, kit.NameMaxLength);

        kit.Name = "Field kit";
        Assert.Equal("Field kit", kit.Header);

        kit.Name = string.Empty;
        Assert.Equal("Kit 1", kit.Header);
        Assert.Equal("Mechanic", viewModel.Kits[1].Header);
        viewModel.Kits[1].Name = string.Empty;
        Assert.Equal("Kit 2", viewModel.Kits[1].Header);
    }

    /// <summary>
    /// Filtered armour and LBE slots offer item 0 ("Nothing") even when no kit in the table uses it,
    /// which reproduces the legacy <c>uiIndex = 0 OR usItemClass = …</c> picker.
    /// </summary>
    [Fact]
    public async Task FilteredSlots_OfferNothingWithoutDocumentValue()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var content = await File.ReadAllTextAsync(fixture.BasePath);
        await File.WriteAllTextAsync(
            fixture.BasePath,
            content.Replace("<mHelmet>0</mHelmet>", "<mHelmet>3</mHelmet>", StringComparison.Ordinal));
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        var helmet = Assert.Single(
            viewModel.Kits[0].Slots,
            static slot => slot.ItemField == "mHelmet");
        var nothing = Assert.Single(helmet.Relation!.Options, static entry => entry.Value == "0");
        Assert.Equal("Nothing", nothing.Display);
        Assert.Contains(helmet.Relation.Options, static entry => entry.Value == "3");
        Assert.DoesNotContain(helmet.Relation.Options, static entry => entry.Value == "2");
    }

    /// <summary>
    /// With a Russian overlay selected, kit names show merged values and an edited kit name is written
    /// only to the overlay, leaving both the base table and other kits untouched.
    /// </summary>
    [Fact]
    public async Task TranslatedKitName_WritesOnlyToOverlay()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture, languageCode: "ru");
        var baseBefore = await File.ReadAllBytesAsync(fixture.BasePath);

        Assert.True(viewModel.IsTranslationMode);
        Assert.Equal(8, viewModel.LanguageOptions.Count);
        Assert.Equal("ru", viewModel.SelectedLanguageOption?.Code);
        Assert.Equal("Стандарт", viewModel.Kits[0].Name);
        Assert.Equal("Стандарт", viewModel.Kits[0].Header);
        Assert.True(viewModel.Kits[0].IsNameTranslated);
        Assert.Equal("Fighter", viewModel.Kits[2].Name);
        Assert.False(viewModel.Kits[2].IsNameTranslated);
        Assert.True(viewModel.Kits[2].ShowNameMarker);

        viewModel.Kits[0].Name = "Стандарт+";
        Assert.Equal("Стандарт+", viewModel.Kits[0].Header);
        Assert.True(viewModel.IsDirty);
        await viewModel.SaveAsync();
        Assert.False(viewModel.IsDirty);

        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(fixture.BasePath));
        var saved = System.Xml.Linq.XDocument.Load(fixture.OverlayPath);
        var merc = saved.Root!.Elements("MERCGEAR").First(static element => element.Element("mIndex")?.Value == "0");
        var kits = merc.Elements("GEARKIT").ToArray();
        Assert.Equal("Стандарт+", kits[0].Element("mGearKitName")?.Value);
        Assert.Equal("Механик", kits[1].Element("mGearKitName")?.Value);
        Assert.Equal(2, kits.Length);
        Assert.Equal("Standard", System.Xml.Linq.XDocument.Load(fixture.BasePath)
            .Root!.Elements("MERCGEAR")
            .First(static element => element.Element("mIndex")?.Value == "0")
            .Elements("GEARKIT")
            .First()
            .Element("mGearKitName")?.Value);
    }

    /// <summary>
    /// A confirmed overwrite saves gear edits over an externally changed file and rebases the baseline.
    /// </summary>
    [Fact]
    public async Task Save_ExternalChangeOverwrite_WritesGearFile()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        viewModel.Kits[0].PriceMod = "5";
        await File.AppendAllTextAsync(fixture.BasePath, " ");
        dialog.ExternalChangeResult = ExternalChangeChoice.Overwrite;

        var saved = await viewModel.SaveWithRecoveryAsync();

        Assert.True(saved);
        Assert.False(viewModel.IsDirty);
        Assert.Contains("changed on disk", dialog.LastExternalChangeMessage, StringComparison.OrdinalIgnoreCase);
        var savedDocument = System.Xml.Linq.XDocument.Load(fixture.BasePath);
        var merc = savedDocument.Root!.Elements("MERCGEAR")
            .First(static element => element.Element("mIndex")?.Value == "0");
        Assert.Equal("5", merc.Elements("GEARKIT").First().Element("mPriceMod")?.Value);
    }

    private static async Task<(MercGearWorkspaceViewModel ViewModel, FakeDialogService Dialog)> CreateWorkspaceAsync(
        Fixture fixture,
        string? languageCode = null)
    {
        var databasePath = Path.Combine(fixture.Workspace.DirectoryPath, "metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        var catalog = new FileSystemTableCatalog(languages.Data!, provider);
        var repository = new TableRepository(provider);
        var session = new DataSetSession(fixture.TableDataDirectory, provider, catalog, repository);
        var descriptors = await catalog.GetTablesAsync(fixture.TableDataDirectory);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate =>
            candidate.TableName == TableName.Parse("Inventory.MercStartingGear"));
        var dialog = new FakeDialogService();
        var viewModel = new MercGearWorkspaceViewModel(session, descriptor, languageCode, dialog);
        await viewModel.InitializeAsync();
        return (viewModel, dialog);
    }

    private static Fixture CreateFixture(TestWorkspace workspace)
    {
        var tableData = workspace.PathOf(Path.Combine("Data-1.13", "TableData"));
        var inventoryDirectory = Path.Combine(tableData, "Inventory");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(inventoryDirectory);
        Directory.CreateDirectory(itemsDirectory);

        var basePath = Path.Combine(inventoryDirectory, "MercStartingGear.xml");
        var overlayPath = Path.Combine(inventoryDirectory, "Russian.MercStartingGear.xml");
        var itemsPath = Path.Combine(itemsDirectory, "Items.xml");

        File.WriteAllText(
            basePath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><MERCGEARLIST>\r\n"
            + "\t<MERCGEAR>\r\n"
            + "\t\t<mIndex>0</mIndex>\r\n"
            + "\t\t<mName>Barry</mName>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mPriceMod>0</mPriceMod>\r\n"
            + "\t\t\t<mGearKitName>Standard</mGearKitName>\r\n"
            + "\t\t\t<mAbsolutePrice>-1</mAbsolutePrice>\r\n"
            + "\t\t\t<mHelmet>0</mHelmet>\r\n"
            + "\t\t\t<mHelmetDrop>0</mHelmetDrop>\r\n"
            + "\t\t\t<mHelmetStatus>0</mHelmetStatus>\r\n"
            + "\t\t\t<mWeapon>1</mWeapon>\r\n"
            + "\t\t\t<mWeaponDrop>0</mWeaponDrop>\r\n"
            + "\t\t\t<mWeaponStatus>91</mWeaponStatus>\r\n"
            + "\t\t\t<mBig0>2</mBig0>\r\n"
            + "\t\t\t<mBig0Status>100</mBig0Status>\r\n"
            + "\t\t\t<mBig0Quantity>3</mBig0Quantity>\r\n"
            + "\t\t\t<mBig0Drop>0</mBig0Drop>\r\n"
            + "\t\t\t<CustomKitFlag>keep-me</CustomKitFlag>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mGearKitName>Mechanic</mGearKitName>\r\n"
            + "\t\t\t<mWeapon>1</mWeapon>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mGearKitName>Fighter</mGearKitName>\r\n"
            + "\t\t\t<mWeapon>1</mWeapon>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t</MERCGEAR>\r\n"
            + "\t<MERCGEAR>\r\n"
            + "\t\t<mIndex>1</mIndex>\r\n"
            + "\t\t<mName>Blood</mName>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mGearKitName>Standard</mGearKitName>\r\n"
            + "\t\t\t<mWeapon>1</mWeapon>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t</MERCGEAR>\r\n"
            + "</MERCGEARLIST>\r\n");

        File.WriteAllText(
            overlayPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><MERCGEARLIST>\r\n"
            + "\t<MERCGEAR>\r\n"
            + "\t\t<mIndex>0</mIndex>\r\n"
            + "\t\t<mName>Барри</mName>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mGearKitName>Стандарт</mGearKitName>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t\t<GEARKIT>\r\n"
            + "\t\t\t<mGearKitName>Механик</mGearKitName>\r\n"
            + "\t\t</GEARKIT>\r\n"
            + "\t</MERCGEAR>\r\n"
            + "</MERCGEARLIST>\r\n");

        File.WriteAllText(
            itemsPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>\r\n"
            + "\t<ITEM><uiIndex>0</uiIndex><szLongItemName>Nothing</szLongItemName><usItemClass>1</usItemClass></ITEM>\r\n"
            + "\t<ITEM><uiIndex>1</uiIndex><szLongItemName>Pistol</szLongItemName><usItemClass>2</usItemClass></ITEM>\r\n"
            + "\t<ITEM><uiIndex>2</uiIndex><szLongItemName>Ammo</szLongItemName><usItemClass>1024</usItemClass></ITEM>\r\n"
            + "\t<ITEM><uiIndex>3</uiIndex><szLongItemName>Helmet</szLongItemName><usItemClass>2048</usItemClass></ITEM>\r\n"
            + "\t<ITEM><uiIndex>4</uiIndex><szLongItemName>Vest</szLongItemName><usItemClass>2048</usItemClass></ITEM>\r\n"
            + "</ITEMLIST>\r\n");

        return new Fixture(workspace, tableData, basePath, overlayPath, itemsPath);
    }

    private sealed record Fixture(
        TestWorkspace Workspace,
        string TableDataDirectory,
        string BasePath,
        string OverlayPath,
        string ItemsPath);

    private sealed class FakeDialogService : IDialogService
    {
        public UnsavedChangesChoice UnsavedChangesResult { get; set; } = UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ExternalChangeResult { get; set; } = ExternalChangeChoice.Cancel;

        public string? LastExternalChangeMessage { get; private set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => true;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesResult;

        public ExternalChangeChoice ResolveExternalChange(string message, string title)
        {
            LastExternalChangeMessage = message;
            return ExternalChangeResult;
        }

        public ulong? EditBits(
            string title,
            ulong value,
            IReadOnlyDictionary<int, string>? labels = null,
            string? valueName = null) => null;

        public void ShowTranslationReport(TranslationReport report, string title)
        {
        }

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => null;
    }
}
