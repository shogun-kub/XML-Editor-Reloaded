using System.Globalization;
using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Filtering;
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
    private static readonly TableName Magazines = TableName.Parse("Items.Magazines");
    private static readonly TableName Attachments = TableName.Parse("Items.Attachments");
    private static readonly TableName CompatibleFaceItems = TableName.Parse("Items.CompatibleFaceItems");
    private static readonly TableName Launchables = TableName.Parse("Items.Launchables");

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
    /// The item workspace shows both legacy directions of the relation tables: the reverse attachment
    /// view ("Attachable items") and the forward launchables view ("Ammunition"), while the original
    /// launcher-child view stays hidden for a launcher class.
    /// </summary>
    [Fact]
    public async Task Initialize_ShowsReverseAttachmentsAndForwardLaunchables()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateRelationFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        viewModel.SelectRecord("10");

        var attachTo = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.Reverse);
        Assert.Equal("Attachable items", attachTo.Label);
        Assert.True(attachTo.IsVisible);
        Assert.Single(attachTo.Editor.Rows);
        Assert.Equal("1", attachTo.Editor.Rows[0]["itemIndex"]);

        var ammunition = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == Launchables
                && section.Member.Relation.Kind == TableRelationKind.Reverse);
        Assert.Equal("Ammunition", ammunition.Label);
        Assert.True(ammunition.IsVisible);
        Assert.Single(ammunition.Editor.Rows);
        Assert.Equal("13", ammunition.Editor.Rows[0]["launchableIndex"]);

        var launcherChild = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == Launchables
                && section.Member.Relation.Kind == TableRelationKind.ChildRows);
        Assert.False(launcherChild.IsVisible);
    }

    /// <summary>
    /// Compatible face items are filtered by the selected item's <c>itemIndex</c>, matching the legacy
    /// CompatibleFaceItemGrid, and the row exposes <c>compatiblefaceitemIndex</c> as the related item.
    /// </summary>
    [Fact]
    public async Task CompatibleFaceItems_FiltersByItemIndex()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateRelationFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        viewModel.SelectRecord("17");

        var faces = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == CompatibleFaceItems);
        Assert.Equal(TableRelationKind.ChildRows, faces.Member.Relation.Kind);
        Assert.True(faces.IsVisible);
        var row = Assert.Single(faces.Editor.Rows);
        Assert.Equal("18", row["compatiblefaceitemIndex"]);
    }

    /// <summary>
    /// Activating a row of a child or reverse section navigates to the related item in the same item
    /// workspace, including when the target row is not the current selection.
    /// </summary>
    [Fact]
    public async Task ChildSectionNavigation_SelectsRelatedItem()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateRelationFixture(workspace);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);

        viewModel.SelectRecord("10");
        var ammunition = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == Launchables
                && section.Member.Relation.Kind == TableRelationKind.Reverse);
        Assert.True(ammunition.Editor.NavigateToRelationTarget(ammunition.Editor.Rows[0]));
        Assert.Equal("13", viewModel.SelectedItem?.Key);

        viewModel.SelectRecord("10");
        var attachTo = Assert.Single(
            viewModel.ChildSections,
            section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.Reverse);
        Assert.True(attachTo.Editor.NavigateToRelationTarget(attachTo.Editor.Rows[0]));
        Assert.Equal("1", viewModel.SelectedItem?.Key);
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
            .Single(section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.ChildRows)
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
            .Single(section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.ChildRows)
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
    /// Creation and identifier changes clear the item filters and the search term so the affected
    /// record is visible instead of falling back to an unrelated row.
    /// </summary>
    [Fact]
    public async Task NewItemAndRename_ClearFiltersThatWouldHideTheRecord()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, dialog) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.keys");
        dialog.CreateItemResult = new ItemCreationRequest("40", "New key", "65536");
        viewModel.NewItemCommand.Execute(null);

        Assert.Equal("40", viewModel.SelectedItem?.Key);
        Assert.Equal("all", viewModel.SelectedClassFilter?.Key);

        viewModel.SelectRecord("19");
        viewModel.SearchText = "19";
        Assert.Single(viewModel.Items);
        viewModel.ItemIdText = "41";
        viewModel.ChangeItemIdCommand.Execute(null);

        Assert.Equal("41", viewModel.SelectedItem?.Key);
        Assert.Equal(string.Empty, viewModel.SearchText);
    }

    /// <summary>
    /// Reloading from disk rebuilds the filter options and the custom filter columns instead of keeping
    /// criteria that refer to a schema which may have changed.
    /// </summary>
    [Fact]
    public async Task Reload_ResetsItemFiltersAndRebuildsOptions()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.guns");
        viewModel.CustomItemFilter!.AddCriterion();
        viewModel.CustomItemFilter.Criteria[0].Operand = "2";

        await viewModel.ReloadFromDiskAsync();

        Assert.Equal(16, viewModel.ClassFilterOptions.Count);
        Assert.Equal(7, viewModel.FlagFilterOptions.Count);
        Assert.Equal("all", viewModel.SelectedClassFilter?.Key);
        Assert.Equal("all", viewModel.SelectedFlagFilter?.Key);
        Assert.NotNull(viewModel.CustomItemFilter);
        Assert.Empty(viewModel.CustomItemFilter!.Criteria);
        Assert.Equal(25, viewModel.Items.Count);
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
    /// Every class preset reproduces the legacy <c>Items &gt; Show &gt; By Class</c> selection.
    /// </summary>
    [Fact]
    public async Task ClassFilters_MatchLegacyPresets()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        Assert.Equal(16, viewModel.ClassFilterOptions.Count);
        var expected = new Dictionary<string, string[]>
        {
            ["class.guns"] = new[] { "1", "30", "31" },
            ["class.ammo"] = new[] { "11" },
            ["class.launchers"] = new[] { "10", "33", "34" },
            ["class.grenades"] = new[] { "13" },
            ["class.explosives"] = new[] { "14" },
            ["class.knives"] = new[] { "15", "28" },
            ["class.other-weapons"] = new[] { "16", "26", "27" },
            ["class.armour"] = new[] { "12" },
            ["class.face"] = new[] { "17" },
            ["class.kits"] = new[] { "18", "25" },
            ["class.keys"] = new[] { "19" },
            ["class.lbe"] = new[] { "20" },
            ["class.misc"] = new[] { "21", "22", "32" },
            ["class.none"] = new[] { "23" },
            ["class.random"] = new[] { "24" },
        };

        foreach (var pair in expected)
        {
            viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == pair.Key);
            Assert.Equal(pair.Value, VisibleKeys(viewModel));
        }

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "all");
        Assert.Equal(25, viewModel.Items.Count);
    }

    /// <summary>
    /// Every item-flag preset reproduces the legacy <c>Items &gt; Show &gt; By Other Filters</c> selection.
    /// </summary>
    [Fact]
    public async Task FlagFilters_MatchLegacyPresets()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        Assert.Equal(7, viewModel.FlagFilterOptions.Count);
        var allKeys = VisibleKeys(viewModel);
        Assert.Equal(25, allKeys.Length);
        var expected = new Dictionary<string, string[]>
        {
            ["flag.attachments"] = new[] { "19", "24", "30" },
            ["flag.drugs"] = new[] { "32" },
            ["flag.big-gun-list"] = new[] { "30" },
            ["flag.normal-guns"] = allKeys.Where(key => key != "30").ToArray(),
            ["flag.scifi"] = new[] { "30" },
            ["flag.realistic"] = allKeys.Where(key => key != "30").ToArray(),
        };

        foreach (var pair in expected)
        {
            viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == pair.Key);
            Assert.Equal(pair.Value, VisibleKeys(viewModel));
        }

        viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == "all");
        Assert.Equal(25, viewModel.Items.Count);
    }

    /// <summary>
    /// The class preset, the flag preset, the search term, and the custom structured filter are
    /// combined with AND; clearing the filters restores the full list.
    /// </summary>
    [Fact]
    public async Task ItemFilters_CombineWithSearchAndClear()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.guns");
        viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == "flag.scifi");
        viewModel.SearchText = "Sci-Fi";

        Assert.Equal(new[] { "30" }, VisibleKeys(viewModel));

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.ammo");
        Assert.Empty(viewModel.Items);

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "all");
        viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == "all");
        viewModel.SearchText = string.Empty;
        Assert.Equal(25, viewModel.Items.Count);

        Assert.NotNull(viewModel.CustomItemFilter);
        viewModel.CustomItemFilter!.AddCriterion();
        var criterion = Assert.Single(viewModel.CustomItemFilter.Criteria);
        criterion.SelectedColumn = viewModel.CustomItemFilter.Columns
            .Single(column => column.Name == "usItemClass");
        criterion.SelectedOperator = criterion.Operators
            .Single(option => option.Operator == RowFilterOperator.Equals);
        criterion.Operand = "2048";

        Assert.Equal(new[] { "12" }, VisibleKeys(viewModel));
        Assert.True(viewModel.HasActiveItemFilters);

        viewModel.ClearItemFiltersCommand.Execute(null);

        Assert.Equal(25, viewModel.Items.Count);
        Assert.Empty(viewModel.CustomItemFilter.Criteria);
        Assert.Equal("all", viewModel.SelectedClassFilter?.Key);
        Assert.Equal("all", viewModel.SelectedFlagFilter?.Key);
        Assert.False(viewModel.HasActiveItemFilters);
    }

    /// <summary>
    /// The legacy per-class page map hides irrelevant field groups, nested structures, list fields,
    /// sub-tables, and child sections for the selected item class.
    /// </summary>
    [Fact]
    public async Task ClassVisibility_HidesIrrelevantSections()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectRecord("1");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "combat");
        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.Contains(viewModel.NestedGroups, group => group.Key == "nested:STAND_MODIFIERS");
        Assert.Contains(viewModel.ListSections, section => section.FieldName == "DefaultAttachment");
        Assert.Contains(viewModel.ChildSections, section => section.TableName == Attachments && section.IsVisible);
        Assert.Contains(
            viewModel.ChildSections,
            section => section.TableName == CompatibleFaceItems && !section.IsVisible);
        Assert.Contains(
            viewModel.ChildSections,
            section => section.TableName == Launchables && !section.IsVisible);
        Assert.Contains(viewModel.VisibleChildSections, section => section.TableName == Attachments);
        Assert.DoesNotContain(viewModel.VisibleChildSections, section => section.TableName == CompatibleFaceItems);

        viewModel.SelectRecord("13");
        Assert.Contains(
            viewModel.ChildSections,
            section => section.TableName == Launchables && section.IsVisible);
        Assert.Contains(viewModel.VisibleChildSections, section => section.TableName == Launchables);

        viewModel.SelectRecord("19");
        Assert.DoesNotContain(
            viewModel.ItemGroups,
            group => group.Key is "combat" or "vision" or "food" or "stance" or "attachments" or "modifiers");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "identity");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "flags");
        Assert.Empty(viewModel.NestedGroups);
        Assert.Empty(viewModel.SubTableSections);
        Assert.DoesNotContain(viewModel.ListSections, section => section.FieldName == "DefaultAttachment");
        Assert.DoesNotContain(viewModel.VisibleChildSections, section => section.TableName == Attachments);
        Assert.All(viewModel.VisibleChildSections, section => Assert.True(section.IsVisible));

        viewModel.SelectRecord("11");
        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Magazines);
        Assert.DoesNotContain(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "combat");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "vision");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "food");
        Assert.Contains(viewModel.ChildSections, section => section.TableName == Attachments && !section.IsVisible);

        viewModel.SelectRecord("24");
        Assert.DoesNotContain(viewModel.ItemGroups, group => group.Key is "attachments" or "combat" or "stance" or "modifiers");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "vision");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "food");
    }

    /// <summary>
    /// Changing <c>usItemClass</c> immediately rebuilds the class-aware groups and sub-tables.
    /// </summary>
    [Fact]
    public async Task ChangeClass_UpdatesClassVisibility()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectRecord("1");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "combat");

        var result = viewModel.SetItemValue("usItemClass", "65536");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.DoesNotContain(viewModel.ItemGroups, group => group.Key == "combat");
        Assert.DoesNotContain(viewModel.SubTableSections, section => section.TableName == Weapons);
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "identity");

        result = viewModel.SetItemValue("usItemClass", "2");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "combat");
        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Weapons);
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
    /// Navigational selection by key clears the class, flag, and custom filters that would otherwise
    /// hide the requested item.
    /// </summary>
    [Fact]
    public async Task GoToItemIdAndSelectRecord_ClearItemFilters()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (viewModel, _) = await CreateWorkspaceAsync(CreateFilterFixture(workspace));

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.guns");
        viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == "flag.scifi");
        Assert.DoesNotContain(viewModel.Items, entry => entry.Key == "19");

        viewModel.GoToIdText = "19";
        viewModel.GoToItemIdCommand.Execute(null);

        Assert.Equal("19", viewModel.SelectedItem?.Key);
        Assert.Equal("all", viewModel.SelectedClassFilter?.Key);
        Assert.Equal("all", viewModel.SelectedFlagFilter?.Key);

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == "class.ammo");
        viewModel.SelectRecord("12");

        Assert.Equal("12", viewModel.SelectedItem?.Key);
        Assert.Equal("all", viewModel.SelectedClassFilter?.Key);
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

    /// <summary>
    /// The shopkeeper tab lists dealer stock rows for the selected item and saves a quantity edit to the
    /// dealer file without rewriting the item file.
    /// </summary>
    [Fact]
    public async Task ShopkeeperTab_ListsDealersAndSavesQuantity()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace, includeDealer: true);
        var (viewModel, _) = await CreateWorkspaceAsync(fixture);
        var itemsBefore = await File.ReadAllBytesAsync(fixture.ItemsPath);

        Assert.True(viewModel.HasShopkeeperEntries);
        var entry = Assert.Single(viewModel.ShopkeeperEntries);
        Assert.Equal("Alberto", entry.DealerName);
        Assert.Equal("9", entry.Quantity);

        entry.Quantity = "12";
        Assert.True(viewModel.IsDirty);
        Assert.True(viewModel.CanUndo);
        await viewModel.SaveAsync();

        Assert.False(viewModel.IsDirty);
        Assert.Equal(itemsBefore, await File.ReadAllBytesAsync(fixture.ItemsPath));
        var saved = XDocument.Load(fixture.DealerPath);
        var row = saved.Root!.Elements("INVENTORY").First(static element => element.Element("uiIndex")?.Value == "0");
        Assert.Equal("12", row.Element("ubOptimalNumber")?.Value);
        Assert.Equal("1", row.Element("sItemIndex")?.Value);
    }

    /// <summary>
    /// An external change in a sub-table is reported with the table name; overwriting saves the item and
    /// the changed sub-table in one retry.
    /// </summary>
    [Fact]
    public async Task Save_ExternalChangeInSubTable_OverwriteSavesWholeSet()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        viewModel.ItemGroups
            .SelectMany(group => group.Fields)
            .Single(field => field.Name == "szItemName")
            .Value = "Glock 17A";
        viewModel.SubTableSections
            .Single(section => section.TableName == Weapons)
            .Fields
            .Single(field => field.Name == "usRange")
            .Value = "150";
        await File.AppendAllTextAsync(fixture.WeaponPath, " ");
        dialog.ExternalChangeResult = ExternalChangeChoice.Overwrite;

        var saved = await viewModel.SaveWithRecoveryAsync();

        Assert.True(saved);
        Assert.False(viewModel.IsDirty);
        Assert.Contains("Items.Weapons", dialog.LastExternalChangeMessage, StringComparison.Ordinal);
        Assert.Equal(
            "Glock 17A",
            XDocument.Load(fixture.ItemsPath).Root!
                .Elements("ITEM").First(item => item.Element("uiIndex")?.Value == "1")
                .Element("szItemName")!.Value);
        Assert.Equal(
            "150",
            XDocument.Load(fixture.WeaponPath).Root!
                .Elements("WEAPON").First(weapon => weapon.Element("uiIndex")?.Value == "1")
                .Element("usRange")!.Value);
    }

    /// <summary>
    /// Reloading after an external change discards item-set edits and rereads every file.
    /// </summary>
    [Fact]
    public async Task Save_ExternalChangeInSubTable_ReloadKeepsExternalContents()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (viewModel, dialog) = await CreateWorkspaceAsync(fixture);
        viewModel.SubTableSections
            .Single(section => section.TableName == Weapons)
            .Fields
            .Single(field => field.Name == "usRange")
            .Value = "150";
        await File.AppendAllTextAsync(fixture.WeaponPath, " ");
        dialog.ExternalChangeResult = ExternalChangeChoice.Reload;

        var saved = await viewModel.SaveWithRecoveryAsync();

        Assert.True(saved);
        Assert.False(viewModel.IsDirty);
        var range = viewModel.SubTableSections
            .Single(section => section.TableName == Weapons)
            .Fields
            .Single(field => field.Name == "usRange");
        Assert.Equal("100", range.Value);
        Assert.Contains("Items.Weapons", dialog.LastExternalChangeMessage, StringComparison.Ordinal);
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> set, the attachment child section of item 12 shows exactly the rows
    /// stored for that item instead of the rows of other items that share an attachment index.
    /// </summary>
    [Fact]
    public async Task Corpus_Item12ShowsOnlyItsOwnAttachments()
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
        var session = new DataSetSession(corpus, provider, catalog, repository);
        var descriptors = await catalog.GetTablesAsync(corpus);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);
        var viewModel = new ItemWorkspaceViewModel(
            session,
            descriptor,
            languageCode: null,
            new FakeDialogService(),
            new TestItemImageService());
        await viewModel.InitializeAsync();

        viewModel.SelectRecord("12");

        var editor = viewModel.ChildSections
            .Single(section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.ChildRows)
            .Editor;
        var rawCount = XDocument
            .Load(Path.Combine(corpus, "Items", "Attachments.xml"))
            .Root!
            .Elements("ATTACHMENT")
            .Count(element => element.Element("itemIndex")?.Value == "12");
        Assert.True(rawCount > 0);
        Assert.Equal(rawCount, editor.Rows.Count);
        Assert.All(editor.Rows, row => Assert.Equal("12", row["itemIndex"]));

        var attachToEditor = viewModel.ChildSections
            .Single(section => section.TableName == Attachments
                && section.Member.Relation.Kind == TableRelationKind.Reverse)
            .Editor;
        var rawAttachToCount = XDocument
            .Load(Path.Combine(corpus, "Items", "Attachments.xml"))
            .Root!
            .Elements("ATTACHMENT")
            .Count(element => element.Element("attachmentIndex")?.Value == "12");
        Assert.Equal(rawAttachToCount, attachToEditor.Rows.Count);
        Assert.All(attachToEditor.Rows, row => Assert.Equal("12", row["attachmentIndex"]));
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> set, every item-list preset matches the class and flag values of the
    /// raw items file, and class-aware sections hide the legacy pages that do not apply to the selected
    /// item class.
    /// </summary>
    [Fact]
    public async Task Corpus_ItemFiltersAndClassVisibility()
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
        var session = new DataSetSession(corpus, provider, catalog, repository);
        var descriptors = await catalog.GetTablesAsync(corpus);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);
        var viewModel = new ItemWorkspaceViewModel(
            session,
            descriptor,
            languageCode: null,
            new FakeDialogService(),
            new TestItemImageService());
        await viewModel.InitializeAsync();

        var records = XDocument
            .Load(Path.Combine(corpus, "Items", "Items.xml"))
            .Root!
            .Elements("ITEM")
            .ToArray();
        var expected = new Dictionary<string, int>
        {
            ["all"] = records.Length,
            ["class.guns"] = records.Count(record =>
                ClassOf(record) == 2 && !string.Equals(FieldOf(record, "RocketLauncher"), "1", StringComparison.Ordinal)),
            ["class.ammo"] = records.Count(record => ClassOf(record) == 1024),
            ["class.launchers"] = records.Count(record =>
                ClassOf(record) == 16 || string.Equals(FieldOf(record, "RocketLauncher"), "1", StringComparison.Ordinal)),
            ["class.grenades"] = records.Count(record => ClassOf(record) == 256),
            ["class.explosives"] = records.Count(record => ClassOf(record) == 512),
            ["class.knives"] = records.Count(record => ClassOf(record) is 4 or 8),
            ["class.other-weapons"] = records.Count(record => ClassOf(record) is 32 or 64 or 128),
            ["class.armour"] = records.Count(record => ClassOf(record) == 2048),
            ["class.face"] = records.Count(record => ClassOf(record) == 32768),
            ["class.kits"] = records.Count(record => ClassOf(record) is 4096 or 8192),
            ["class.keys"] = records.Count(record => ClassOf(record) == 65536),
            ["class.lbe"] = records.Count(record => ClassOf(record) == 131072),
            ["class.misc"] = records.Count(record => ClassOf(record) is 268435456 or 536870912),
            ["class.none"] = records.Count(record => ClassOf(record) == 1),
            ["class.random"] = records.Count(record => ClassOf(record) == 1073741824),
            ["flag.attachments"] = records.Count(record => FieldOf(record, "Attachment") == "1"),
            ["flag.drugs"] = records.Count(record => FieldOf(record, "Medical") == "1"),
            ["flag.big-gun-list"] = records.Count(record => FieldOf(record, "BigGunList") == "1"),
            ["flag.normal-guns"] = records.Count(record => FieldOf(record, "BigGunList") != "1"),
            ["flag.scifi"] = records.Count(record => FieldOf(record, "SciFi") == "1"),
            ["flag.realistic"] = records.Count(record => FieldOf(record, "SciFi") != "1"),
        };

        foreach (var pair in expected)
        {
            if (pair.Key.StartsWith("class.", StringComparison.Ordinal))
            {
                viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions[0];
                viewModel.SelectedClassFilter = viewModel.ClassFilterOptions.Single(option => option.Key == pair.Key);
            }
            else
            {
                viewModel.SelectedClassFilter = viewModel.ClassFilterOptions[0];
                viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions.Single(option => option.Key == pair.Key);
            }

            Assert.Equal(pair.Value, viewModel.Items.Count);
        }

        viewModel.SelectedClassFilter = viewModel.ClassFilterOptions[0];
        viewModel.SelectedFlagFilter = viewModel.FlagFilterOptions[0];
        Assert.Equal(records.Length, viewModel.Items.Count);

        var keyRecord = records.First(record => ClassOf(record) == 65536);
        viewModel.SelectRecord(FieldOf(keyRecord, "uiIndex")!);
        Assert.DoesNotContain(viewModel.ItemGroups, group => group.Key == "combat");
        Assert.DoesNotContain(viewModel.ItemGroups, group => group.Key == "attachments");
        Assert.Contains(viewModel.ItemGroups, group => group.Key == "identity");

        var gunRecord = records.First(record => ClassOf(record) == 2);
        viewModel.SelectRecord(FieldOf(gunRecord, "uiIndex")!);
        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Weapons);

        var ammoRecord = records.First(record => ClassOf(record) == 1024);
        viewModel.SelectRecord(FieldOf(ammoRecord, "uiIndex")!);
        Assert.Contains(viewModel.SubTableSections, section => section.TableName == Magazines);
        Assert.DoesNotContain(viewModel.SubTableSections, section => section.TableName == Weapons);
    }

    private static long ClassOf(XElement record) =>
        long.TryParse(FieldOf(record, "usItemClass"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0;

    private static string? FieldOf(XElement record, string fieldName) =>
        record.Element(fieldName)?.Value.Trim();

    private static string[] VisibleKeys(ItemWorkspaceViewModel viewModel) =>
        viewModel.Items
            .Select(static entry => entry.Key)
            .OrderBy(static key => key, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// Builds a fixture whose relation rows exercise both directions: item 10 is a launcher that
    /// launches item 13, item 17 is a face item compatible with item 18, and attachments rows make
    /// item 1 attachable to item 10 through <c>attachmentIndex</c>.
    /// </summary>
    private static Fixture CreateRelationFixture(TestWorkspace workspace)
    {
        var fixture = CreateFilterFixture(workspace);
        var itemsDirectory = Path.GetDirectoryName(fixture.ItemsPath)!;
        File.WriteAllText(
            Path.Combine(itemsDirectory, "CompatibleFaceItems.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><COMPATIBLEFACEITEMLIST>
            <COMPATIBLEFACEITEM><compatiblefaceitemIndex>18</compatiblefaceitemIndex><itemIndex>17</itemIndex></COMPATIBLEFACEITEM>
            </COMPATIBLEFACEITEMLIST>
            """);
        File.WriteAllText(
            Path.Combine(itemsDirectory, "Launchables.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><LAUNCHABLELIST>
            <LAUNCHABLE><launchableIndex>13</launchableIndex><itemIndex>10</itemIndex></LAUNCHABLE>
            </LAUNCHABLELIST>
            """);
        return fixture;
    }

    /// <summary>
    /// Builds a fixture with one item of every legacy item class and the flag combinations used by the
    /// item list presets.
    /// </summary>
    private static Fixture CreateFilterFixture(TestWorkspace workspace)
    {
        var fixture = CreateFixture(workspace);
        var lookupDirectory = Path.Combine(fixture.TableDataDirectory, "Lookup");
        File.WriteAllText(
            Path.Combine(lookupDirectory, "ItemClass.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><JA2Data>
            <ItemClass><id>1</id><name>Nothing</name></ItemClass>
            <ItemClass><id>2</id><name>Gun</name></ItemClass>
            <ItemClass><id>4</id><name>Knife</name></ItemClass>
            <ItemClass><id>8</id><name>Throwing Knife</name></ItemClass>
            <ItemClass><id>16</id><name>Launcher</name></ItemClass>
            <ItemClass><id>32</id><name>Tentacle</name></ItemClass>
            <ItemClass><id>64</id><name>Thrown</name></ItemClass>
            <ItemClass><id>128</id><name>Punch</name></ItemClass>
            <ItemClass><id>256</id><name>Grenade</name></ItemClass>
            <ItemClass><id>512</id><name>Bomb</name></ItemClass>
            <ItemClass><id>1024</id><name>Ammo</name></ItemClass>
            <ItemClass><id>2048</id><name>Armour</name></ItemClass>
            <ItemClass><id>4096</id><name>Med Kit</name></ItemClass>
            <ItemClass><id>8192</id><name>Kit</name></ItemClass>
            <ItemClass><id>32768</id><name>Facial Gear</name></ItemClass>
            <ItemClass><id>65536</id><name>Key</name></ItemClass>
            <ItemClass><id>131072</id><name>LBE</name></ItemClass>
            <ItemClass><id>268435456</id><name>Misc</name></ItemClass>
            <ItemClass><id>536870912</id><name>Money</name></ItemClass>
            <ItemClass><id>1073741824</id><name>Random Item</name></ItemClass>
            </JA2Data>
            """);

        File.WriteAllText(
            fixture.ItemsPath,
            """
            <?xml version="1.0" encoding="utf-8"?><ITEMLIST>
            <ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName><usItemClass>2</usItemClass><ubClassIndex>1</ubClassIndex><ProjectionFactor>1.0</ProjectionFactor><DefaultAttachment>7</DefaultAttachment><DefaultAttachment>6</DefaultAttachment><STAND_MODIFIERS><FlatBase>0</FlatBase></STAND_MODIFIERS><CustomUnknown>keep me</CustomUnknown></ITEM>
            <ITEM><uiIndex>10</uiIndex><szItemName>RPG-7</szItemName><usItemClass>16</usItemClass><ubClassIndex>2</ubClassIndex><RocketLauncher>1</RocketLauncher></ITEM>
            <ITEM><uiIndex>11</uiIndex><szItemName>5.56mm Magazine</szItemName><usItemClass>1024</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus><PercentNoiseReduction>0</PercentNoiseReduction><FoodType>0</FoodType></ITEM>
            <ITEM><uiIndex>12</uiIndex><szItemName>Kevlar Vest</szItemName><usItemClass>2048</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>13</uiIndex><szItemName>Stun Grenade</szItemName><usItemClass>256</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>14</uiIndex><szItemName>TNT</szItemName><usItemClass>512</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>15</uiIndex><szItemName>Combat Knife</szItemName><usItemClass>4</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>16</uiIndex><szItemName>Rock</szItemName><usItemClass>64</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>17</uiIndex><szItemName>Extended Ear</szItemName><usItemClass>32768</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>18</uiIndex><szItemName>First Aid Kit</szItemName><usItemClass>4096</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>19</uiIndex><szItemName>Door Key</szItemName><usItemClass>65536</usItemClass><Attachment>1</Attachment><ToHitBonus>0</ToHitBonus><PercentNoiseReduction>0</PercentNoiseReduction><FoodType>0</FoodType><ProjectionFactor>1.5</ProjectionFactor><DefaultAttachment>5</DefaultAttachment><STAND_MODIFIERS><FlatBase>0</FlatBase></STAND_MODIFIERS></ITEM>
            <ITEM><uiIndex>20</uiIndex><szItemName>Combat Pack</szItemName><usItemClass>131072</usItemClass><ToHitBonus>0</ToHitBonus><PercentNoiseReduction>0</PercentNoiseReduction><FoodType>0</FoodType></ITEM>
            <ITEM><uiIndex>21</uiIndex><szItemName>Loose Wire</szItemName><usItemClass>268435456</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>22</uiIndex><szItemName>Money</szItemName><usItemClass>536870912</usItemClass></ITEM>
            <ITEM><uiIndex>23</uiIndex><szItemName>Nothing</szItemName><usItemClass>1</usItemClass></ITEM>
            <ITEM><uiIndex>24</uiIndex><szItemName>Random Loot</szItemName><usItemClass>1073741824</usItemClass><Attachment>1</Attachment><ToHitBonus>0</ToHitBonus><PercentNoiseReduction>10</PercentNoiseReduction><FoodType>2</FoodType><ProjectionFactor>1.0</ProjectionFactor></ITEM>
            <ITEM><uiIndex>25</uiIndex><szItemName>Toolkit</szItemName><usItemClass>8192</usItemClass><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>26</uiIndex><szItemName>Punch</szItemName><usItemClass>128</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>27</uiIndex><szItemName>Tentacle</szItemName><usItemClass>32</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>28</uiIndex><szItemName>Throwing Knife</szItemName><usItemClass>8</usItemClass><ubClassIndex>1</ubClassIndex><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>30</uiIndex><szItemName>Sci-Fi Gun</szItemName><usItemClass>2</usItemClass><ubClassIndex>1</ubClassIndex><SciFi>1</SciFi><BigGunList>1</BigGunList><Attachment>1</Attachment><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>31</uiIndex><szItemName>Real Gun</szItemName><usItemClass>2</usItemClass><ubClassIndex>1</ubClassIndex><SciFi>0</SciFi><BigGunList>0</BigGunList><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>32</uiIndex><szItemName>Medical Misc</szItemName><usItemClass>268435456</usItemClass><Medical>1</Medical><ToHitBonus>0</ToHitBonus></ITEM>
            <ITEM><uiIndex>33</uiIndex><szItemName>Rocket Gun</szItemName><usItemClass>2</usItemClass><ubClassIndex>1</ubClassIndex><RocketLauncher>1</RocketLauncher></ITEM>
            <ITEM><uiIndex>34</uiIndex><szItemName>Mortar</szItemName><usItemClass>16</usItemClass><ubClassIndex>2</ubClassIndex><RocketLauncher>0</RocketLauncher></ITEM>
            </ITEMLIST>
            """);

        File.WriteAllText(
            Path.Combine(Path.GetDirectoryName(fixture.ItemsPath)!, "CompatibleFaceItems.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><COMPATIBLEFACEITEMLIST>
            <COMPATIBLEFACEITEM><compatiblefaceitemIndex>17</compatiblefaceitemIndex><itemIndex>210</itemIndex></COMPATIBLEFACEITEM>
            </COMPATIBLEFACEITEMLIST>
            """);

        File.WriteAllText(
            Path.Combine(Path.GetDirectoryName(fixture.ItemsPath)!, "Launchables.xml"),
            """
            <?xml version="1.0" encoding="utf-8"?><LAUNCHABLELIST>
            <LAUNCHABLE><launchableIndex>13</launchableIndex><itemIndex>10</itemIndex></LAUNCHABLE>
            </LAUNCHABLELIST>
            """);

        return fixture;
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
        bool includeOverlay = false,
        bool includeDealer = false)
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

        var dealerPath = Path.Combine(tableData, "NPCInventory", "AlbertoInventory.xml");
        if (includeDealer)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dealerPath)!);
            File.WriteAllText(
                dealerPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><INVENTORYLIST>\r\n"
                + "\t<INVENTORY>\r\n"
                + "\t\t<uiIndex>0</uiIndex>\r\n"
                + "\t\t<sItemIndex>1</sItemIndex>\r\n"
                + "\t\t<ubOptimalNumber>9</ubOptimalNumber>\r\n"
                + "\t</INVENTORY>\r\n"
                + "</INVENTORYLIST>\r\n");
        }

        return new Fixture(workspace, tableData, itemsPath, weaponPath, attachmentsPath, dealerPath);
    }

    private sealed record Fixture(
        TestWorkspace Workspace,
        string TableDataDirectory,
        string ItemsPath,
        string WeaponPath,
        string AttachmentsPath,
        string DealerPath);

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; }

        public UnsavedChangesChoice UnsavedChangesResult { get; set; } = UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ExternalChangeResult { get; set; } = ExternalChangeChoice.Cancel;

        public string? LastExternalChangeMessage { get; private set; }

        public ItemCreationRequest? CreateItemResult { get; set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesResult;

        public ExternalChangeChoice ResolveExternalChange(string message, string title)
        {
            LastExternalChangeMessage = message;
            return ExternalChangeResult;
        }

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
