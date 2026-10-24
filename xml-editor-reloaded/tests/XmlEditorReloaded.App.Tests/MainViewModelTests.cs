using System.Globalization;
using System.IO;
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
/// Smoke tests for the WPF shell view model.
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
        var data13 = workspace.PathOf("Data-1.13");
        Directory.CreateDirectory(Path.Combine(data13, "TableData", "Items"));
        await File.WriteAllTextAsync(
            Path.Combine(data13, "TableData", "Items", "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName></ITEM>"
            + "</ITEMLIST>");

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

        var viewModel = new MainViewModel(
            catalog,
            repository,
            settingsStore,
            new FakeDialogService(),
            new FakeImageViewerService());

        await viewModel.InitializeAsync();

        var item = Assert.Single(viewModel.Tables);
        Assert.Equal("Items.Items", item.TableName);
        Assert.True(item.Descriptor.IsMapped);

        viewModel.SelectedTable = item;
        await WaitUntilAsync(() => !string.IsNullOrEmpty(viewModel.RecordCountText));

        Assert.Contains("1", viewModel.RecordCountText, StringComparison.Ordinal);
        Assert.Equal("Loaded", viewModel.DetailsText);
    }

    /// <summary>
    /// An invalid data directory clears the table list and reports the problem without throwing.
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
        var viewModel = new MainViewModel(
            new FileSystemTableCatalog(),
            new TableRepository(),
            settingsStore,
            new FakeDialogService(),
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
        var settingsStore = new JsonSettingsStore(workspace.PathOf("settings.json"));
        Assert.True((await settingsStore.SaveAsync(new AppSettings(
            new[] { data13 },
            data13,
            "en",
            "en",
            new[] { data13 }))).IsSuccess);
        var viewModel = new MainViewModel(
            new FileSystemTableCatalog(),
            new TableRepository(),
            settingsStore,
            new FakeDialogService(),
            new FakeImageViewerService());

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
    /// An unexpected catalog failure is reported through the status text instead of escaping the command.
    /// </summary>
    [Fact]
    public async Task Refresh_CatalogThrows_SetsErrorStatus()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        Directory.CreateDirectory(Path.Combine(data13, "TableData"));
        var viewModel = new MainViewModel(
            new ThrowingCatalog(),
            new TableRepository(),
            new JsonSettingsStore(workspace.PathOf("settings.json")),
            new FakeDialogService(),
            new FakeImageViewerService())
        {
            DataDirectory = data13,
        };

        await viewModel.RefreshCommand.ExecuteAsync(null);

        Assert.Equal("Error: boom", viewModel.StatusText);
        Assert.False(viewModel.IsBusy);
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
        public string? PickFolder(string title, string? initialDirectory = null) => null;
    }

    private sealed class FakeImageViewerService : IImageViewerService
    {
        public string? LastDataDirectory { get; private set; }

        public void Show(string? dataDirectory) => LastDataDirectory = dataDirectory;
    }

    private sealed class ThrowingCatalog : ITableCatalog
    {
        public Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
            string tableDataDirectory,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("boom");
    }
}
