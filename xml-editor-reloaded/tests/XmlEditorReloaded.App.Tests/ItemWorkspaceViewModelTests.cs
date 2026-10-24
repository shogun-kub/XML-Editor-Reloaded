using System.Globalization;
using System.IO;
using System.Xml.Linq;
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
/// Tests for the item master-detail workspace: the item selector, the item form, 1:1 and 1:N sections,
/// nested and repeated fields, the item workflow, saving, and composite undo.
/// </summary>
public class ItemWorkspaceViewModelTests
{
    private static readonly TableName Items = TableName.Parse("Items.Items");
    private static readonly TableName Weapons = TableName.Parse("Items.Weapons");
    private static readonly TableName Attachments = TableName.Parse("Items.Attachments");

    /// <summary>
    /// Loading builds the item list, the item field groups, the repeated-field section, the nested
    /// modifier group, and only the sub-table sections that belong to the item class.
    /// </summary>
    [Fact]
    public async Task Initialize_BuildsFormSectionsForItemClass()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        Assert.Single(viewModel.Items);
        Assert.NotNull(viewModel.SelectedItem);
        Assert.Equal("1", viewModel.SelectedItem!.Key);
        Assert.Equal("Glock 17", viewModel.SelectedItem.Name);
        Assert.Equal("Gun", viewModel.SelectedItem.ClassText);
        Assert.Equal("1: Glock 17", viewModel.DetailHeader);
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "identity");
        Assert.Contains(
            viewModel.ItemGroups.SelectMany(group => group.Fields),
            field => field.Name == "szItemName" && field.Value == "Glock 17");
        Assert.Contains(viewModel.NestedGroups, group => group.Key == "nested:STAND_MODIFIERS");
        Assert.Contains(
            viewModel.ListSections,
            section => section.FieldName == "DefaultAttachment" && section.Values.Count == 2);

        var weapon = Assert.Single(
            viewModel.SubTableSections,
            section => section.TableName == Weapons);
        Assert.True(weapon.HasRow);
        Assert.Equal("100", weapon.Fields.Single(field => field.Name == "usRange").Value);
        Assert.DoesNotContain(viewModel.SubTableSections, section => section.TableName == TableName.Parse("Items.Armours"));
        Assert.Contains(viewModel.ChildSections, section => section.TableName == Attachments);
    }

    /// <summary>
    /// Editing the item and its weapon from one workspace marks the set dirty and one save writes both
    /// files.
    /// </summary>
    [Fact]
    public async Task EditItemAndWeapon_SavesBothFiles()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        var nameField = viewModel.ItemGroups
            .SelectMany(group => group.Fields)
            .Single(field => field.Name == "szItemName");
        nameField.Value = "Glock 17A";
        Assert.True(viewModel.IsDirty);

        var range = viewModel.SubTableSections
            .Single(section => section.TableName == Weapons)
            .Fields
            .Single(field => field.Name == "usRange");
        range.Value = "150";
        Assert.True(viewModel.IsDirty);

        await viewModel.SaveAsync();

        Assert.False(viewModel.IsDirty);
        var items = XDocument.Load(fixture.ItemsPath);
        Assert.Equal(
            "Glock 17A",
            items.Root!.Elements("ITEM").First(item => item.Element("uiIndex")?.Value == "1").Element("szItemName")!.Value);
        var weapons = XDocument.Load(fixture.WeaponPath);
        Assert.Equal(
            "150",
            weapons.Root!.Elements("WEAPON").First(weapon => weapon.Element("uiIndex")?.Value == "1").Element("usRange")!.Value);
        var attachments = XDocument.Load(fixture.AttachmentsPath);
        Assert.Equal(
            "keep",
            attachments.Root!.Elements("ATTACHMENT").First().Element("APCost")!.Value);
    }

    /// <summary>
    /// Duplicating an item copies the weapon row and the child attachment rows, and one undo restores
    /// every file.
    /// </summary>
    [Fact]
    public async Task DuplicateItem_CopiesDependentsAndUndoesAsOneStep()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        viewModel.DuplicateItemCommand.Execute(null);

        Assert.Equal(2, viewModel.Items.Count);
        var duplicate = Assert.Single(viewModel.Items, entry => entry.Key == "2");
        Assert.Equal("Glock 17", duplicate.Name);
        viewModel.SelectRecord("2");
        var weapon = Assert.Single(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.True(weapon.HasRow);
        Assert.Equal("100", weapon.Fields.Single(field => field.Name == "usRange").Value);
        var attachments = viewModel.ChildSections
            .Single(section => section.TableName == Attachments)
            .Editor;
        Assert.Equal(2, attachments.Rows.Count);

        viewModel.UndoCommand.Execute(null);

        Assert.Single(viewModel.Items);
        Assert.Equal("1", viewModel.SelectedItem?.Key);
    }

    /// <summary>
    /// Deleting an item removes its dependent rows after confirmation, and undo restores the item and
    /// its sections.
    /// </summary>
    [Fact]
    public async Task DeleteItem_RemovesDependentsAndUndoesAsOneStep()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        dialog.ConfirmResult = true;

        viewModel.DeleteItemCommand.Execute(null);

        Assert.Empty(viewModel.Items);
        Assert.Null(viewModel.SelectedItem);
        Assert.True(viewModel.IsDirty);

        viewModel.UndoCommand.Execute(null);

        Assert.Single(viewModel.Items);
        Assert.Equal("1", viewModel.SelectedItem?.Key);
        var weapon = Assert.Single(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.True(weapon.HasRow);
    }

    /// <summary>
    /// Renaming an item rewrites the weapon and attachment references, and a collision with another
    /// item swaps both identifiers.
    /// </summary>
    [Fact]
    public async Task ChangeItemId_RenamesAndSwapsDependents()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        dialog.ConfirmResult = true;

        viewModel.ItemIdText = "5";
        viewModel.ChangeItemIdCommand.Execute(null);

        Assert.Contains(viewModel.Items, entry => entry.Key == "5");
        var weapon = Assert.Single(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.Equal("5", weapon.Row?.Key);
        var attachmentEditor = viewModel.ChildSections
            .Single(section => section.TableName == Attachments)
            .Editor;
        Assert.All(
            attachmentEditor.Rows,
            row => Assert.Equal("5", row["itemIndex"]));

        viewModel.DuplicateItemCommand.Execute(null);
        var duplicateKey = viewModel.Items
            .Select(entry => entry.Key)
            .First(key => !string.Equals(key, "5", StringComparison.Ordinal));
        viewModel.SelectRecord("5");
        viewModel.ItemIdText = duplicateKey;
        viewModel.ChangeItemIdCommand.Execute(null);

        Assert.Equal("Glock 17", viewModel.Items.Single(entry => entry.Key == duplicateKey).Name);
    }

    /// <summary>
    /// Nested modifiers and repeated fields are written without disturbing the sparse record shape.
    /// </summary>
    [Fact]
    public async Task NestedAndListFields_RoundTrip()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        Assert.True(viewModel.SetItemValue("STAND_MODIFIERS/FlatBase", "-4").IsSuccess);
        var list = viewModel.ListSections.Single(section => section.FieldName == "DefaultAttachment");
        list.Values[0].Value = "8";

        await viewModel.SaveAsync();

        var items = XDocument.Load(fixture.ItemsPath);
        var item = items.Root!.Elements("ITEM").First(element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal("-4", item.Element("STAND_MODIFIERS")!.Element("FlatBase")!.Value);
        Assert.Equal(new[] { "8", "6" }, item.Elements("DefaultAttachment").Select(static element => element.Value));
        Assert.Equal("keep me", item.Element("CustomUnknown")!.Value);
        Assert.Equal("1.0", item.Element("ProjectionFactor")!.Value);
    }

    /// <summary>
    /// Creating an item uses the values entered in the dialog and selects the new record.
    /// </summary>
    [Fact]
    public async Task NewItem_CreatesRecordFromDialogRequest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        dialog.CreateItemResult = new ItemCreationRequest("7", "New gun", "2");

        viewModel.NewItemCommand.Execute(null);

        Assert.Equal(2, viewModel.Items.Count);
        var created = Assert.Single(viewModel.Items, entry => entry.Key == "7");
        Assert.Equal("New gun", created.Name);
        Assert.Equal("7", viewModel.SelectedItem?.Key);
        Assert.True(viewModel.IsDirty);
        var weapon = Assert.Single(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.False(weapon.HasRow);
        Assert.True(weapon.CanCreate);
    }

    /// <summary>
    /// With a language overlay selected, item name edits go to the overlay and the base file stays
    /// untouched until saving, when only the overlay changes.
    /// </summary>
    [Fact]
    public async Task TranslatableEdit_WritesOverlayOnly()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace, includeOverlay: true);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture, "ru");
        Assert.True(viewModel.IsTranslationMode);

        var nameField = viewModel.ItemGroups
            .SelectMany(group => group.Fields)
            .Single(field => field.Name == "szItemName");
        Assert.Equal("Глок 17", nameField.Value);
        var baseBytes = await File.ReadAllBytesAsync(fixture.ItemsPath);

        nameField.Value = "Глок 17А";

        Assert.Equal(baseBytes, await File.ReadAllBytesAsync(fixture.ItemsPath));
        await viewModel.SaveAsync();
        var baseDocument = XDocument.Load(fixture.ItemsPath);
        Assert.Equal(
            "Glock 17",
            baseDocument.Root!.Elements("ITEM").First().Element("szItemName")!.Value);
        var overlayPath = Path.Combine(Path.GetDirectoryName(fixture.ItemsPath)!, "Russian.Items.xml");
        var overlay = XDocument.Load(overlayPath);
        Assert.Equal("Глок 17А", overlay.Root!.Elements("ITEM").First().Element("szItemName")!.Value);
    }

    /// <summary>
    /// Changing the item class rebuilds the visible sub-table sections without rewriting
    /// <c>ubClassIndex</c>.
    /// </summary>
    [Fact]
    public async Task ChangeClass_RebuildsSectionsAndKeepsClassIndex()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Weapons);

        var classResult = viewModel.SetItemValue("usItemClass", "1024");

        Assert.True(classResult.IsSuccess, classResult.ErrorText);
        Assert.Equal("Ammo", viewModel.ItemClassText);
        Assert.DoesNotContain(viewModel.SubTableSections, section => section.TableName == Weapons);
        var magazines = Assert.Single(
            viewModel.SubTableSections,
            section => section.TableName == TableName.Parse("Items.Magazines"));
        Assert.True(magazines.HasRow);
        Assert.Equal("15", magazines.Fields.Single(field => field.Name == "ubMagSize").Value);
        Assert.Equal("1", viewModel.SelectedItem!.Row["ubClassIndex"]);
    }

    /// <summary>
    /// The go-to-id command selects an item even when the search filter would hide it.
    /// </summary>
    [Fact]
    public async Task GoToItemId_ClearsSearchAndSelectsItem()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace, includeSecondItem: true);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        viewModel.SearchText = "Glock 17";
        Assert.Single(viewModel.Items);
        Assert.Equal("Glock 17", viewModel.SelectedItem?.Name);

        viewModel.GoToIdText = "2";
        viewModel.GoToItemIdCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.SearchText);
        Assert.Equal("2", viewModel.SelectedItem?.Key);
        Assert.Equal("Glock 18", viewModel.SelectedItem?.Name);
    }

    /// <summary>
    /// Item previews are requested with the graphic type and index of the selected item.
    /// </summary>
    [Fact]
    public async Task SelectItem_LoadsPreviewImages()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        for (var attempt = 0; attempt < 100 && viewModel.DetailImages.All(panel => panel.Image is null); attempt++)
        {
            await Task.Delay(25);
        }

        Assert.All(viewModel.DetailImages, panel => Assert.NotNull(panel.Image));
        Assert.All(viewModel.DetailImages, panel => Assert.Equal(2, panel.Image!.PixelWidth));
    }

    private static async Task<(ItemWorkspaceViewModel ViewModel, FakeDialogService Dialog)> CreateWorkspaceAsync(
        Fixture fixture,
        string? languageCode = null)
    {
        var databasePath = Path.Combine(fixture.Workspace.DirectoryPath, "metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = new DataSetSession(fixture.TableDataDirectory, provider, catalog, repository);
        var descriptors = await catalog.GetTablesAsync(fixture.TableDataDirectory);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);
        var dialog = new FakeDialogService();
        var images = new TestItemImageService();
        var viewModel = new ItemWorkspaceViewModel(session, descriptor, languageCode, dialog, images);
        await viewModel.InitializeAsync();
        return (viewModel, dialog);
    }

    private static Fixture CreateFixture(
        TestWorkspace workspace,
        bool includeSecondItem = false,
        bool includeOverlay = false)
    {
        var tableData = workspace.PathOf(Path.Combine("Data-1.13", "TableData"));
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var lookupDirectory = Path.Combine(tableData, "Lookup");
        Directory.CreateDirectory(lookupDirectory);
        File.WriteAllText(
            Path.Combine(lookupDirectory, "ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>1</id><name>Nothing</name></ItemClass>"
            + "<ItemClass><id>2</id><name>Gun</name></ItemClass>"
            + "<ItemClass><id>1024</id><name>Ammo</name></ItemClass>"
            + "<ItemClass><id>2048</id><name>Armour</name></ItemClass>"
            + "</JA2Data>");

        var itemsPath = Path.Combine(itemsDirectory, "Items.xml");
        var secondItem = includeSecondItem
            ? "<ITEM><uiIndex>2</uiIndex><szItemName>Glock 18</szItemName>"
              + "<usItemClass>2</usItemClass><ubClassIndex>2</ubClassIndex>"
              + "<STAND_MODIFIERS /><CustomUnknown>keep me</CustomUnknown>"
              + "<ProjectionFactor>1.0</ProjectionFactor></ITEM>"
            : string.Empty;
        File.WriteAllText(
            itemsPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>\r\n"
            + "\t<ITEM>\r\n"
            + "\t\t<uiIndex>1</uiIndex>\r\n"
            + "\t\t<szItemName>Glock 17</szItemName>\r\n"
            + "\t\t<usItemClass>2</usItemClass>\r\n"
            + "\t\t<ubClassIndex>1</ubClassIndex>\r\n"
            + "\t\t<ubGraphicType>0</ubGraphicType>\r\n"
            + "\t\t<ubGraphicNum>1</ubGraphicNum>\r\n"
            + "\t\t<ProjectionFactor>1.0</ProjectionFactor>\r\n"
            + "\t\t<DefaultAttachment>7</DefaultAttachment>\r\n"
            + "\t\t<DefaultAttachment>6</DefaultAttachment>\r\n"
            + "\t\t<STAND_MODIFIERS>\r\n"
            + "\t\t\t<FlatBase>0</FlatBase>\r\n"
            + "\t\t\t<PercentBase>0</PercentBase>\r\n"
            + "\t\t</STAND_MODIFIERS>\r\n"
            + "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n"
            + "\t</ITEM>\r\n"
            + secondItem
            + "</ITEMLIST>\r\n");

        File.WriteAllText(
            Path.Combine(itemsDirectory, "Magazines.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><MAGAZINELIST>\r\n"
            + "\t<MAGAZINE>\r\n"
            + "\t\t<uiIndex>1</uiIndex>\r\n"
            + "\t\t<ubMagSize>15</ubMagSize>\r\n"
            + "\t</MAGAZINE>\r\n"
            + "</MAGAZINELIST>\r\n");
        if (includeOverlay)
        {
            File.WriteAllText(
                Path.Combine(itemsDirectory, "Russian.Items.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>\r\n"
                + "\t<ITEM>\r\n"
                + "\t\t<uiIndex>1</uiIndex>\r\n"
                + "\t\t<szItemName>Глок 17</szItemName>\r\n"
                + "\t</ITEM>\r\n"
                + "</ITEMLIST>\r\n");
        }

        var weaponPath = Path.Combine(itemsDirectory, "Weapons.xml");
        File.WriteAllText(
            weaponPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><WEAPONLIST>\r\n"
            + "\t<WEAPON>\r\n"
            + "\t\t<uiIndex>1</uiIndex>\r\n"
            + "\t\t<usRange>100</usRange>\r\n"
            + "\t\t<ubCalibre>0</ubCalibre>\r\n"
            + "\t</WEAPON>\r\n"
            + "\t<WEAPON>\r\n"
            + "\t\t<uiIndex>2</uiIndex>\r\n"
            + "\t\t<usRange>110</usRange>\r\n"
            + "\t\t<ubCalibre>0</ubCalibre>\r\n"
            + "\t</WEAPON>\r\n"
            + "</WEAPONLIST>\r\n");

        var attachmentsPath = Path.Combine(itemsDirectory, "Attachments.xml");
        File.WriteAllText(
            attachmentsPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>\r\n"
            + "\t<ATTACHMENT>\r\n"
            + "\t\t<attachmentIndex>10</attachmentIndex>\r\n"
            + "\t\t<itemIndex>1</itemIndex>\r\n"
            + "\t\t<APCost>keep</APCost>\r\n"
            + "\t</ATTACHMENT>\r\n"
            + "\t<ATTACHMENT>\r\n"
            + "\t\t<attachmentIndex>11</attachmentIndex>\r\n"
            + "\t\t<itemIndex>1</itemIndex>\r\n"
            + "\t\t<APCost>keep</APCost>\r\n"
            + "\t</ATTACHMENT>\r\n"
            + "</ATTACHMENTLIST>\r\n");

        return new Fixture(workspace, tableData, itemsPath, weaponPath, attachmentsPath);
    }

    private sealed record Fixture(
        TestWorkspace Workspace,
        string TableDataDirectory,
        string ItemsPath,
        string WeaponPath,
        string AttachmentsPath);

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; }

        public ItemCreationRequest? CreateItemResult { get; set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

        public uint? EditBits(
            string title,
            uint value,
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
            IReadOnlyList<LookupOption> classes) => CreateItemResult;
    }
}
