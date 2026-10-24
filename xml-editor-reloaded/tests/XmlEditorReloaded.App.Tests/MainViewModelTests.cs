using System.Globalization;
using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Data.Settings;
using XmlEditorReloaded.Xml.Repository;

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
            new TableEditorFactory(dialog, new TestItemImageService()),
            settingsStore,
            dialog,
            new FakeImageViewerService());

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
            new TableEditorFactory(dialog, new TestItemImageService()),
            new JsonSettingsStore(workspace.PathOf("settings.json")),
            dialog,
            new FakeImageViewerService())
        {
            DataDirectory = data13,
        };

        await viewModel.RefreshCommand.ExecuteAsync(null);

        Assert.Equal("Error: boom", viewModel.StatusText);
        Assert.False(viewModel.IsBusy);
    }

    /// <summary>
    /// Opening a table creates an editor tab, cells edit through the workspace, invalid values are
    /// rejected, saving writes the file, and closing a clean tab removes it.
    /// </summary>
    [Fact]
    public async Task OpenTable_EditsCellAndSaves()
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

        var editor = Assert.Single(viewModel.Editors);
        Assert.Same(editor, viewModel.SelectedEditor);
        Assert.True(viewModel.HasEditors);
        Assert.Single(editor.Rows);
        Assert.Equal("Items.Items", editor.DisplayName);
        var row = editor.Rows[0];
        Assert.Equal("Nothing", row["szItemName"]);

        row["szItemName"] = "Glock 18";
        Assert.True(editor.IsDirty);
        Assert.EndsWith("*", editor.DisplayName, StringComparison.Ordinal);

        row["ubWeight"] = "not a number";
        Assert.Equal("10", row["ubWeight"]);
        Assert.Contains("not a valid value", editor.StatusText, StringComparison.Ordinal);

        await editor.SaveCommand.ExecuteAsync(null);

        Assert.False(editor.IsDirty);
        Assert.Equal("Items.Items", editor.DisplayName);
        var saved = XDocument.Load(Path.Combine(data13, "TableData", "Items", "Items.xml"));
        Assert.Equal("Glock 18", saved.Root!.Element("ITEM")!.Element("szItemName")!.Value);

        viewModel.CloseEditorCommand.Execute(editor);
        Assert.Empty(viewModel.Editors);
        Assert.False(viewModel.HasEditors);
    }

    private static MainViewModel CreateShell(
        SqliteMetadataProvider provider,
        FileSystemTableCatalog catalog,
        TableRepository repository,
        JsonSettingsStore settingsStore,
        FakeDialogService dialog) =>
        new(
            new DataSetSessionFactory(provider, catalog, repository),
            new TableEditorFactory(dialog, new TestItemImageService()),
            settingsStore,
            dialog,
            new FakeImageViewerService());

    private static string CreateItemsData(
        TestWorkspace workspace,
        bool includeWeight = false)
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
        return data13;
    }

    private static async Task<(
        SqliteMetadataProvider Provider,
        FileSystemTableCatalog Catalog,
        TableRepository Repository,
        JsonSettingsStore SettingsStore)> CreateServicesAsync(TestWorkspace workspace, string data13)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var settingsStore = new JsonSettingsStore(workspace.PathOf("settings.json"));
        var settings = new AppSettings(
            new[] { data13 },
            data13,
            "ru",
            "en",
            new[] { data13 });
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

        public uint? EditBitsResult { get; set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

        public uint? EditBits(string title, uint value) => EditBitsResult;
    }

    private sealed class FakeImageViewerService : IImageViewerService
    {
        public string? LastDataDirectory { get; private set; }

        public void Show(string? dataDirectory) => LastDataDirectory = dataDirectory;
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
