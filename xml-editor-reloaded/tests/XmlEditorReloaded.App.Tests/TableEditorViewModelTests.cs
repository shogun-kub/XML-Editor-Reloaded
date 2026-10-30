using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the editable table grid view model and view.
/// </summary>
public class TableEditorViewModelTests
{
    /// <summary>
    /// Initialization discovers the schema, builds default columns, and loads rows.
    /// </summary>
    [Fact]
    public async Task Initialize_BuildsColumnsRowsAndBaseLanguage()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        Assert.True(editor.IsEditable);
        Assert.False(editor.IsReadOnly);
        Assert.Equal("Items.Items", editor.DisplayName);
        Assert.Equal("Base", editor.LanguageText);
        Assert.Equal(3, editor.Rows.Count);
        Assert.Equal(5, editor.Columns.Count);
        Assert.Equal(5, editor.VisibleColumns.Count);
        Assert.True(editor.VisibleColumns[0].IsKey);
        Assert.Contains("3", editor.RecordCountText, StringComparison.Ordinal);
    }

    /// <summary>
    /// A table whose last row was deleted reopens editable, so the grid can add the first row again and
    /// save it.
    /// </summary>
    [Fact]
    public async Task EmptyTable_ReopensEditableAndAddsFirstRow()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, _, _) = await CreateEditorAsync(
            workspace,
            includeOverlay: false,
            emptyTable: true);

        Assert.True(editor.IsEditable);
        Assert.False(editor.IsReadOnly);
        Assert.Empty(editor.Rows);

        editor.AddRowCommand.Execute(null);

        Assert.Single(editor.Rows);
        Assert.Equal("0", editor.SelectedRow!.Key);
        Assert.True(editor.IsDirty);

        await editor.SaveCommand.ExecuteAsync(null);

        Assert.False(editor.IsDirty);
        var saved = XDocument.Load(descriptor.FilePath);
        Assert.Equal("0", Assert.Single(saved.Root!.Elements("ITEM")).Element("uiIndex")?.Value);
    }

    /// <summary>
    /// A table whose records are nested in the root (Map.Cities keeps them under CITYLIST/CITY) shows
    /// its real columns and rows, hides the nested structures from the default grid, and keeps every
    /// row read-only instead of failing with a key error.
    /// </summary>
    [Fact]
    public async Task NestedCityTable_ShowsRowsAndColumnsAndStaysReadOnly()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var mapDirectory = Path.Combine(tableData, "Map");
        Directory.CreateDirectory(mapDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(mapDirectory, "Cities.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO><CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Omerta</townName><townUsesLoyalty>1</townUsesLoyalty>"
            + "<baseSector><x>9</x><y>1</y></baseSector></CITY>"
            + "<CITY><uiIndex>2</uiIndex><townName>Drassen</townName><townUsesLoyalty>1</townUsesLoyalty>"
            + "<baseSector><x>12</x><y>2</y></baseSector></CITY>"
            + "</CITYLIST></CITY_INFO>");

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = (await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Map.Cities");
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            null,
            new FakeDialogService(),
            new TestItemImageService());

        await editor.InitializeAsync();

        Assert.Equal(2, editor.Rows.Count);
        Assert.Equal(new[] { "1", "2" }, editor.Rows.Select(static row => row.Key));
        Assert.Equal("Omerta", editor.Rows[0]["townName"]);
        Assert.Equal("Drassen", editor.Rows[1]["townName"]);
        Assert.NotNull(editor.Columns.Single(column => column.Name == "uiIndex"));
        Assert.NotNull(editor.Columns.Single(column => column.Name == "townName"));
        Assert.Equal("uiIndex", editor.VisibleColumns[0].Name);

        var nested = Assert.Single(editor.Columns, column => column.Name == "baseSector");
        Assert.True(nested.Column.IsNested);
        Assert.DoesNotContain(editor.VisibleColumns, column => column.Name == "baseSector");

        Assert.False(editor.IsEditable);
        Assert.True(editor.IsReadOnly);
        Assert.All(editor.Rows, static row => Assert.False(row.CanEdit));

        editor.Rows[0]["townName"] = "Changed";

        Assert.Equal("Omerta", editor.Rows[0]["townName"]);
        Assert.Contains("read-only", editor.StatusText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("key.not_found", editor.StatusText, StringComparison.Ordinal);
    }

    /// <summary>
    /// Typing into an empty cell adds the field, while invalid numeric text is rejected without changes.
    /// </summary>
    [Fact]
    public async Task SetCellValue_AddsMissingFieldAndRejectsInvalidNumbers()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, data13, _, _) = await CreateEditorAsync(workspace);
        var row = Assert.Single(editor.Rows, candidate => candidate.Key == "0");

        row["ItemClass"] = "abc";
        Assert.Equal(string.Empty, row["ItemClass"]);
        Assert.False(editor.Workspace!.BaseDocument.Records[0].HasField("ItemClass"));
        Assert.False(editor.IsDirty);

        row["ubWeight"] = "abc";
        Assert.Equal("0", row["ubWeight"]);

        row["ItemClass"] = "7";
        Assert.Equal("7", row["ItemClass"]);
        Assert.True(editor.IsDirty);

        await editor.SaveCommand.ExecuteAsync(null);

        Assert.False(editor.IsDirty);
        Assert.Contains(
            "<ItemClass>7</ItemClass>",
            await File.ReadAllTextAsync(descriptor.FilePath),
            StringComparison.Ordinal);
        Assert.Equal(
            Path.Combine(data13, "TableData", "Items", "Items.xml"),
            descriptor.FilePath);
    }

    /// <summary>
    /// An indexed (dict) family such as <c>ItemCapacityPerSizeN</c> is edited through its concrete
    /// expanded columns. The hint added for the pocket capacities validates the type, a missing index is
    /// created on demand, and the detail panel exposes the same field as editable.
    /// </summary>
    [Fact]
    public async Task DictFields_AreEditableValidatedAndCreatedOnDemand()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var pocketsPath = Path.Combine(itemsDirectory, "Pockets.xml");
        await File.WriteAllTextAsync(
            pocketsPath,
            """
            <?xml version="1.0" encoding="utf-8"?><POCKETLIST>
            <POCKET><pIndex>1</pIndex><ItemCapacityPerSize0>1</ItemCapacityPerSize0><ItemCapacityPerSize2>2</ItemCapacityPerSize2></POCKET>
            <POCKET><pIndex>2</pIndex><ItemCapacityPerSize0>3</ItemCapacityPerSize0></POCKET>
            </POCKETLIST>
            """);

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = (await new DataSetSessionFactory(provider, catalog, repository)
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Pockets");
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            languageCode: null,
            new FakeDialogService(),
            new TestItemImageService());
        await editor.InitializeAsync();

        var column = editor.Columns.Single(candidate => candidate.Name == "ItemCapacityPerSize0");
        Assert.True(column.IsDict);
        Assert.False(column.IsReadOnly);

        var row = editor.Rows.Single(candidate => candidate.Key == "2");
        editor.SelectedRow = row;
        row["ItemCapacityPerSize0"] = "abc";
        Assert.Equal("3", row["ItemCapacityPerSize0"]);

        row["ItemCapacityPerSize2"] = "7";
        Assert.Equal("7", row["ItemCapacityPerSize2"]);
        Assert.True(editor.IsDirty);

        var detail = FlattenDetailFields(editor).Single(field => field.Name == "ItemCapacityPerSize2");
        Assert.False(detail.IsReadOnly);

        await editor.SaveCommand.ExecuteAsync(null);

        var document = XDocument.Load(pocketsPath);
        var pocket = document.Root!
            .Elements("POCKET")
            .Single(element => element.Element("pIndex")?.Value == "2");
        Assert.Equal("7", pocket.Element("ItemCapacityPerSize2")?.Value);
        Assert.Equal("3", pocket.Element("ItemCapacityPerSize0")?.Value);
    }

    /// <summary>
    /// Undo and redo restore grid values, dirty state, and command availability.
    /// </summary>
    [Fact]
    public async Task UndoRedoCommands_RestoreCellAndHistoryState()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        Assert.False(editor.CanUndo);
        Assert.False(editor.CanRedo);

        Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"] = "12";

        Assert.True(editor.CanUndo);
        Assert.False(editor.CanRedo);
        Assert.True(editor.IsDirty);
        Assert.Equal("12", Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);

        editor.UndoCommand.Execute(null);

        Assert.Equal("10", Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);
        Assert.False(editor.CanUndo);
        Assert.True(editor.CanRedo);
        Assert.False(editor.IsDirty);

        editor.RedoCommand.Execute(null);

        Assert.Equal("12", Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);
        Assert.True(editor.CanUndo);
        Assert.False(editor.CanRedo);
        Assert.True(editor.IsDirty);
    }

    /// <summary>
    /// The bulk command passes the current rows and editable columns to the dialog and applies the
    /// confirmed request as one undoable transaction.
    /// </summary>
    [Fact]
    public async Task BulkEditCommand_AppliesRequestAndUndoesAsOneStep()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        dialog.BulkEditHandler = context =>
        {
            Assert.Equal(3, context.RecordKeys.Count);
            Assert.Contains(context.Columns, column => column.FieldName == "ubWeight");
            Assert.DoesNotContain(context.Columns, column => column.FieldName == "uiIndex");
            return new BulkEditRequest(
                "ubWeight",
                BulkEditOperation.Set,
                "7",
                IncludeMissingFields: false,
                context.RecordKeys);
        };

        editor.BulkEditCommand.Execute(null);

        Assert.All(editor.Rows, row => Assert.Equal("7", row["ubWeight"]));
        Assert.True(editor.CanUndo);
        Assert.True(editor.IsDirty);
        Assert.Contains("7", editor.StatusText, StringComparison.Ordinal);

        editor.UndoCommand.Execute(null);

        Assert.Equal("0", Assert.Single(editor.Rows, row => row.Key == "0")["ubWeight"]);
        Assert.Equal("10", Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);
        Assert.Equal("2", Assert.Single(editor.Rows, row => row.Key == "2")["ubWeight"]);
        Assert.False(editor.CanUndo);
        Assert.False(editor.IsDirty);
    }

    /// <summary>
    /// Cancelling the bulk dialog leaves the table unchanged.
    /// </summary>
    [Fact]
    public async Task BulkEditCommand_Cancelled_DoesNotChangeTable()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        dialog.BulkEditHandler = _ => null;

        editor.BulkEditCommand.Execute(null);

        Assert.All(editor.Rows, row => Assert.NotEqual("7", row["ubWeight"]));
        Assert.False(editor.IsDirty);
        Assert.False(editor.CanUndo);
        Assert.NotNull(dialog.LastBulkContext);
        Assert.Equal(3, dialog.LastBulkContext!.RecordKeys.Count);
    }

    /// <summary>
    /// With a language selected, translated cells show overlay values and edits stay in the overlay.
    /// </summary>
    [Fact]
    public async Task OverlayLanguage_ShowsAndWritesTranslations()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, _, _) = await CreateEditorAsync(workspace, language: "ru");

        Assert.Contains("Russian", editor.LanguageText, StringComparison.Ordinal);
        var translated = Assert.Single(editor.Rows, row => row.Key == "1");
        var untranslated = Assert.Single(editor.Rows, row => row.Key == "0");
        Assert.Equal("Глок 17", translated["szItemName"]);
        Assert.True(translated.IsTranslated("szItemName"));
        Assert.False(untranslated.IsTranslated("szItemName"));
        var baseBefore = await File.ReadAllBytesAsync(descriptor.FilePath);

        translated["szItemName"] = "Глок 18";

        Assert.Equal("Глок 18", translated["szItemName"]);
        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(descriptor.FilePath));
        await editor.SaveCommand.ExecuteAsync(null);
        Assert.False(editor.IsDirty);
        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(descriptor.FilePath));
        Assert.Contains(
            "<szItemName>Глок 18</szItemName>",
            await File.ReadAllTextAsync(descriptor.Overlays[0].OverlayFilePath),
            StringComparison.Ordinal);
    }

    /// <summary>
    /// The grid uses a combo template for lookup columns, a checkbox for Boolean columns, and a text
    /// editor when a configured lookup cannot be loaded.
    /// </summary>
    [Fact]
    public async Task View_BuildsLookupAndBooleanEditors()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            includeLookups: true,
            includeBooleans: true);
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));

                var lookup = Assert.Single(
                    grid.Columns,
                    column => column.SortMemberPath == "ubCursor");
                Assert.IsType<DataGridTemplateColumn>(lookup);

                var boolean = Assert.Single(
                    grid.Columns,
                    column => column.SortMemberPath == "AllowClimbing");
                Assert.IsType<DataGridCheckBoxColumn>(boolean);

                var fallback = Assert.Single(
                    grid.Columns,
                    column => column.SortMemberPath == "Inseparable");
                Assert.IsType<DataGridTextColumn>(fallback);
                var header = Assert.IsType<TextBlock>(fallback.Header);
                Assert.Contains("⚠", header.Text, StringComparison.Ordinal);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The grid editor test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// Add, duplicate, and delete operate on the selected row and respect confirmation.
    /// </summary>
    [Fact]
    public async Task RowCommands_AddDuplicateAndDelete()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, dialog, _) = await CreateEditorAsync(workspace);

        editor.AddRowCommand.Execute(null);
        Assert.Equal(4, editor.Rows.Count);
        Assert.Equal("3", editor.SelectedRow!.Key);
        Assert.True(editor.IsDirty);

        editor.DuplicateRowCommand.Execute(null);
        Assert.Equal(5, editor.Rows.Count);
        Assert.Equal("4", editor.SelectedRow!.Key);

        dialog.ConfirmResult = false;
        editor.DeleteRowCommand.Execute(null);
        Assert.Equal(5, editor.Rows.Count);

        dialog.ConfirmResult = true;
        editor.DeleteRowCommand.Execute(null);
        Assert.Equal(4, editor.Rows.Count);

        await editor.SaveCommand.ExecuteAsync(null);

        Assert.False(editor.IsDirty);
        var saved = System.Xml.Linq.XDocument.Load(descriptor.FilePath);
        Assert.Equal(4, saved.Root!.Elements("ITEM").Count());
        Assert.Contains(saved.Root.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "3");
        Assert.DoesNotContain(saved.Root.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "4");
    }

    /// <summary>
    /// Search filters visible cells and sorting orders rows by value.
    /// </summary>
    [Fact]
    public async Task SearchAndSort_FilterAndOrderRows()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        editor.SearchText = "Glock";
        Assert.Equal("1", Assert.Single(editor.Rows).Key);

        editor.SearchText = string.Empty;
        Assert.Equal(3, editor.Rows.Count);

        editor.SortBy("ubWeight", ListSortDirection.Descending);
        Assert.Equal(new[] { "1", "2", "0" }, editor.Rows.Select(row => row.Key));

        editor.SortBy("szItemName", ListSortDirection.Ascending);
        Assert.Equal(
            new[] { "Glock 17", "Knife", "Nothing" },
            editor.Rows.Select(row => row["szItemName"]));
    }

    /// <summary>
    /// A no-match search message is cleared when the search is emptied or yields rows again, while a
    /// status set after the message (for example a save result) is preserved.
    /// </summary>
    [Fact]
    public async Task Search_NoMatchesStatusClearsWhenRowsReturn()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        editor.SearchText = "zzz";
        Assert.Empty(editor.Rows);
        Assert.Equal("No records match the search.", editor.StatusText);
        Assert.Contains("0 of 3", editor.RecordCountText, StringComparison.Ordinal);

        editor.SearchText = "Glock";
        Assert.Equal("1", Assert.Single(editor.Rows).Key);
        Assert.DoesNotContain("No records match", editor.StatusText, StringComparison.Ordinal);
        Assert.DoesNotContain("0 of 3", editor.RecordCountText, StringComparison.Ordinal);

        editor.SearchText = "zzz";
        Assert.Equal("No records match the search.", editor.StatusText);

        editor.SearchText = string.Empty;
        Assert.Equal(3, editor.Rows.Count);
        Assert.Equal("Ready.", editor.StatusText);
        Assert.Contains("3 of 3", editor.RecordCountText, StringComparison.Ordinal);

        editor.SearchText = "zzz";
        editor.StatusText = "Saved.";
        editor.SearchText = string.Empty;

        Assert.Equal("Saved.", editor.StatusText);
        Assert.Equal(3, editor.Rows.Count);
    }

    /// <summary>
    /// A transient status message, such as "No changes to save.", is replaced by the idle status after
    /// the configured delay instead of staying on screen until the next edit (S12).
    /// </summary>
    [Fact]
    public async Task StatusText_TransientMessage_ResetsToIdleAfterDelay()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            statusAutoResetDelay: TimeSpan.FromMilliseconds(50));

        await editor.SaveAsync();
        Assert.Equal("No changes to save.", editor.StatusText);

        await WaitForStatusAsync(editor, "Ready.");
        Assert.Equal("Ready.", editor.StatusText);
    }

    /// <summary>
    /// A cell-update confirmation is transient too, so the next operation is not reported next to an
    /// outdated message (S12).
    /// </summary>
    [Fact]
    public async Task StatusText_CellUpdate_ResetsToIdleAfterDelay()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            statusAutoResetDelay: TimeSpan.FromMilliseconds(50));

        Assert.True(editor.SetCellValue(editor.Rows[0], "szItemName", "Edited").IsSuccess);
        Assert.Equal("szItemName updated in record 0.", editor.StatusText);

        await WaitForStatusAsync(editor, "Ready.");
        Assert.Equal("Ready.", editor.StatusText);
    }

    /// <summary>
    /// Errors must survive the transient-status reset window, otherwise a failed save or a rejected
    /// value silently looks successful one moment later.
    /// </summary>
    [Fact]
    public async Task StatusText_Error_IsNotAutoCleared()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            statusAutoResetDelay: TimeSpan.FromMilliseconds(50));

        Assert.False(editor.SetCellValue(editor.Rows[0], "ubWeight", "not a number").IsSuccess);
        var error = editor.StatusText;
        Assert.Contains("not a valid value", error, StringComparison.Ordinal);

        await Task.Delay(150);

        Assert.Equal(error, editor.StatusText);
    }

    private static async Task WaitForStatusAsync(TableEditorViewModel editor, string expected)
    {
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (!string.Equals(editor.StatusText, expected, StringComparison.Ordinal)
               && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }
    }

    /// <summary>
    /// Flag masks are sorted as unsigned 64-bit values, so the signed 64-bit boundary and the largest
    /// mask do not wrap into a negative value or compare as text.
    /// </summary>
    [Fact]
    public async Task SortByFlagMask_OrdersUnsignedValuesAcrossSignedBoundary()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        editor.Rows[0]["AttachmentClass"] = "18446744073709551615";
        editor.Rows[1]["AttachmentClass"] = "9223372036854775808";
        editor.Rows[2]["AttachmentClass"] = "9223372036854775807";

        editor.SortBy("AttachmentClass", ListSortDirection.Ascending);

        Assert.Equal(new[] { "2", "1", "0" }, editor.Rows.Select(row => row.Key));
        Assert.Equal("18446744073709551615", editor.Rows[2]["AttachmentClass"]);
    }

    /// <summary>
    /// Column visibility updates the visible set and the chooser filter.
    /// </summary>
    [Fact]
    public async Task ColumnVisibility_UpdatesVisibleColumns()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        var weight = Assert.Single(editor.Columns, column => column.Name == "ubWeight");
        weight.IsVisible = false;
        Assert.Equal(4, editor.VisibleColumns.Count);
        Assert.DoesNotContain(editor.VisibleColumns, column => column.Name == "ubWeight");

        var notifications = new List<string?>();
        weight.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);
        editor.ShowAllColumnsCommand.Execute(null);
        Assert.True(weight.IsVisible);
        // The chooser binds checkbox state to IsVisible, so "Show all" must raise exactly this
        // property; a notification under another name leaves the checkboxes visually unchanged.
        Assert.Contains(nameof(TableColumnViewModel.IsVisible), notifications);
        Assert.Equal(5, editor.VisibleColumns.Count);

        editor.HideAllColumnsCommand.Execute(null);
        Assert.Empty(editor.VisibleColumns);

        editor.ShowDefaultColumnsCommand.Execute(null);
        Assert.Equal(5, editor.VisibleColumns.Count);

        editor.ColumnSearchText = "ubW";
        Assert.Equal("ubWeight", Assert.Single(editor.Columns).Name);
    }

    /// <summary>
    /// Structured filters apply type-aware criteria and combine with search using AND semantics.
    /// </summary>
    [Fact]
    public async Task Filter_AppliesCriteriaAndCombinesWithSearch()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);
        var filter = Assert.IsType<TableFilterViewModel>(editor.Filter);

        filter.AddCriterionCommand.Execute(null);
        var criterion = Assert.Single(filter.Criteria);

        Assert.False(editor.HasActiveFilter);
        Assert.Equal(3, editor.Rows.Count);

        criterion.SelectedColumn = Assert.Single(filter.Columns, column => column.Name == "ubWeight");
        criterion.SelectedOperator = Assert.Single(
            criterion.Operators,
            option => option.Operator == RowFilterOperator.GreaterThan);
        criterion.Operand = "5";

        Assert.True(editor.HasActiveFilter);
        Assert.Equal(new[] { "1" }, editor.Rows.Select(row => row.Key));

        editor.SearchText = "Knife";
        Assert.Empty(editor.Rows);

        editor.SearchText = string.Empty;
        Assert.Equal(new[] { "1" }, editor.Rows.Select(row => row.Key));

        filter.AddCriterionCommand.Execute(null);
        var second = filter.Criteria[1];
        second.SelectedColumn = Assert.Single(filter.Columns, column => column.Name == "szItemName");
        second.SelectedOperator = Assert.Single(
            second.Operators,
            option => option.Operator == RowFilterOperator.Equals);
        second.Operand = "Knife";
        Assert.Empty(editor.Rows);

        filter.SelectedLogicalOperator = Assert.Single(
            filter.LogicalOperators,
            option => option.Operator == RowFilterLogicalOperator.Or);
        Assert.Equal(new[] { "1", "2" }, editor.Rows.Select(row => row.Key));

        editor.ClearFilterCommand.Execute(null);

        Assert.False(editor.HasActiveFilter);
        Assert.Empty(filter.Criteria);
        Assert.Equal(3, editor.Rows.Count);
    }

    /// <summary>
    /// Filtering by the current cell value adds an equality or exclusion condition.
    /// </summary>
    [Fact]
    public async Task FilterByValueAndExclude_UseCurrentCell()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);
        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "1");

        Assert.False(editor.FilterByValueCommand.CanExecute(null));

        editor.CurrentCellField = "szItemName";
        Assert.True(editor.FilterByValueCommand.CanExecute(null));

        editor.FilterByValueCommand.Execute(null);

        Assert.True(editor.IsFilterVisible);
        Assert.Equal(new[] { "1" }, editor.Rows.Select(row => row.Key));
        Assert.Equal(
            RowFilterOperator.Equals,
            Assert.Single(editor.Filter!.Criteria).SelectedOperator!.Operator);

        editor.ClearFilterCommand.Execute(null);
        editor.ExcludeValueCommand.Execute(null);

        Assert.Equal(new[] { "0", "2" }, editor.Rows.Select(row => row.Key));
    }

    /// <summary>
    /// Bulk edit targets explicitly selected rows and falls back to the whole current view.
    /// </summary>
    [Fact]
    public async Task BulkEdit_UsesSelectedRowsAndFallsBackToCurrentView()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        dialog.BulkEditHandler = _ => null;
        editor.UpdateSelectedRows(Array.Empty<TableRowViewModel>());

        editor.BulkEditCommand.Execute(null);

        Assert.NotNull(dialog.LastBulkContext);
        Assert.Equal(3, dialog.LastBulkContext!.RecordKeys.Count);

        dialog.BulkEditHandler = context => new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            context.RecordKeys);
        editor.UpdateSelectedRows(editor.Rows.Where(row => row.Key is "0" or "2"));
        Assert.Equal(2, editor.SelectedRowCount);
        Assert.Contains("2", editor.SelectedRowCountText, StringComparison.Ordinal);

        editor.BulkEditCommand.Execute(null);

        Assert.Equal(new[] { "0", "2" }, dialog.LastBulkContext!.RecordKeys);
        Assert.Equal("7", Assert.Single(editor.Rows, row => row.Key == "0")["ubWeight"]);
        Assert.Equal("10", Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);
        Assert.Equal("7", Assert.Single(editor.Rows, row => row.Key == "2")["ubWeight"]);

        editor.UndoCommand.Execute(null);
        Assert.Equal("0", Assert.Single(editor.Rows, row => row.Key == "0")["ubWeight"]);
    }

    /// <summary>
    /// An empty Set operand clears the field value instead of being rejected by the dialog.
    /// </summary>
    [Fact]
    public async Task BulkEdit_EmptySet_ClearsValue()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        editor.UpdateSelectedRows(editor.Rows.Where(row => row.Key == "1"));
        dialog.BulkEditHandler = context => new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            string.Empty,
            IncludeMissingFields: false,
            context.RecordKeys);

        editor.BulkEditCommand.Execute(null);

        Assert.Equal(string.Empty, Assert.Single(editor.Rows, row => row.Key == "1")["ubWeight"]);
        Assert.True(editor.IsDirty);
    }

    /// <summary>
    /// Column visibility, width, order, sorting, and filter persist and can be reset.
    /// </summary>
    [Fact]
    public async Task Layout_PersistsRestoresAndResets()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var store = new FakeTableLayoutStore();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, layoutStore: store);

        Assert.Single(editor.Columns, column => column.Name == "ubWeight").IsVisible = false;
        editor.ApplyColumnOrder(new[] { "szItemName", "uiIndex", "ItemClass", "AttachmentClass" });
        editor.ApplyColumnWidths(new[] { ("szItemName", 200d) });
        editor.SortBy("ubWeight", ListSortDirection.Descending);
        var filter = Assert.IsType<TableFilterViewModel>(editor.Filter);
        filter.AddCriterionCommand.Execute(null);
        var criterion = Assert.Single(filter.Criteria);
        criterion.SelectedColumn = Assert.Single(filter.Columns, column => column.Name == "szItemName");
        criterion.SelectedOperator = Assert.Single(
            criterion.Operators,
            option => option.Operator == RowFilterOperator.Contains);
        criterion.Operand = "i";
        Assert.Equal(new[] { "2", "0" }, editor.Rows.Select(row => row.Key));

        await editor.FlushLayoutAsync();

        Assert.NotEmpty(store.Saved);
        var saved = store.Saved[^1];
        Assert.Equal("ubWeight", saved.SortField);
        Assert.Equal(TableSortDirection.Descending, saved.SortDirection);
        Assert.Single(saved.Filter.Criteria);
        Assert.Equal(200, saved.Columns.Single(column => column.Name == "szItemName").Width);

        var (restored, _, _, _, _) = await CreateEditorAsync(workspace, layoutStore: store);

        Assert.DoesNotContain(restored.VisibleColumns, column => column.Name == "ubWeight");
        Assert.Equal("szItemName", restored.VisibleColumns[0].Name);
        Assert.Equal("ubWeight", restored.SortField);
        Assert.Equal(ListSortDirection.Descending, restored.SortDirection);
        Assert.Equal(new[] { "2", "0" }, restored.Rows.Select(row => row.Key));
        Assert.Equal(200, Assert.Single(restored.Columns, column => column.Name == "szItemName").Width);

        await restored.ResetLayoutCommand.ExecuteAsync(null);

        Assert.Empty(store.Layouts);
        Assert.Empty(restored.SortField);
        Assert.False(restored.HasActiveFilter);
        Assert.Equal(5, restored.VisibleColumns.Count);
    }

    /// <summary>
    /// Duplicate keys are reported by validation.
    /// </summary>
    [Fact]
    public async Task Validate_ReportsDuplicateKeys()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, duplicateKeys: true);

        await editor.ValidateCommand.ExecuteAsync(null);

        Assert.True(editor.IsValidationVisible);
        Assert.Contains(editor.ValidationIssues, issue => issue.Code == "validation.duplicate_key");
        Assert.Contains("Validation issues", editor.StatusText, StringComparison.Ordinal);
    }

    /// <summary>
    /// The grid view loads its XAML, uses full-row selection for the two-way SelectedRow binding,
    /// and rebuilds columns when visibility changes.
    /// </summary>
    [Fact]
    public async Task View_LoadsXamlAndRebuildsColumns()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));
                Assert.Equal(DataGridSelectionUnit.FullRow, grid.SelectionUnit);
                Assert.Equal(DataGridSelectionMode.Extended, grid.SelectionMode);
                Assert.NotNull(grid.ContextMenu);
                Assert.Equal(editor.VisibleColumns.Count, grid.Columns.Count);

                Assert.Single(editor.Columns, column => column.Name == "ubWeight").IsVisible = false;
                Assert.Equal(editor.VisibleColumns.Count, grid.Columns.Count);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The grid view test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// A right click on a cell of an already selected row keeps the selection and records the clicked
    /// field, while a right click on another row selects only that row. Individual cell selection is
    /// never touched, because it throws for full-row selection.
    /// </summary>
    [Fact]
    public async Task View_RightClickSelectsRowWithoutTouchingCellSelection()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                view.Measure(new Size(1000, 700));
                view.Arrange(new Rect(0, 0, 1000, 700));
                view.UpdateLayout();
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));
                grid.UpdateLayout();
                var firstRow = Assert.IsType<DataGridRow>(
                    grid.ItemContainerGenerator.ContainerFromIndex(0));
                var firstCell = FindColumnCell(firstRow, "uiIndex");
                Assert.NotNull(VisualTreeHelper.GetParent(firstCell));

                grid.SelectedItem = firstRow.Item;

                InvokeRightClick(view, firstCell);

                Assert.Equal("uiIndex", editor.CurrentCellField);
                Assert.Same(firstRow.Item, grid.SelectedItem);
                Assert.Single(grid.SelectedItems);

                var otherRow = Assert.IsType<DataGridRow>(
                    grid.ItemContainerGenerator.ContainerFromIndex(2));
                var otherCell = FindColumnCell(otherRow, "uiIndex");

                InvokeRightClick(view, otherCell);

                Assert.Same(otherRow.Item, grid.SelectedItem);
                Assert.Same(editor.Rows[2], editor.SelectedRow);
                Assert.Single(grid.SelectedItems);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The right-click grid test did not finish in time.");
        Assert.Null(failure);
    }

    private static DataGridCell FindColumnCell(DataGridRow row, string fieldName) =>
        Assert.Single(
            FindVisualChildren<DataGridCell>(row),
            candidate => candidate.Column?.SortMemberPath == fieldName);

    private static void InvokeRightClick(TableView view, DataGridCell cell)
    {
        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right)
        {
            RoutedEvent = UIElement.PreviewMouseRightButtonDownEvent,
            Source = cell,
        };

        // A cell has no logical parent, so a synthetic routed event cannot tunnel through the visual
        // tree to the grid; invoke the real handler with the cell as the event source instead.
        var handler = typeof(TableView).GetMethod(
            "OnPreviewMouseRightButtonDown",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(object), typeof(MouseButtonEventArgs) },
            modifiers: null);
        Assert.NotNull(handler);
        handler.Invoke(view, new object[] { view, args });
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
        {
            var child = VisualTreeHelper.GetChild(parent, index);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var nested in FindVisualChildren<T>(child))
            {
                yield return nested;
            }
        }
    }

    /// <summary>
    /// In translation mode, translatable grid columns get a marker header and an untranslated cell style.
    /// </summary>
    [Fact]
    public async Task View_TranslationMode_MarksTranslatableColumns()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, language: "ru");
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));
                var column = Assert.Single(
                    grid.Columns.OfType<DataGridTextColumn>(),
                    candidate => candidate.SortMemberPath == "szItemName");
                Assert.NotNull(column.ElementStyle);
                var header = Assert.IsType<TextBlock>(column.Header);
                Assert.Contains("*", header.Text, StringComparison.Ordinal);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The grid translation test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> set, the editor loads the real Items table with schema columns.
    /// </summary>
    [Fact]
    public async Task Editor_RealItemsData_LoadsGrid()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var data13 = Directory.GetParent(tableData)?.FullName;
        Assert.NotNull(data13);
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = (await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            null,
            new FakeDialogService(),
            new TestItemImageService());

        await editor.InitializeAsync();

        Assert.True(editor.Rows.Count > 1000, $"Only {editor.Rows.Count} rows were loaded.");
        Assert.True(editor.Columns.Count > 100, $"Only {editor.Columns.Count} columns were discovered.");
        Assert.InRange(editor.VisibleColumns.Count, 1, 12);
        Assert.Equal("uiIndex", editor.VisibleColumns[0].Name);
        Assert.True(editor.VisibleColumns[0].IsKey);
        Assert.True(editor.HasGraphicFields);
        Assert.Contains(editor.DetailGroups, group => group.Key == "identity");
        Assert.Contains(editor.DetailGroups, group => group.Key == "attachments");
        Assert.Contains(editor.DetailGroups, group => group.Key == "stance");
        var fields = FlattenDetailFields(editor);
        Assert.True(Assert.Single(fields, field => field.Name == "AttachmentClass").CanEditBits);
        Assert.False(Assert.Single(fields, field => field.Name == "ubWeight").CanEditBits);
        Assert.True(Assert.Single(fields, field => field.Name == "STAND_MODIFIERS").IsReadOnly);
        Assert.False(editor.IsDirty);
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> set, real explosive and weapon tables show main-table picker names
    /// from AmmoTypes and AmmoStrings without marking the loaded documents dirty.
    /// </summary>
    [Fact]
    public async Task Editor_RealItemTables_ShowRelationPickers()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var data13 = Directory.GetParent(tableData)?.FullName;
        Assert.NotNull(data13);
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = (await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var explosives = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Explosives");
        var explosivesEditor = new TableEditorViewModel(
            session,
            explosives,
            null,
            new FakeDialogService(),
            new TestItemImageService());

        await explosivesEditor.InitializeAsync();

        var fragment = Assert.Single(explosivesEditor.Columns, column => column.Name == "ubFragType");
        Assert.True(fragment.HasRelationPicker, fragment.Relation?.ErrorText);
        Assert.Equal("Items.AmmoTypes", fragment.Relation!.TargetTable.Value);
        Assert.Equal("Ball", fragment.Relation.Display("0"));
        Assert.NotEmpty(fragment.Relation.Options);
        Assert.False(explosivesEditor.IsDirty);

        var weapons = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Weapons");
        var weaponsEditor = new TableEditorViewModel(
            session,
            weapons,
            null,
            new FakeDialogService(),
            new TestItemImageService());
        await weaponsEditor.InitializeAsync();

        var caliber = Assert.Single(weaponsEditor.Columns, column => column.Name == "ubCalibre");
        Assert.True(caliber.HasRelationPicker, caliber.Relation?.ErrorText);
        Assert.Equal("Items.AmmoStrings", caliber.Relation!.TargetTable.Value);
        Assert.Equal("9x19mm", caliber.Relation.Display("2"));
        Assert.False(weaponsEditor.IsDirty);
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> pointing at v4, the Russian overlay is merged into the real Items table.
    /// </summary>
    [Fact]
    public async Task Editor_RealItemsData_RussianOverlay_ShowsTranslations()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var data13 = Directory.GetParent(tableData)?.FullName;
        Assert.NotNull(data13);
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = (await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");
        if (!descriptor.Overlays.Any(overlay => overlay.Language.Code == "ru"))
        {
            return;
        }

        var dialog = new FakeDialogService();
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            "ru",
            dialog,
            new TestItemImageService());

        await editor.InitializeAsync();

        Assert.True(editor.IsTranslationMode);
        Assert.True(editor.Rows.Count > 1000, $"Only {editor.Rows.Count} rows were loaded.");
        var translated = editor.Rows.Count(row =>
            row.TranslationState.TryGetValue("szItemName", out var untranslated) && !untranslated);
        Assert.True(translated > 1000, $"Only {translated} rows have a translated short name.");

        editor.ShowTranslationReportCommand.Execute(null);
        Assert.NotNull(dialog.LastTranslationReport);
        Assert.NotEmpty(dialog.LastTranslationReport!.Issues);
    }

    /// <summary>
    /// The real Items table loads lookup names and bit labels, and a malformed lookup file degrades to
    /// a text editor without preventing the table from opening.
    /// </summary>
    [Fact]
    public async Task Editor_RealItemsData_LoadsLookupsAndBitLabels()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var data13 = Directory.GetParent(tableData)?.FullName;
        Assert.NotNull(data13);
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = (await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");
        var dialog = new FakeDialogService();
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            null,
            dialog,
            new TestItemImageService());

        await editor.InitializeAsync();

        Assert.True(editor.IsEditable);
        Assert.True(editor.Rows.Count > 1000, $"Only {editor.Rows.Count} rows were loaded.");
        var cursor = Assert.Single(editor.Columns, column => column.Name == "ubCursor");
        Assert.True(cursor.HasLookupCombo);
        Assert.NotEmpty(cursor.Lookup!.Entries);

        var attachmentPoint = Assert.Single(editor.Columns, column => column.Name == "AttachmentPoint");
        var attachmentLookup = await session.Lookups.GetEntriesAsync(
            TableName.Parse("Lookup.AttachmentPoint"));
        Assert.Equal(!attachmentLookup.IsSuccess, attachmentPoint.HasLookupError);

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        var classField = Assert.Single(FlattenDetailFields(editor), field => field.Name == "usItemClass");
        Assert.True(classField.CanEditBits);
        dialog.EditBitsResult = null;
        classField.EditBitsCommand.Execute(null);
        Assert.NotNull(dialog.LastBitLabels);
        Assert.NotEmpty(dialog.LastBitLabels!);
        Assert.Contains(dialog.LastBitLabels!, pair => pair.Key == 0);
    }

    /// <summary>
    /// The detail form groups fields, filters them, and edits values through the workspace.
    /// </summary>
    [Fact]
    public async Task DetailGroups_FilterAndEditSelectedRow()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);

        Assert.Equal(5, FlattenDetailFields(editor).Count);
        Assert.Contains("0", editor.DetailHeader, StringComparison.Ordinal);
        Assert.Contains(editor.DetailGroups, group => group.Key == "identity");
        Assert.Contains(editor.DetailGroups, group => group.Key == "inventory");
        Assert.Contains(editor.DetailGroups, group => group.Key == "attachments");
        Assert.True(Assert.Single(FlattenDetailFields(editor), field => field.Name == "uiIndex").IsReadOnly);
        Assert.False(Assert.Single(FlattenDetailFields(editor), field => field.Name == "AttachmentClass").IsReadOnly);
        var name = Assert.Single(FlattenDetailFields(editor), field => field.Name == "szItemName");
        Assert.False(name.IsReadOnly);

        name.Value = "Renamed";
        Assert.Equal("Renamed", name.Value);
        Assert.True(editor.IsDirty);

        editor.DetailFilter = "ubW";
        Assert.Equal("ubWeight", Assert.Single(FlattenDetailFields(editor)).Name);
        editor.DetailFilter = string.Empty;
        Assert.Equal(5, FlattenDetailFields(editor).Count);

        var weight = Assert.Single(FlattenDetailFields(editor), field => field.Name == "ubWeight");
        weight.Value = "abc";
        Assert.Equal("0", weight.Value);
    }

    /// <summary>
    /// The generic table detail panel resolves the same text limits as the item workspace: the
    /// description fields get the counter, the item name field gets only the input limit.
    /// </summary>
    [Fact]
    public async Task DetailForm_DescriptionsExposeCharacterCounterAndLimit()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeDescriptions: true);
        var fields = FlattenDetailFields(editor).ToDictionary(field => field.Name, StringComparer.Ordinal);

        Assert.Equal(400, fields["szItemDesc"].MaxTextLength);
        Assert.True(fields["szItemDesc"].ShowCharacterCounter);
        Assert.Equal(400, fields["szBRDesc"].MaxTextLength);
        Assert.True(fields["szBRDesc"].ShowCharacterCounter);
        Assert.Equal(80, fields["szItemName"].MaxTextLength);
        Assert.False(fields["szItemName"].ShowCharacterCounter);
        Assert.Equal(0, fields["ubWeight"].MaxTextLength);
        Assert.False(fields["ubWeight"].ShowCharacterCounter);
    }

    /// <summary>
    /// Selecting a row with graphic fields loads all three previews; a row without them clears the panels.
    /// </summary>
    [Fact]
    public async Task DetailImages_LoadThreeSizes()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var images = new TestItemImageService();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            images: images,
            includeGraphics: true);

        Assert.True(editor.HasGraphicFields);
        Assert.All(editor.DetailImages, panel => Assert.Contains("No item image", panel.InfoText));

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "1");
        await WaitUntilAsync(() => editor.DetailImages.All(panel => panel.Image is not null));

        Assert.Equal(3, images.Requests.Count);
        Assert.Contains(images.Requests, request => request.Size == ImageSize.Big);
        Assert.Contains(images.Requests, request => request.Size == ImageSize.Medium);
        Assert.Contains(images.Requests, request => request.Size == ImageSize.Small);
        Assert.All(images.Requests, request =>
        {
            Assert.Equal(2, request.GraphicType);
            Assert.Equal(35, request.GraphicIndex);
        });
        Assert.All(editor.DetailImages, panel => Assert.Equal("2×2", panel.InfoText));

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        await WaitUntilAsync(() => editor.DetailImages.All(
            panel => panel.Image is null && panel.InfoText.Contains("No item image", StringComparison.Ordinal)));
    }

    /// <summary>
    /// Editing <c>ubGraphicNum</c> in the standalone grid drops the cached row thumbnail and reloads the
    /// image with the new index.
    /// </summary>
    [Fact]
    public async Task GraphicEdit_InvalidatesGridThumbnail()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var images = new TestItemImageService();
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            images: images,
            includeGraphics: true);
        var row = Assert.Single(editor.Rows, candidate => candidate.Key == "1");
        row.RequestThumbnail();
        await WaitUntilAsync(() => row.Thumbnail is not null);
        Assert.Contains(images.Requests, request =>
            request.Size == ImageSize.Small && request.GraphicType == 2 && request.GraphicIndex == 35);

        row["ubGraphicNum"] = "36";

        await WaitUntilAsync(() => row.Thumbnail is not null && images.Requests.Any(request =>
            request.Size == ImageSize.Small && request.GraphicType == 2 && request.GraphicIndex == 36));
        Assert.Equal("36", row["ubGraphicNum"]);
        Assert.True(editor.IsDirty);
    }

    /// <summary>
    /// A grid thumbnail load that is still pending when the index changes must not overwrite the image
    /// of the new frame.
    /// </summary>
    [Fact]
    public async Task GraphicEdit_WhileGridThumbnailLoadPending_KeepsNewestImage()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var images = new TestItemImageService
        {
            BitmapFactory = request => TestItemImageService.CreateBitmap(request.GraphicIndex + 1),
        };
        var (editor, _, _, _, _) = await CreateEditorAsync(
            workspace,
            images: images,
            includeGraphics: true);
        var row = Assert.Single(editor.Rows, candidate => candidate.Key == "1");
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        images.BeforeImageReturn = request =>
            request.Size == ImageSize.Small && request.GraphicIndex == 35
                ? gate.Task
                : Task.CompletedTask;
        row.RequestThumbnail();
        await WaitUntilAsync(() => images.Requests.Any(request =>
            request.Size == ImageSize.Small && request.GraphicIndex == 35));
        Assert.Null(row.Thumbnail);

        row["ubGraphicNum"] = "36";
        await WaitUntilAsync(() => row.Thumbnail?.PixelWidth == 37);

        gate.SetResult(true);
        await Task.Delay(100);

        Assert.Equal(37, row.Thumbnail?.PixelWidth);
    }

    /// <summary>
    /// The detail bit editor is offered only for flag masks and writes the composed value back.
    /// </summary>
    [Fact]
    public async Task DetailBits_AppliesComposedValueOnlyForFlags()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        dialog.EditBitsResult = 5;

        var weight = Assert.Single(FlattenDetailFields(editor), field => field.Name == "ubWeight");
        Assert.False(weight.CanEditBits);
        var attachment = Assert.Single(FlattenDetailFields(editor), field => field.Name == "AttachmentClass");
        Assert.True(attachment.CanEditBits);

        attachment.EditBitsCommand.Execute(null);

        Assert.Equal("5", attachment.Value);
        Assert.Equal("5", editor.Workspace!.BaseDocument.Records[0].GetRawValue("AttachmentClass"));
        Assert.True(editor.IsDirty);
    }

    /// <summary>
    /// Lookup columns show names, keep unknown identifiers editable as raw values, and fall back to
    /// text when the lookup file cannot be loaded.
    /// </summary>
    [Fact]
    public async Task Lookups_LoadNamesUnknownValuesAndFallback()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeLookups: true);

        var cursor = Assert.Single(editor.Columns, column => column.Name == "ubCursor");
        Assert.True(cursor.HasLookupCombo);
        Assert.NotNull(cursor.Lookup);
        Assert.Equal("Hand", cursor.Lookup!.GetName("2"));
        Assert.Equal("99 (unknown)", cursor.Lookup.Display("99"));
        Assert.Contains(cursor.Lookup.Options, option => option.IdText == "99" && option.IsUnknown);
        Assert.Contains(cursor.Lookup.Options, option => option.IdText == "2" && !option.IsUnknown);

        var inseparable = Assert.Single(editor.Columns, column => column.Name == "Inseparable");
        Assert.True(inseparable.HasLookupError);
        Assert.False(inseparable.HasLookupCombo);
        Assert.Contains("Lookup unavailable", inseparable.HeaderTooltip, StringComparison.Ordinal);

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        var cursorField = Assert.Single(FlattenDetailFields(editor), field => field.Name == "ubCursor");
        Assert.True(cursorField.IsLookup);
        Assert.Contains(cursorField.LookupOptions, option => option.IdText == "99" && option.IsUnknown);
        cursorField.Value = "3";
        Assert.Equal("3", editor.Workspace!.BaseDocument.Records[0].GetRawValue("ubCursor"));

        var inseparableField = Assert.Single(FlattenDetailFields(editor), field => field.Name == "Inseparable");
        Assert.True(inseparableField.HasLookupError);
        Assert.True(inseparableField.IsPlainText);
        Assert.Contains("Lookup unavailable", inseparableField.FieldTooltip, StringComparison.Ordinal);
    }

    /// <summary>
    /// Relation columns show target names, keep unknown identifiers raw, fall back to plain text when
    /// the target cannot be loaded, and allow navigation to the referenced record.
    /// </summary>
    [Fact]
    public async Task Relations_ResolveNamesUnknownValuesAndNavigation()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeRelations: true);

        var column = Assert.Single(editor.Columns, candidate => candidate.Name == "DiscardedLauncherItem");
        Assert.True(column.HasRelationPicker, column.Relation?.ErrorText);
        Assert.False(column.HasRelationError);
        Assert.Equal("Glock 17", column.Relation!.Display("1"));
        Assert.Equal("99 (unknown)", column.Relation.Display("99"));
        Assert.Contains(column.Relation.Options, option => option.Value == "1" && !option.IsUnknown);
        Assert.Contains(column.Relation.Options, option => option.Value == "99" && option.IsUnknown);
        Assert.DoesNotContain(column.Relation.Options, option => option.Value == "0");
        Assert.Equal("Knife", column.Relation.Display("2"));

        var missingTarget = Assert.Single(editor.Columns, candidate => candidate.Name == "FoodType");
        Assert.True(missingTarget.HasRelationError);
        Assert.False(missingTarget.HasRelationPicker);
        Assert.Contains(
            "Relation target unavailable",
            missingTarget.HeaderTooltip,
            StringComparison.Ordinal);

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "1");
        var field = Assert.Single(FlattenDetailFields(editor), candidate => candidate.Name == "DiscardedLauncherItem");
        Assert.True(field.IsRelation);
        Assert.True(field.CanGoToTarget);
        Assert.Equal("Glock 17", field.Relation!.Display(field.Value));

        field.Value = "2";
        Assert.Equal("2", editor.Workspace!.BaseDocument.Records[1].GetRawValue("DiscardedLauncherItem"));
        Assert.True(editor.IsDirty);

        field.GoToTargetCommand.Execute(null);
        Assert.Equal("2", editor.SelectedRow!.Key);

        var unknownRow = Assert.Single(editor.Rows, row => row.Key == "2");
        editor.SelectedRow = unknownRow;
        var unknownField = Assert.Single(
            FlattenDetailFields(editor),
            candidate => candidate.Name == "DiscardedLauncherItem");
        Assert.Equal("99", unknownField.Value);
        Assert.False(unknownField.CanGoToTarget);
    }

    /// <summary>
    /// Relation validation reports source values that are absent from the resolved target table.
    /// </summary>
    [Fact]
    public async Task Validate_ReportsUnknownRelationValues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeRelations: true);

        await editor.ValidateCommand.ExecuteAsync(null);

        var issue = Assert.Single(
            editor.ValidationIssues,
            candidate => candidate.Code == "validation.lookup_missing"
                && candidate.Text.Contains("DiscardedLauncherItem", StringComparison.Ordinal));
        Assert.Contains("99", issue.Text, StringComparison.Ordinal);
    }

    /// <summary>
    /// The grid builds a relation picker template and keeps a plain-text fallback with a warning when
    /// the target table is missing.
    /// </summary>
    [Fact]
    public async Task View_BuildsRelationPickerAndFallback()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeRelations: true);
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));

                var relation = Assert.Single(
                    grid.Columns,
                    column => column.SortMemberPath == "DiscardedLauncherItem");
                var template = Assert.IsType<DataGridTemplateColumn>(relation);
                Assert.NotNull(template.CellEditingTemplate);

                var fallback = Assert.Single(
                    grid.Columns,
                    column => column.SortMemberPath == "FoodType");
                Assert.IsType<DataGridTextColumn>(fallback);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The relation grid test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// Boolean fields use checkboxes and preserve the sparse distinction between an absent field and
    /// an explicit zero.
    /// </summary>
    [Fact]
    public async Task BooleanCheckbox_TogglesSparseValues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeBooleans: true);

        var column = Assert.Single(editor.Columns, candidate => candidate.Name == "AllowClimbing");
        Assert.True(column.IsBoolean);
        Assert.False(column.HasLookupCombo);

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        var field = Assert.Single(FlattenDetailFields(editor), candidate => candidate.Name == "AllowClimbing");
        Assert.True(field.IsBoolean);
        Assert.True(field.BooleanValue);
        field.BooleanValue = false;
        Assert.Equal("0", field.Value);
        Assert.Equal("0", editor.Workspace!.BaseDocument.Records[0].GetRawValue("AllowClimbing"));

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "2");
        var absent = Assert.Single(FlattenDetailFields(editor), candidate => candidate.Name == "AllowClimbing");
        Assert.False(absent.BooleanValue);
        Assert.Equal("Not set", absent.BooleanTooltip);
        absent.BooleanValue = true;
        Assert.Equal("1", absent.Value);
        Assert.Equal("1", editor.Workspace!.BaseDocument.Records[2].GetRawValue("AllowClimbing"));
    }

    /// <summary>
    /// Flag masks receive bit labels from the lookup file mapped to the field, including exact names
    /// for combination values.
    /// </summary>
    [Fact]
    public async Task LookupMasks_ProvideNamedBits()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace, includeLookups: true);
        dialog.EditBitsResult = 5;

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        var field = Assert.Single(FlattenDetailFields(editor), candidate => candidate.Name == "AttachmentClass");
        Assert.True(field.CanEditBits);

        field.EditBitsCommand.Execute(null);

        Assert.NotNull(dialog.LastBitLabels);
        Assert.Equal(3, dialog.LastBitLabels!.Count);
        Assert.Equal("A", dialog.LastBitLabels[0]);
        Assert.Equal("B", dialog.LastBitLabels[1]);
        Assert.Equal("C", dialog.LastBitLabels[2]);
        Assert.Equal("A", dialog.LastBitValueName);
        Assert.Equal("5", field.Value);
    }

    /// <summary>
    /// Lookup identifiers above 32 bits produce bit labels instead of being dropped, and the edited
    /// value keeps its full 64-bit precision.
    /// </summary>
    [Fact]
    public async Task LookupMasks_IncludeBitsAbove32Bits()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(
            workspace,
            includeLookups: true,
            includeHighBits: true);
        dialog.EditBitsResult = (1UL << 33) + 1;

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "0");
        var field = Assert.Single(FlattenDetailFields(editor), candidate => candidate.Name == "AttachmentClass");

        field.EditBitsCommand.Execute(null);

        Assert.NotNull(dialog.LastBitLabels);
        Assert.Equal("High32", dialog.LastBitLabels![32]);
        Assert.Equal("High33", dialog.LastBitLabels[33]);
        Assert.Equal(((1UL << 33) + 1).ToString(CultureInfo.InvariantCulture), field.Value);
    }

    /// <summary>
    /// Nested structures are read-only in the detail form and cannot be overwritten as scalar text.
    /// </summary>
    [Fact]
    public async Task DetailNestedField_IsReadOnly()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace, includeNested: true);
        editor.SelectedRow = Assert.Single(editor.Rows, row => row.Key == "1");

        var nested = Assert.Single(FlattenDetailFields(editor), field => field.Name == "MODIFIERS");
        Assert.True(nested.IsReadOnly);
        Assert.False(nested.CanEditBits);
        Assert.Equal("nested", nested.GroupKey);

        nested.Value = "replaced";
        Assert.False(editor.IsDirty);
        Assert.Equal("1", editor.Workspace!.BaseDocument.Records[1].GetRawValue("MODIFIERS"));
    }

    /// <summary>
    /// The bit editor model composes unsigned values and its window loads XAML.
    /// </summary>
    [Fact]
    public void BitEditor_ComposesValueAndLoadsXaml()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = new BitEditorViewModel("bits", 5);
        Assert.Equal(64, viewModel.Bits.Count);
        Assert.True(viewModel.Bits[0].IsSet);
        Assert.False(viewModel.Bits[1].IsSet);
        Assert.True(viewModel.Bits[2].IsSet);
        Assert.Equal(5UL, viewModel.ComposeValue());
        viewModel.Bits[1].IsSet = true;
        Assert.Equal(7UL, viewModel.ComposeValue());

        var highBit = new BitEditorViewModel("bits", 1UL << 63);
        Assert.True(highBit.Bits[63].IsSet);
        Assert.All(highBit.Bits.Take(63), bit => Assert.False(bit.IsSet));
        Assert.Equal(1UL << 63, highBit.ComposeValue());
        highBit.Bits[0].IsSet = true;
        Assert.Equal((1UL << 63) + 1, highBit.ComposeValue());

        var maxValue = new BitEditorViewModel("bits", ulong.MaxValue);
        Assert.All(maxValue.Bits, bit => Assert.True(bit.IsSet));
        Assert.Equal(ulong.MaxValue, maxValue.ComposeValue());

        var lookupBits = new BitEditorViewModel(
            "bits",
            1UL << 33,
            new Dictionary<int, string> { [32] = "Bit32", [33] = "Bit33" });
        Assert.Equal("Bit33 (33)", lookupBits.Bits[33].DisplayText);
        Assert.Equal("Bit32 (32)", lookupBits.Bits[32].DisplayText);

        var named = new BitEditorViewModel(
            "bits",
            5,
            new Dictionary<int, string> { [0] = "Gun", [2] = "Knife" },
            "Gun + Knife");
        Assert.Equal("Value: 5 (Gun + Knife)", named.ValueSummary);
        Assert.Equal("Gun (0)", named.Bits[0].DisplayText);
        Assert.Equal("1", named.Bits[1].DisplayText);
        Assert.Equal("Knife (2)", named.Bits[2].DisplayText);

        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var window = new BitEditorWindow(viewModel);
                Assert.Same(viewModel, window.DataContext);
                window.Close();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The bit editor window test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// The language selector lists the base table and every configured language, marking missing overlays.
    /// </summary>
    [Fact]
    public async Task LanguageOptions_ListExistingAndCreatableLanguages()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var withOverlay = new TestWorkspace();
        var (existingEditor, _, _, _, _) = await CreateEditorAsync(withOverlay);
        Assert.False(existingEditor.IsTranslationMode);
        Assert.Equal(8, existingEditor.LanguageOptions.Count);
        var existingRussian = Assert.Single(existingEditor.LanguageOptions, option => option.Code == "ru");
        Assert.True(existingRussian.HasOverlay);
        Assert.Equal("Russian", existingRussian.DisplayText);
        Assert.Null(existingEditor.LanguageOptions[0].Language);
        Assert.Contains(existingEditor.LanguageOptions, option => option.Code == "de" && !option.HasOverlay);

        using var withoutOverlay = new TestWorkspace();
        var (missingEditor, _, _, _, _) = await CreateEditorAsync(withoutOverlay, includeOverlay: false);
        var missingRussian = Assert.Single(missingEditor.LanguageOptions, option => option.Code == "ru");
        Assert.False(missingRussian.HasOverlay);
        Assert.Contains("create overlay", missingRussian.DisplayText, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Switching to an existing overlay reloads merged values and exposes untranslated markers.
    /// </summary>
    [Fact]
    public async Task SwitchLanguage_ReloadsMergedValuesAndMarkers()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _, _) = await CreateEditorAsync(workspace);
        var russian = Assert.Single(editor.LanguageOptions, option => option.Code == "ru");

        editor.SelectedLanguageOption = russian;
        await WaitUntilAsync(() => editor.IsTranslationMode && !editor.IsBusy);

        Assert.Contains("Russian", editor.LanguageText, StringComparison.Ordinal);
        Assert.Equal("Глок 17", Assert.Single(editor.Rows, row => row.Key == "1")["szItemName"]);
        var untranslated = Assert.Single(editor.Rows, row => row.Key == "0");
        Assert.True(untranslated.TranslationState["szItemName"]);

        editor.SelectedRow = untranslated;
        var detailName = Assert.Single(FlattenDetailFields(editor), field => field.Name == "szItemName");
        Assert.True(detailName.IsUntranslated);

        editor.SelectedLanguageOption = editor.LanguageOptions[0];
        await WaitUntilAsync(() => !editor.IsTranslationMode && !editor.IsBusy);
        Assert.All(editor.Rows, row => Assert.Empty(row.TranslationState));
    }

    /// <summary>
    /// Selecting a language without an overlay creates an empty overlay, then saves only translations.
    /// </summary>
    [Fact]
    public async Task CreateOverlay_WritesEmptyFileAndKeepsBaseUntouched()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, _, _) = await CreateEditorAsync(workspace, includeOverlay: false);
        var overlayPath = Path.Combine(Path.GetDirectoryName(descriptor.FilePath)!, "Russian.Items.xml");
        var baseBefore = await File.ReadAllBytesAsync(descriptor.FilePath);
        Assert.False(File.Exists(overlayPath));

        var russian = Assert.Single(editor.LanguageOptions, option => option.Code == "ru");
        Assert.False(russian.HasOverlay);
        editor.SelectedLanguageOption = russian;
        await WaitUntilAsync(() => File.Exists(overlayPath) && editor.IsTranslationMode && !editor.IsBusy);

        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(descriptor.FilePath));
        Assert.Contains(editor.Descriptor.Overlays, overlay => overlay.Language.Code == "ru");
        Assert.True(Assert.Single(editor.LanguageOptions, option => option.Code == "ru").HasOverlay);
        var created = System.Xml.Linq.XDocument.Load(overlayPath);
        Assert.Equal("ITEMLIST", created.Root!.Name.LocalName);
        Assert.Empty(created.Root.Elements());

        var translated = Assert.Single(editor.Rows, row => row.Key == "1");
        translated["szItemName"] = "Глок 18";
        await editor.SaveCommand.ExecuteAsync(null);

        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(descriptor.FilePath));
        var saved = System.Xml.Linq.XDocument.Load(overlayPath);
        var item = Assert.Single(saved.Root!.Elements("ITEM"));
        Assert.Equal("1", item.Element("uiIndex")?.Value);
        Assert.Equal("Глок 18", item.Element("szItemName")?.Value);
        Assert.Equal(2, item.Elements().Count());
    }

    /// <summary>
    /// Switching language with unsaved changes asks first and keeps the edits when the user declines.
    /// </summary>
    [Fact]
    public async Task SwitchLanguage_WithDirtyChanges_RespectsConfirmation()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace);
        var row = Assert.Single(editor.Rows, candidate => candidate.Key == "1");
        row["szItemName"] = "Changed";
        Assert.True(editor.IsDirty);

        dialog.ConfirmResult = false;
        editor.SelectedLanguageOption = Assert.Single(editor.LanguageOptions, option => option.Code == "ru");

        Assert.False(editor.IsTranslationMode);
        Assert.Equal("Changed", row["szItemName"]);
        Assert.Null(editor.SelectedLanguageOption?.Language);
    }

    /// <summary>
    /// The translation report command passes the analyzer output to the dialog.
    /// </summary>
    [Fact]
    public async Task ShowTranslationReport_WithOverlay_InvokesDialog()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, dialog, _) = await CreateEditorAsync(workspace, language: "ru");

        editor.ShowTranslationReportCommand.Execute(null);

        Assert.NotNull(dialog.LastTranslationReport);
        Assert.NotEmpty(dialog.LastTranslationReport!.Issues);
        Assert.Contains("Items.Items", dialog.LastTranslationReportTitle, StringComparison.Ordinal);
        Assert.Contains("Russian", dialog.LastTranslationReportTitle, StringComparison.Ordinal);

        var viewModel = new TranslationReportViewModel(dialog.LastTranslationReport, "report");
        Assert.NotEmpty(viewModel.Groups);
        Assert.Contains(viewModel.Issues, issue => issue.KindLabel == "Missing translation");
    }

    /// <summary>
    /// The translation report window loads its XAML and binds the report view model.
    /// </summary>
    [Fact]
    public void TranslationReportWindow_LoadsXaml()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var report = new TranslationReport(new[]
        {
            new TranslationIssue(
                TranslationIssueKind.MissingTranslation,
                "1",
                null,
                "Base record '1' has no overlay translation."),
        });
        var viewModel = new TranslationReportViewModel(report, "Translations: Items.Items (Russian)");
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var window = new TranslationReportWindow(viewModel);
                Assert.Same(viewModel, window.DataContext);
                window.Close();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(
            thread.Join(TimeSpan.FromSeconds(10)),
            "The translation report window test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// A confirmed overwrite replaces an externally changed file and rebases the editor baseline, so a
    /// later regular save succeeds.
    /// </summary>
    [Fact]
    public async Task Save_ExternalChangeOverwrite_ReplacesFileAndRebasesBaseline()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, data13, dialog, _) = await CreateEditorAsync(workspace);
        var path = Path.Combine(data13, "TableData", "Items", "Items.xml");
        editor.Rows[0]["szItemName"] = "Changed";
        await File.AppendAllTextAsync(path, " ");
        dialog.ExternalChangeResult = ExternalChangeChoice.Overwrite;

        var saved = await editor.SaveWithRecoveryAsync();

        Assert.True(saved);
        Assert.False(editor.IsDirty);
        Assert.Contains("changed on disk", dialog.LastExternalChangeMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(
            "Changed",
            System.Xml.Linq.XDocument.Load(path).Root!
                .Elements("ITEM").First().Element("szItemName")!.Value);

        editor.Rows[0]["szItemName"] = "Changed again";
        Assert.True(await editor.SaveWithRecoveryAsync());
        Assert.Equal(
            "Changed again",
            System.Xml.Linq.XDocument.Load(path).Root!
                .Elements("ITEM").First().Element("szItemName")!.Value);
    }

    /// <summary>
    /// Choosing reload on an external change discards unsaved edits and shows the file contents.
    /// </summary>
    [Fact]
    public async Task Save_ExternalChangeReload_DiscardsEditorChanges()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, data13, dialog, _) = await CreateEditorAsync(workspace);
        var path = Path.Combine(data13, "TableData", "Items", "Items.xml");
        editor.Rows[0]["szItemName"] = "Changed";
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>External</szItemName><ubWeight>0</ubWeight></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName><ubWeight>10</ubWeight></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><szItemName>Knife</szItemName><ubWeight>2</ubWeight></ITEM>"
            + "</ITEMLIST>");
        dialog.ExternalChangeResult = ExternalChangeChoice.Reload;

        var saved = await editor.SaveWithRecoveryAsync();

        Assert.True(saved);
        Assert.False(editor.IsDirty);
        Assert.Equal("External", editor.Rows[0]["szItemName"]);
        Assert.Equal("External", System.Xml.Linq.XDocument.Load(path).Root!
            .Elements("ITEM").First().Element("szItemName")!.Value);
    }

    private static List<DetailFieldViewModel> FlattenDetailFields(TableEditorViewModel editor) =>
        editor.DetailGroups.SelectMany(static group => group.Fields).ToList();

    private static async Task WaitUntilAsync(Func<bool> predicate)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (predicate())
            {
                return;
            }

            await Task.Delay(25);
        }

        Assert.Fail("The asynchronous editor operation did not complete in time.");
    }

    private static async Task<(
        TableEditorViewModel Editor,
        TableDescriptor Descriptor,
        string Data13,
        FakeDialogService Dialog,
        TestItemImageService Images)> CreateEditorAsync(
        TestWorkspace workspace,
        string? language = null,
        bool duplicateKeys = false,
        TestItemImageService? images = null,
        bool includeGraphics = false,
        bool includeNested = false,
        bool includeOverlay = true,
        bool includeLookups = false,
        bool includeBooleans = false,
        bool includeRelations = false,
        bool includeHighBits = false,
        bool includeDescriptions = false,
        bool emptyTable = false,
        FakeTableLayoutStore? layoutStore = null,
        TimeSpan? statusAutoResetDelay = null)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var secondKey = duplicateKeys ? "1" : "2";
        var graphics = includeGraphics
            ? "<ubGraphicType>2</ubGraphicType><ubGraphicNum>35</ubGraphicNum>"
            : string.Empty;
        var nested = includeNested
            ? "<MODIFIERS><FlatBase>1</FlatBase></MODIFIERS>"
            : string.Empty;
        var lookups = includeLookups
            ? "<ubCursor>2</ubCursor><Inseparable>1</Inseparable><AttachmentSystem>2</AttachmentSystem>"
            : string.Empty;
        var boolean = includeBooleans ? "<AllowClimbing>1</AllowClimbing>" : string.Empty;
        var unknownLookup = includeLookups ? "<ubCursor>99</ubCursor>" : string.Empty;
        var relation = includeRelations ? "<usItemClass>2</usItemClass><DiscardedLauncherItem>1</DiscardedLauncherItem>" : string.Empty;
        var missingRelationTarget = includeRelations ? "<FoodType>1</FoodType>" : string.Empty;
        var unknownRelation = includeRelations ? "<DiscardedLauncherItem>99</DiscardedLauncherItem>" : string.Empty;
        // Relation pickers display szLongItemName; it is added only for relation tests so the other
        // fixtures keep their established column counts.
        var longNames = includeRelations
            ? "<szLongItemName>Nothing</szLongItemName>"
            : string.Empty;
        var descriptions = includeDescriptions
            ? "<szItemDesc>Nothing at all.</szItemDesc><szBRDesc>Nothing at all (BR).</szBRDesc>"
            : string.Empty;
        var tableBody = emptyTable
            ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST></ITEMLIST>"
            : "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
              + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName>" + longNames + descriptions + "<ubWeight>0</ubWeight>"
              + "<AttachmentClass>1</AttachmentClass>" + lookups + boolean + missingRelationTarget + "</ITEM>"
              + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName>"
              + (includeRelations ? "<szLongItemName>Glock 17</szLongItemName>" : string.Empty)
              + "<ubWeight>10</ubWeight>"
              + graphics + nested + unknownLookup + relation + "</ITEM>"
              + $"<ITEM><uiIndex>{secondKey}</uiIndex><szItemName>Knife</szItemName>"
              + (includeRelations ? "<szLongItemName>Knife</szLongItemName>" : string.Empty)
              + "<ubWeight>2</ubWeight><ItemClass>5</ItemClass>" + unknownRelation + "</ITEM>"
              + "</ITEMLIST>";
        await File.WriteAllTextAsync(Path.Combine(itemsDirectory, "Items.xml"), tableBody);
        if (includeOverlay)
        {
            await File.WriteAllTextAsync(
                Path.Combine(itemsDirectory, "Russian.Items.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
                + "<ITEM><uiIndex>1</uiIndex><szItemName>Глок 17</szItemName></ITEM>"
                + "</ITEMLIST>");
        }

        if (includeLookups)
        {
            var lookupDirectory = Path.Combine(tableData, "Lookup");
            Directory.CreateDirectory(lookupDirectory);
            await File.WriteAllTextAsync(
                Path.Combine(lookupDirectory, "Cursor.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
                + "<Cursor><id>1</id><name>Default</name></Cursor>"
                + "<Cursor><id>2</id><name>Hand</name></Cursor>"
                + "<Cursor><id>3</id><name>Use</name></Cursor>"
                + "</JA2Data>");
            await File.WriteAllTextAsync(
                Path.Combine(lookupDirectory, "AttachmentSystem.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
                + "<AttachmentSystem><id>1</id><name>None</name></AttachmentSystem>"
                + "<AttachmentSystem><id>2</id><name>NAS</name></AttachmentSystem>"
                + "</JA2Data>");
            await File.WriteAllTextAsync(
                Path.Combine(lookupDirectory, "AttachmentClass.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
                + "<AttachmentClass><id>1</id><name>A</name></AttachmentClass>"
                + "<AttachmentClass><id>2</id><name>B</name></AttachmentClass>"
                + "<AttachmentClass><id>3</id><name>AB</name></AttachmentClass>"
                + "<AttachmentClass><id>4</id><name>C</name></AttachmentClass>"
                + (includeHighBits
                    ? "<AttachmentClass><id>4294967296</id><name>High32</name></AttachmentClass>"
                      + "<AttachmentClass><id>8589934592</id><name>High33</name></AttachmentClass>"
                    : string.Empty)
                + "</JA2Data>");
            // Separability.xml is intentionally absent so the editor must fall back to text.
        }

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = (await new DataSetSessionFactory(provider, catalog, repository)
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");
        var dialog = new FakeDialogService();
        images ??= new TestItemImageService();
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            language,
            dialog,
            images,
            layoutStore,
            statusAutoResetDelay);
        await editor.InitializeAsync();
        return (editor, descriptor, data13, dialog, images);
    }

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; } = true;

        public UnsavedChangesChoice UnsavedChangesResult { get; set; } = UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ExternalChangeResult { get; set; } = ExternalChangeChoice.Cancel;

        public string? LastExternalChangeMessage { get; private set; }

        public ulong? EditBitsResult { get; set; }

        public IReadOnlyDictionary<int, string>? LastBitLabels { get; private set; }

        public string? LastBitValueName { get; private set; }

        public TranslationReport? LastTranslationReport { get; private set; }

        public string? LastTranslationReportTitle { get; private set; }

        public Func<BulkEditContext, BulkEditRequest?>? BulkEditHandler { get; set; }

        public BulkEditContext? LastBulkContext { get; private set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

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
            string? valueName = null)
        {
            LastBitLabels = labels;
            LastBitValueName = valueName;
            return EditBitsResult;
        }

        public void ShowTranslationReport(TranslationReport report, string title)
        {
            LastTranslationReport = report;
            LastTranslationReportTitle = title;
        }

        public ItemCreationRequest? CreateItemResult { get; set; }

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => CreateItemResult;

        public BulkEditRequest? EditBulk(BulkEditContext context)
        {
            LastBulkContext = context;
            return BulkEditHandler?.Invoke(context);
        }
    }
}
