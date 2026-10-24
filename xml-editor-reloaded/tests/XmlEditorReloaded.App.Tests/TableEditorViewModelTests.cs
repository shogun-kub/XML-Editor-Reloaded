using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;

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
        var (editor, _, _, _) = await CreateEditorAsync(workspace);

        Assert.True(editor.IsEditable);
        Assert.False(editor.IsReadOnly);
        Assert.Equal("Items.Items", editor.DisplayName);
        Assert.Equal("Base", editor.LanguageText);
        Assert.Equal(3, editor.Rows.Count);
        Assert.Equal(4, editor.Columns.Count);
        Assert.Equal(4, editor.VisibleColumns.Count);
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
        var (editor, descriptor, data13, _) = await CreateEditorAsync(workspace);
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
    /// With a language selected, translated cells show overlay values and edits stay in the overlay.
    /// </summary>
    [Fact]
    public async Task OverlayLanguage_ShowsAndWritesTranslations()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, _) = await CreateEditorAsync(workspace, language: "ru");

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
    /// Add, duplicate, and delete operate on the selected row and respect confirmation.
    /// </summary>
    [Fact]
    public async Task RowCommands_AddDuplicateAndDelete()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, descriptor, _, dialog) = await CreateEditorAsync(workspace);

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
        var (editor, _, _, _) = await CreateEditorAsync(workspace);

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
        var (editor, _, _, _) = await CreateEditorAsync(workspace);

        var weight = Assert.Single(editor.Columns, column => column.Name == "ubWeight");
        weight.IsVisible = false;
        Assert.Equal(3, editor.VisibleColumns.Count);
        Assert.DoesNotContain(editor.VisibleColumns, column => column.Name == "ubWeight");

        editor.ShowAllColumnsCommand.Execute(null);
        Assert.Equal(4, editor.VisibleColumns.Count);

        editor.HideAllColumnsCommand.Execute(null);
        Assert.Empty(editor.VisibleColumns);

        editor.ShowDefaultColumnsCommand.Execute(null);
        Assert.Equal(4, editor.VisibleColumns.Count);

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
        var (editor, _, _, _) = await CreateEditorAsync(workspace, duplicateKeys: true);

        await editor.ValidateCommand.ExecuteAsync(null);

        Assert.True(editor.IsValidationVisible);
        Assert.Contains(editor.ValidationIssues, issue => issue.Code == "validation.duplicate_key");
        Assert.Contains("Validation issues", editor.StatusText, StringComparison.Ordinal);
    }

    /// <summary>
    /// The grid view loads its XAML and rebuilds columns when visibility changes.
    /// </summary>
    [Fact]
    public async Task View_LoadsXamlAndRebuildsColumns()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _, _, _) = await CreateEditorAsync(workspace);
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var view = new TableView { DataContext = editor };
                var grid = Assert.IsType<DataGrid>(view.FindName("RowsGrid"));
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
        var editor = new TableEditorViewModel(session, descriptor, null, new FakeDialogService());

        await editor.InitializeAsync();

        Assert.True(editor.Rows.Count > 1000, $"Only {editor.Rows.Count} rows were loaded.");
        Assert.True(editor.Columns.Count > 100, $"Only {editor.Columns.Count} columns were discovered.");
        Assert.InRange(editor.VisibleColumns.Count, 1, 12);
        Assert.Equal("uiIndex", editor.VisibleColumns[0].Name);
        Assert.True(editor.VisibleColumns[0].IsKey);
        Assert.False(editor.IsDirty);
    }

    private static async Task<(TableEditorViewModel Editor, TableDescriptor Descriptor, string Data13, FakeDialogService Dialog)>
        CreateEditorAsync(
            TestWorkspace workspace,
            string? language = null,
            bool duplicateKeys = false)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var secondKey = duplicateKeys ? "1" : "2";
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName><ubWeight>0</ubWeight></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName><ubWeight>10</ubWeight></ITEM>"
            + $"<ITEM><uiIndex>{secondKey}</uiIndex><szItemName>Knife</szItemName>"
            + "<ubWeight>2</ubWeight><ItemClass>5</ItemClass></ITEM>"
            + "</ITEMLIST>");
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Russian.Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Глок 17</szItemName></ITEM>"
            + "</ITEMLIST>");

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
        var editor = new TableEditorViewModel(session, descriptor, language, dialog);
        await editor.InitializeAsync();
        return (editor, descriptor, data13, dialog);
    }

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; } = true;

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => ConfirmResult;
    }
}
