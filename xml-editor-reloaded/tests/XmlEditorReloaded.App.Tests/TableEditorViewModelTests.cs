using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Imaging;
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

        editor.ShowAllColumnsCommand.Execute(null);
        Assert.Equal(5, editor.VisibleColumns.Count);

        editor.HideAllColumnsCommand.Execute(null);
        Assert.Empty(editor.VisibleColumns);

        editor.ShowDefaultColumnsCommand.Execute(null);
        Assert.Equal(5, editor.VisibleColumns.Count);

        editor.ColumnSearchText = "ubW";
        Assert.Equal("ubWeight", Assert.Single(editor.Columns).Name);
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
        Assert.Equal(32, viewModel.Bits.Count);
        Assert.True(viewModel.Bits[0].IsSet);
        Assert.False(viewModel.Bits[1].IsSet);
        Assert.True(viewModel.Bits[2].IsSet);
        Assert.Equal(5u, viewModel.ComposeValue());
        viewModel.Bits[1].IsSet = true;
        Assert.Equal(7u, viewModel.ComposeValue());

        var maxValue = new BitEditorViewModel("bits", uint.MaxValue);
        Assert.All(maxValue.Bits, bit => Assert.True(bit.IsSet));
        Assert.Equal(uint.MaxValue, maxValue.ComposeValue());

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
        Assert.Equal(2, existingEditor.LanguageOptions.Count);
        var existingRussian = Assert.Single(existingEditor.LanguageOptions, option => option.Code == "ru");
        Assert.True(existingRussian.HasOverlay);
        Assert.Equal("Russian", existingRussian.DisplayText);
        Assert.Null(existingEditor.LanguageOptions[0].Language);

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
        bool includeRelations = false)
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
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName>" + longNames + "<ubWeight>0</ubWeight>"
            + "<AttachmentClass>1</AttachmentClass>" + lookups + boolean + missingRelationTarget + "</ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName>"
            + (includeRelations ? "<szLongItemName>Glock 17</szLongItemName>" : string.Empty)
            + "<ubWeight>10</ubWeight>"
            + graphics + nested + unknownLookup + relation + "</ITEM>"
            + $"<ITEM><uiIndex>{secondKey}</uiIndex><szItemName>Knife</szItemName>"
            + (includeRelations ? "<szLongItemName>Knife</szLongItemName>" : string.Empty)
            + "<ubWeight>2</ubWeight><ItemClass>5</ItemClass>" + unknownRelation + "</ITEM>"
            + "</ITEMLIST>");
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
        var editor = new TableEditorViewModel(session, descriptor, language, dialog, images);
        await editor.InitializeAsync();
        return (editor, descriptor, data13, dialog, images);
    }

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; } = true;

        public uint? EditBitsResult { get; set; }

        public IReadOnlyDictionary<int, string>? LastBitLabels { get; private set; }

        public string? LastBitValueName { get; private set; }

        public TranslationReport? LastTranslationReport { get; private set; }

        public string? LastTranslationReportTitle { get; private set; }

        public Func<BulkEditContext, BulkEditRequest?>? BulkEditHandler { get; set; }

        public BulkEditContext? LastBulkContext { get; private set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

        public uint? EditBits(
            string title,
            uint value,
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
