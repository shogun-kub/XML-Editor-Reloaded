using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Data.Settings;
using XmlEditorReloaded.Xml.Copy;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the WPF shell view model, its table list, and its editor tabs.
/// </summary>
public class MainViewModelTests
{
    /// <summary>
    /// Initialization loads a real catalog and selecting a table produces a record count.
    /// </summary>
    [Fact]
    public async Task InitializeAndSelect_LoadsTablesAndRecordCount()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);

        await viewModel.InitializeAsync();

        var item = Assert.Single(viewModel.Tables);
        Assert.Equal("Items.Items", item.TableName);
        Assert.True(item.Descriptor.IsMapped);

        viewModel.SelectedTable = item;
        await WaitUntilAsync(() => !string.IsNullOrEmpty(viewModel.RecordCountText));

        Assert.Contains("1", viewModel.RecordCountText, StringComparison.Ordinal);
        Assert.Equal("Loaded", viewModel.DetailsText);
        Assert.False(viewModel.HasEditors);
    }

    /// <summary>
    /// A failed data-set session leaves the shell empty and reports the invalid directory.
    /// </summary>
    [Fact]
    public async Task Initialize_MissingDataDirectory_ShowsInvalidDirectory()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var missing = workspace.PathOf("missing");
        var settingsStore = new JsonSettingsStore(workspace.PathOf("settings.json"));
        Assert.True((await settingsStore.SaveAsync(new AppSettings(
            new[] { missing },
            missing,
            "ru",
            "en",
            new[] { missing }))).IsSuccess);
        var dialog = new FakeDialogService();
        var viewModel = new MainViewModel(
            new FailingSessionFactory(),
            new TableEditorFactory(dialog, new TestItemImageService(), new FakeTableLayoutStore()),
            settingsStore,
            dialog,
            new FakeImageViewerService(),
            new FakeVersionCompareService(),
            new FakeTableCopyDialogService(),
            new FakeSettingsDialogService(),
            new FakeTableLayoutStore());

        await viewModel.InitializeAsync();

        Assert.Empty(viewModel.Tables);
        Assert.Equal(
            "The selected folder does not contain TableData or Data-1.13\\TableData.",
            viewModel.StatusText);
    }

    /// <summary>
    /// Selecting a table without metadata loads it in document-driven mode and surfaces the diagnostic.
    /// </summary>
    [Fact]
    public async Task SelectUnmappedTable_ShowsMetadataUnmappedDiagnostic()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        Directory.CreateDirectory(Path.Combine(data13, "TableData", "Custom"));
        await File.WriteAllTextAsync(
            Path.Combine(data13, "TableData", "Custom", "Notes.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><NOTES>"
            + "<NOTE><id>1</id><text>Hello</text></NOTE>"
            + "</NOTES>");
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);

        await viewModel.InitializeAsync();
        var table = Assert.Single(viewModel.Tables);
        Assert.False(table.Descriptor.IsMapped);

        viewModel.SelectedTable = table;
        await WaitUntilAsync(() => viewModel.Diagnostics.Count > 0);

        Assert.Contains(viewModel.Diagnostics, line => line.StartsWith("metadata.unmapped:", StringComparison.Ordinal));
        Assert.Equal("Loaded", viewModel.DetailsText);
        Assert.False(viewModel.IsBusy);
    }

    /// <summary>
    /// An unexpected session failure is reported through the status text instead of escaping the command.
    /// </summary>
    [Fact]
    public async Task Refresh_SessionFactoryThrows_SetsErrorStatus()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        Directory.CreateDirectory(Path.Combine(data13, "TableData"));
        var dialog = new FakeDialogService();
        var viewModel = new MainViewModel(
            new ThrowingSessionFactory(),
            new TableEditorFactory(dialog, new TestItemImageService(), new FakeTableLayoutStore()),
            new JsonSettingsStore(workspace.PathOf("settings.json")),
            dialog,
            new FakeImageViewerService(),
            new FakeVersionCompareService(),
            new FakeTableCopyDialogService(),
            new FakeSettingsDialogService(),
            new FakeTableLayoutStore())
        {
            DataDirectory = data13,
        };

        await viewModel.RefreshCommand.ExecuteAsync(null);

        Assert.Equal("Error: boom", viewModel.StatusText);
        Assert.False(viewModel.IsBusy);
    }

    /// <summary>
    /// Opening a generic table creates an editor tab, cells edit through the workspace, invalid values
    /// are rejected, saving writes the file, and closing a clean tab removes it.
    /// </summary>
    [Fact]
    public async Task OpenTable_EditsCellAndSaves()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);

        await viewModel.OpenTableCommand.ExecuteAsync(null);

        var editor = Assert.Single(viewModel.Editors.OfType<TableEditorViewModel>());
        Assert.Same(editor, viewModel.SelectedEditor);
        Assert.True(viewModel.HasEditors);
        Assert.Single(editor.Rows);
        Assert.Equal("Custom.Notes", editor.DisplayName);
        var row = editor.Rows[0];
        Assert.Equal("Hello", row["text"]);

        row["text"] = "Hello again";
        Assert.True(editor.IsDirty);
        Assert.EndsWith("*", editor.DisplayName, StringComparison.Ordinal);

        row["id"] = "not a number";
        Assert.Equal("1", row["id"]);
        Assert.Contains("not a valid value", editor.StatusText, StringComparison.Ordinal);

        await editor.SaveCommand.ExecuteAsync(null);

        Assert.False(editor.IsDirty);
        Assert.Equal("Custom.Notes", editor.DisplayName);
        var saved = XDocument.Load(Path.Combine(data13, "TableData", "Custom", "Notes.xml"));
        Assert.Equal("Hello again", saved.Root!.Element("NOTE")!.Element("text")!.Value);

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);
        Assert.Empty(viewModel.Editors);
        Assert.False(viewModel.HasEditors);
    }

    /// <summary>
    /// Opening the Items table creates the item master-detail workspace instead of the generic grid.
    /// </summary>
    [Fact]
    public async Task OpenItems_OpensItemWorkspace()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace, includeWeight: true);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);

        await viewModel.OpenTableCommand.ExecuteAsync(null);

        var editor = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        Assert.Same(editor, viewModel.SelectedEditor);
        Assert.Equal("Items", editor.Title);
        Assert.Single(editor.Items);
        Assert.NotNull(editor.SelectedItem);
        Assert.Equal("Nothing", editor.SelectedItem!.Name);
        Assert.Contains(editor.ItemGroups, group => group.Key == "identity");
    }

    /// <summary>
    /// Following a relation from the detail panel opens the target table tab and selects the
    /// referenced record.
    /// </summary>
    [Fact]
    public async Task FollowRelation_OpensTargetTableAndSelectsRecord()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName>"
            + "<TestAttachment>7</TestAttachment></ITEM>"
            + "</ITEMLIST>");
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Attachments.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>"
            + "<ATTACHMENT><attachmentIndex>7</attachmentIndex><itemIndex>1</itemIndex>"
            + "<APCost>20</APCost></ATTACHMENT>"
            + "</ATTACHMENTLIST>");
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        await using (var connection = new SqliteConnection(
                         $"Data Source={workspace.PathOf("metadata.sqlite")}"))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO TableRelations
                    (SourceTableId, SourceField, TargetTableId, TargetValueField, TargetDisplayField,
                     Kind, RemapByText, Ordinal)
                SELECT s.Id, 'TestAttachment', t.Id, 'attachmentIndex', 'itemIndex',
                       'Reference', 0, 0
                FROM Tables s, Tables t
                WHERE s.XmlTable = 'Items.Items' AND t.XmlTable = 'Items.Attachments';
                """;
            Assert.Equal(1, await command.ExecuteNonQueryAsync());
        }

        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables, item => item.TableName == "Items.Items");
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var itemsEditor = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        itemsEditor.SelectRecord("1");
        var field = itemsEditor.ItemGroups
            .SelectMany(group => group.Fields)
            .Single(candidate => candidate.Name == "TestAttachment");
        Assert.True(field.IsRelation, $"{field.FieldTooltip} / {itemsEditor.StatusText}");
        Assert.Equal("7", field.Value);
        Assert.True(
            field.Relation!.TryGetTargetValue("7", out var targetValue),
            $"target={field.Relation.Relation.TargetTable} options={field.Relation.Options.Count} "
            + $"error={field.Relation.ErrorText}");
        Assert.Equal("7", targetValue);
        Assert.True(field.CanGoToTarget, field.FieldTooltip);

        field.GoToTargetCommand.Execute(null);

        await WaitUntilAsync(() => viewModel.Editors.Count == 2);
        var target = Assert.Single(
            viewModel.Editors.OfType<TableEditorViewModel>(),
            editor => editor.TableName.Value == "Items.Attachments");
        await WaitUntilAsync(() => target.SelectedRow?.KeyDisplay == "7 | 1");
        Assert.Same(target, viewModel.SelectedEditor);
    }

    /// <summary>
    /// Switching an editor's language persists the choice for future table tabs.
    /// </summary>
    [Fact]
    public async Task SwitchLanguage_PersistsActiveLanguageCode()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace, includeOverlay: true);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);

        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        Assert.True(editor.IsTranslationMode);

        editor.SelectedLanguageOption = editor.LanguageOptions[0];
        await WaitUntilAsync(() => !editor.IsTranslationMode && !editor.IsBusy);

        AppSettings? persisted = null;
        for (var attempt = 0; attempt < 100; attempt++)
        {
            persisted = (await settingsStore.LoadAsync()).Data;
            if (persisted?.ActiveLanguageCode is null)
            {
                break;
            }

            await Task.Delay(25);
        }

        Assert.NotNull(persisted);
        Assert.Null(persisted!.ActiveLanguageCode);
    }

    /// <summary>
    /// Reopening a table after creating an overlay re-reads the catalog and uses the new overlay.
    /// </summary>
    [Fact]
    public async Task ReopenTable_AfterOverlayCreation_UsesNewOverlay()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);

        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        Assert.False(editor.IsTranslationMode);
        var russian = Assert.Single(editor.LanguageOptions, option => option.Code == "ru");
        Assert.False(russian.HasOverlay);

        editor.SelectedLanguageOption = russian;
        await WaitUntilAsync(() => editor.IsTranslationMode && !editor.IsBusy);
        var overlayPath = Path.Combine(data13, "TableData", "Items", "Russian.Items.xml");
        Assert.True(File.Exists(overlayPath));

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);
        await viewModel.OpenTableCommand.ExecuteAsync(null);

        var reopened = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        Assert.True(reopened.IsTranslationMode);
        Assert.True(Assert.Single(reopened.LanguageOptions, option => option.Code == "ru").HasOverlay);
    }

    /// <summary>
    /// The compare command opens the comparison window with the active data directory.
    /// </summary>
    [Fact]
    public async Task CompareVersions_OpensWithActiveDataDirectory()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var compare = new FakeVersionCompareService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog, compare);
        await viewModel.InitializeAsync();

        viewModel.CompareVersionsCommand.Execute(null);

        Assert.Equal(data13, compare.LastDataDirectory);
    }

    /// <summary>
    /// Switching the data-directory selector reloads the table list but keeps open tabs from both data
    /// sets, and each tab reports its own label.
    /// </summary>
    [Fact]
    public async Task SwitchDataDirectory_KeepsTabsAcrossDataSets()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateNotesData(workspace, "v4", "First");
        var second = CreateNotesData(workspace, "v5", "Second");
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, first, second);
        var dialog = new FakeDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();

        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var firstEditor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        Assert.Equal("First", firstEditor.Rows[0]["text"]);
        Assert.Equal("v4", firstEditor.DatasetLabel);

        var secondItem = Assert.Single(
            viewModel.DataDirectories,
            item => string.Equals(item.Path, second, StringComparison.OrdinalIgnoreCase));
        viewModel.SelectedDataDirectory = secondItem;
        await WaitUntilAsync(() =>
            string.Equals(viewModel.DataDirectory, second, StringComparison.OrdinalIgnoreCase)
            && !viewModel.IsBusy);

        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);

        var secondEditor = Assert.Single(
            viewModel.Editors.OfType<TableEditorViewModel>(),
            editor => string.Equals(
                editor.TableDataDirectory,
                Path.Combine(second, "TableData"),
                StringComparison.OrdinalIgnoreCase));
        Assert.Equal("Second", secondEditor.Rows[0]["text"]);
        Assert.Equal("v5", secondEditor.DatasetLabel);
        Assert.Equal(2, viewModel.Editors.Count);
        Assert.Same(secondEditor, viewModel.SelectedEditor);
    }

    /// <summary>
    /// The copy command passes the selected tab's data set, table, and record key to the dialog.
    /// </summary>
    [Fact]
    public async Task CopyData_PrefillsSelectedEditorTableAndKey()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateItemsData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var copyDialog = new FakeTableCopyDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog, copyDialog: copyDialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);

        var editor = Assert.IsType<ItemWorkspaceViewModel>(Assert.Single(viewModel.Editors));
        editor.SelectRecord("0");

        await viewModel.CopyDataCommand.ExecuteAsync(null);

        var request = Assert.IsType<TableCopyDialogRequest>(copyDialog.LastRequest);
        Assert.Equal(Path.Combine(data13, "TableData"), request.InitialSourceDirectory);
        Assert.Equal("Items.Items", request.InitialTableName);
        Assert.Equal(new[] { "0" }, request.InitialRecordKeys);
        Assert.Contains(data13, request.DataDirectories);
    }

    /// <summary>
    /// A committed copy reloads every clean tab of the target data set from disk.
    /// </summary>
    [Fact]
    public async Task CopyData_ReloadsCleanTargetTabs()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var source = CreateNotesData(workspace, "v4", "First");
        var target = CreateNotesData(workspace, "v5", "Second");
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, source, target);
        var dialog = new FakeDialogService();
        var copyDialog = new FakeTableCopyDialogService();
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog, copyDialog: copyDialog);
        await viewModel.InitializeAsync();

        var targetItem = Assert.Single(
            viewModel.DataDirectories,
            item => string.Equals(item.Path, target, StringComparison.OrdinalIgnoreCase));
        viewModel.SelectedDataDirectory = targetItem;
        await WaitUntilAsync(() =>
            string.Equals(viewModel.DataDirectory, target, StringComparison.OrdinalIgnoreCase)
            && !viewModel.IsBusy);
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var targetEditor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        Assert.Equal("Second", targetEditor.Rows[0]["text"]);

        var notesPath = Path.Combine(target, "TableData", "Custom", "Notes.xml");
        await File.WriteAllTextAsync(
            notesPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><NOTES>"
            + "<NOTE><id>1</id><text>External</text></NOTE>"
            + "</NOTES>");

        var sourceItem = Assert.Single(
            viewModel.DataDirectories,
            item => string.Equals(item.Path, source, StringComparison.OrdinalIgnoreCase));
        viewModel.SelectedDataDirectory = sourceItem;
        await WaitUntilAsync(() =>
            string.Equals(viewModel.DataDirectory, source, StringComparison.OrdinalIgnoreCase)
            && !viewModel.IsBusy);

        var tableName = TableName.Parse("Custom.Notes");
        copyDialog.Result = new TableCopyDialogOutcome(
            Path.Combine(target, "TableData"),
            new TableCopyResult(
                tableName,
                new[] { new TableCopyAppliedTable(tableName, 1, 0, 0) },
                new[] { notesPath },
                Array.Empty<string>()));

        await viewModel.CopyDataCommand.ExecuteAsync(null);

        await WaitUntilAsync(() => targetEditor.Rows[0]["text"] == "External");
        Assert.Contains("Copied: 1 added", viewModel.StatusText, StringComparison.Ordinal);
        Assert.False(targetEditor.IsDirty);
    }

    /// <summary>
    /// Confirming settings switches the active data set without closing open tabs and persists the new
    /// directory list and interface language.
    /// </summary>
    [Fact]
    public async Task OpenSettings_AppliesActiveDirectoryAndPersistsSettings()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateNotesData(workspace, "v4", "First");
        var second = CreateNotesData(workspace, "v5", "Second");
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, first, second);
        var dialog = new FakeDialogService();
        var settingsDialog = new FakeSettingsDialogService
        {
            Result = new SettingsResult(new[] { first, second }, second, "ru", ResetLayouts: false),
        };
        var viewModel = CreateShell(
            provider,
            catalog,
            repository,
            settingsStore,
            dialog,
            settingsDialog: settingsDialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.Single(viewModel.Editors);

        await viewModel.OpenSettingsCommand.ExecuteAsync(null);

        Assert.Equal(first, settingsDialog.LastRequest!.DataDirectories[0]);
        Assert.Equal(second, viewModel.DataDirectory);
        Assert.Equal(new[] { first, second }, settingsDialog.LastRequest.DataDirectories);
        Assert.Single(viewModel.Editors);
        Assert.Same(editor, viewModel.Editors[0]);
        Assert.StartsWith(first, editor.TableDataDirectory, StringComparison.OrdinalIgnoreCase);

        var persisted = (await settingsStore.LoadAsync()).Data!;
        Assert.Equal(new[] { first, second }, persisted.DataDirectories);
        Assert.Equal(second, persisted.ActiveDataDirectory);
        Assert.Equal("ru", persisted.UiLanguageCode);
    }

    /// <summary>
    /// Confirming the settings dialog with the layout reset removes every stored layout.
    /// </summary>
    [Fact]
    public async Task OpenSettings_ResetLayouts_ClearsStoredLayouts()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService();
        var layoutStore = new FakeTableLayoutStore();
        var tableData = Path.Combine(data13, "TableData");
        Assert.True((await layoutStore.SaveAsync(tableData, "Custom.Notes", new TableLayout(
            Array.Empty<ColumnLayout>(),
            "id",
            TableSortDirection.Descending,
            null))).IsSuccess);
        var settingsDialog = new FakeSettingsDialogService
        {
            Result = new SettingsResult(new[] { data13 }, data13, "en", ResetLayouts: true),
        };
        var viewModel = CreateShell(
            provider,
            catalog,
            repository,
            settingsStore,
            dialog,
            settingsDialog: settingsDialog,
            layoutStore: layoutStore);
        await viewModel.InitializeAsync();

        await viewModel.OpenSettingsCommand.ExecuteAsync(null);

        Assert.Empty(layoutStore.Layouts);
        Assert.Equal(1, layoutStore.ResetCount);
        Assert.Contains("Settings saved.", viewModel.StatusText, StringComparison.Ordinal);
    }

    /// <summary>
    /// Cancelling the unsaved-changes dialog keeps a dirty tab open.
    /// </summary>
    [Fact]
    public async Task CloseDirtyTab_CancelKeepsTab()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService { UnsavedChangesResult = UnsavedChangesChoice.Cancel };
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);

        Assert.Same(editor, Assert.Single(viewModel.Editors));
        Assert.True(editor.IsDirty);
        Assert.Contains("unsaved changes", dialog.LastUnsavedChangesMessage, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Discarding changes closes a dirty tab without writing the file.
    /// </summary>
    [Fact]
    public async Task CloseDirtyTab_DiscardClosesWithoutSaving()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService { UnsavedChangesResult = UnsavedChangesChoice.Discard };
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";
        var path = Path.Combine(data13, "TableData", "Custom", "Notes.xml");

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);

        Assert.Empty(viewModel.Editors);
        Assert.Contains("Hello", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
        Assert.DoesNotContain("Changed", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// Choosing Save in the unsaved-changes dialog writes the file before the tab is removed.
    /// </summary>
    [Fact]
    public async Task CloseDirtyTab_SaveWritesAndCloses()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService { UnsavedChangesResult = UnsavedChangesChoice.Save };
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";
        var path = Path.Combine(data13, "TableData", "Custom", "Notes.xml");

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);

        Assert.Empty(viewModel.Editors);
        Assert.Contains("Changed", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// A failed save keeps the tab open and preserves its unsaved changes.
    /// </summary>
    [Fact]
    public async Task CloseDirtyTab_SaveFailureKeepsTabOpen()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService
        {
            UnsavedChangesResult = UnsavedChangesChoice.Save,
            ExternalChangeResult = ExternalChangeChoice.Cancel,
        };
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";
        await File.AppendAllTextAsync(
            Path.Combine(data13, "TableData", "Custom", "Notes.xml"),
            " ");

        await viewModel.CloseEditorCommand.ExecuteAsync(editor);

        Assert.Same(editor, Assert.Single(viewModel.Editors));
        Assert.True(editor.IsDirty);
    }

    /// <summary>
    /// Closing the application saves dirty tabs, flushes pending layouts, and reports that closing may
    /// proceed.
    /// </summary>
    [Fact]
    public async Task RequestClose_SavesDirtyTabsAndFlushesLayouts()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService { UnsavedChangesResult = UnsavedChangesChoice.Save };
        var layoutStore = new FakeTableLayoutStore();
        var viewModel = CreateShell(
            provider,
            catalog,
            repository,
            settingsStore,
            dialog,
            layoutStore: layoutStore);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";
        editor.SortBy("id", System.ComponentModel.ListSortDirection.Ascending);

        var canClose = await viewModel.RequestCloseAsync();

        Assert.True(canClose);
        Assert.False(editor.IsDirty);
        Assert.NotEmpty(layoutStore.Saved);
        Assert.Contains("Changed", await File.ReadAllTextAsync(
            Path.Combine(data13, "TableData", "Custom", "Notes.xml")), StringComparison.Ordinal);
    }

    /// <summary>
    /// Cancelling the unsaved-changes dialog during application close aborts the close.
    /// </summary>
    [Fact]
    public async Task RequestClose_CancelledByUser_ReturnsFalse()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateNotesData(workspace);
        var (provider, catalog, repository, settingsStore) = await CreateServicesAsync(workspace, data13);
        var dialog = new FakeDialogService { UnsavedChangesResult = UnsavedChangesChoice.Cancel };
        var viewModel = CreateShell(provider, catalog, repository, settingsStore, dialog);
        await viewModel.InitializeAsync();
        viewModel.SelectedTable = Assert.Single(viewModel.Tables);
        await viewModel.OpenTableCommand.ExecuteAsync(null);
        var editor = Assert.IsType<TableEditorViewModel>(Assert.Single(viewModel.Editors));
        editor.Rows[0]["text"] = "Changed";

        Assert.False(await viewModel.RequestCloseAsync());
        Assert.True(editor.IsDirty);
        Assert.Single(viewModel.Editors);
    }

    private static MainViewModel CreateShell(
        SqliteMetadataProvider provider,
        FileSystemTableCatalog catalog,
        TableRepository repository,
        JsonSettingsStore settingsStore,
        FakeDialogService dialog,
        FakeVersionCompareService? versionCompare = null,
        FakeTableCopyDialogService? copyDialog = null,
        FakeSettingsDialogService? settingsDialog = null,
        FakeTableLayoutStore? layoutStore = null) =>
        new(
            new DataSetSessionFactory(provider, catalog, repository),
            new TableEditorFactory(
                dialog,
                new TestItemImageService(),
                layoutStore ?? new FakeTableLayoutStore()),
            settingsStore,
            dialog,
            new FakeImageViewerService(),
            versionCompare ?? new FakeVersionCompareService(),
            copyDialog ?? new FakeTableCopyDialogService(),
            settingsDialog ?? new FakeSettingsDialogService(),
            layoutStore ?? new FakeTableLayoutStore());

    private static string CreateNotesData(TestWorkspace workspace) =>
        CreateNotesData(workspace, gameFolder: null, text: "Hello");

    private static string CreateNotesData(
        TestWorkspace workspace,
        string? gameFolder,
        string text)
    {
        var data13 = gameFolder is null
            ? workspace.PathOf("Data-1.13")
            : workspace.PathOf(Path.Combine(gameFolder, "Data-1.13"));
        var directory = Path.Combine(data13, "TableData", "Custom");
        Directory.CreateDirectory(directory);
        File.WriteAllText(
            Path.Combine(directory, "Notes.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><NOTES>"
            + $"<NOTE><id>1</id><text>{text}</text></NOTE>"
            + "</NOTES>");
        return data13;
    }

    private static string CreateItemsData(
        TestWorkspace workspace,
        bool includeWeight = false,
        bool includeOverlay = false)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var itemsDirectory = Path.Combine(data13, "TableData", "Items");
        Directory.CreateDirectory(itemsDirectory);
        var weight = includeWeight ? "<ubWeight>10</ubWeight>" : string.Empty;
        File.WriteAllText(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName>" + weight + "</ITEM>"
            + "</ITEMLIST>");
        if (includeOverlay)
        {
            File.WriteAllText(
                Path.Combine(itemsDirectory, "Russian.Items.xml"),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
                + "<ITEM><uiIndex>0</uiIndex><szItemName>Ничто</szItemName></ITEM>"
                + "</ITEMLIST>");
        }

        return data13;
    }

    private static async Task<(
        SqliteMetadataProvider Provider,
        FileSystemTableCatalog Catalog,
        TableRepository Repository,
        JsonSettingsStore SettingsStore)> CreateServicesAsync(
        TestWorkspace workspace,
        string data13,
        params string[] additionalDirectories)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var settingsStore = new JsonSettingsStore(workspace.PathOf("settings.json"));
        var directories = new[] { data13 }.Concat(additionalDirectories).ToArray();
        var settings = new AppSettings(
            directories,
            data13,
            "ru",
            "en",
            directories);
        Assert.True((await settingsStore.SaveAsync(settings)).IsSuccess);
        return (provider, catalog, repository, settingsStore);
    }

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

        Assert.Fail("The asynchronous shell operation did not complete in time.");
    }

    private sealed class FakeDialogService : IDialogService
    {
        public bool ConfirmResult { get; set; } = true;

        public UnsavedChangesChoice UnsavedChangesResult { get; set; } = UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ExternalChangeResult { get; set; } = ExternalChangeChoice.Cancel;

        public uint? EditBitsResult { get; set; }

        public TranslationReport? LastTranslationReport { get; private set; }

        public string? LastUnsavedChangesMessage { get; private set; }

        public string? LastExternalChangeMessage { get; private set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public string? SaveFileResult { get; set; }

        public string? PickSaveFile(string title, string filter, string defaultFileName) => SaveFileResult;

        public bool Confirm(string message, string title) => ConfirmResult;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title)
        {
            LastUnsavedChangesMessage = message;
            return UnsavedChangesResult;
        }

        public ExternalChangeChoice ResolveExternalChange(string message, string title)
        {
            LastExternalChangeMessage = message;
            return ExternalChangeResult;
        }

        public uint? EditBits(
            string title,
            uint value,
            IReadOnlyDictionary<int, string>? labels = null,
            string? valueName = null) => EditBitsResult;

        public void ShowTranslationReport(TranslationReport report, string title) =>
            LastTranslationReport = report;

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItemResult { get; set; }

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => CreateItemResult;
    }

    private sealed class FakeSettingsDialogService : ISettingsDialogService
    {
        public SettingsRequest? LastRequest { get; private set; }

        public SettingsResult? Result { get; set; }

        public SettingsResult? Show(SettingsRequest request)
        {
            LastRequest = request;
            return Result;
        }
    }

    private sealed class FakeImageViewerService : IImageViewerService
    {
        public string? LastDataDirectory { get; private set; }

        public void Show(string? dataDirectory) => LastDataDirectory = dataDirectory;
    }

    private sealed class FakeVersionCompareService : IVersionCompareService
    {
        public string? LastDataDirectory { get; private set; }

        public void Show(string? initialDirectory) => LastDataDirectory = initialDirectory;
    }

    private sealed class FakeTableCopyDialogService : ITableCopyDialogService
    {
        public TableCopyDialogRequest? LastRequest { get; private set; }

        public TableCopyDialogOutcome? Result { get; set; }

        public Task<TableCopyDialogOutcome?> ShowAsync(
            TableCopyDialogRequest request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            return Task.FromResult(Result);
        }
    }

    private sealed class FailingSessionFactory : IDataSetSessionFactory
    {
        public Task<OperationResult<IDataSetSession>> OpenAsync(
            string? selectedPath,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IDataSetSession>.Failure(
                ErrorCodes.IoFileNotFound,
                "The selected path does not contain a TableData directory."));
    }

    private sealed class ThrowingSessionFactory : IDataSetSessionFactory
    {
        public Task<OperationResult<IDataSetSession>> OpenAsync(
            string? selectedPath,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("boom");
    }
}
