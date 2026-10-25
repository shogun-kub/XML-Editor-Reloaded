using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Copy;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Grid behaviour for a table whose natural key consists of several fields.
/// </summary>
public class TableEditorCompoundKeyTests
{
    /// <summary>
    /// Compound key columns stay editable, duplicate is disabled, a new row can be completed in place,
    /// and deleting a row removes exactly that row.
    /// </summary>
    [Fact]
    public async Task CompoundTable_EditsKeysAddsRowsAndDeletesExactRow()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var (editor, _) = await CreateEditorAsync(workspace);

        Assert.Equal(3, editor.Rows.Count);
        var keyColumns = editor.Columns.Where(static column => column.IsKey).ToArray();
        Assert.Equal(2, keyColumns.Length);
        Assert.All(keyColumns, static column => Assert.True(column.IsCompoundKey));
        Assert.All(keyColumns, static column => Assert.False(column.IsReadOnly));
        Assert.False(editor.DuplicateRowCommand.CanExecute(null));

        editor.SelectedRow = Assert.Single(editor.Rows, row => row.KeyDisplay == "10 | 2");
        editor.DeleteRowCommand.Execute(null);

        Assert.Equal(2, editor.Rows.Count);
        Assert.Contains(editor.Rows, row => row.KeyDisplay == "10 | 1");
        Assert.DoesNotContain(editor.Rows, row => row.KeyDisplay == "10 | 2");

        editor.UndoCommand.Execute(null);

        Assert.Equal(3, editor.Rows.Count);
        Assert.Contains(editor.Rows, row => row.KeyDisplay == "10 | 2");

        editor.AddRowCommand.Execute(null);

        Assert.Equal(4, editor.Rows.Count);
        var added = editor.SelectedRow;
        Assert.NotNull(added);
        Assert.Same(added, editor.Rows[^1]);
        var attachmentIndexField = Assert.Single(
            editor.DetailGroups.SelectMany(static group => group.Fields),
            field => field.Name == "attachmentIndex");
        Assert.False(attachmentIndexField.IsReadOnly);

        added!["attachmentIndex"] = "12";
        added["itemIndex"] = "3";

        Assert.Equal("12 | 3", added.KeyDisplay);
        Assert.True(editor.IsDirty);
        Assert.Equal("2:12|1:3", added.Key);
    }

    private static async Task<(TableEditorViewModel Editor, TableDescriptor Descriptor)> CreateEditorAsync(
        TestWorkspace workspace)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Attachments.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>\r\n"
            + "\t<ATTACHMENT>\r\n\t\t<attachmentIndex>10</attachmentIndex>\r\n"
            + "\t\t<itemIndex>1</itemIndex>\r\n\t\t<APCost>5</APCost>\r\n\t</ATTACHMENT>\r\n"
            + "\t<ATTACHMENT>\r\n\t\t<attachmentIndex>11</attachmentIndex>\r\n"
            + "\t\t<itemIndex>1</itemIndex>\r\n\t\t<APCost>6</APCost>\r\n\t</ATTACHMENT>\r\n"
            + "\t<ATTACHMENT>\r\n\t\t<attachmentIndex>10</attachmentIndex>\r\n"
            + "\t\t<itemIndex>2</itemIndex>\r\n\t\t<APCost>7</APCost>\r\n\t</ATTACHMENT>\r\n"
            + "</ATTACHMENTLIST>\r\n");

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var session = (await new DataSetSessionFactory(provider, catalog, repository)
            .OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(
            tables.Data!,
            table => table.TableName.Value == "Items.Attachments");
        var editor = new TableEditorViewModel(
            session,
            descriptor,
            languageCode: null,
            new FakeDialogService(),
            new TestItemImageService());
        await editor.InitializeAsync();
        return (editor, descriptor);
    }

    private sealed class FakeDialogService : IDialogService
    {
        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => true;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ResolveExternalChange(string message, string title) =>
            ExternalChangeChoice.Cancel;

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
