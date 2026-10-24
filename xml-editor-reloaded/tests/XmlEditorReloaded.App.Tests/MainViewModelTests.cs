using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
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

        var viewModel = new MainViewModel(catalog, repository, settingsStore, new FakeDialogService());

        await viewModel.InitializeAsync();

        var item = Assert.Single(viewModel.Tables);
        Assert.Equal("Items.Items", item.TableName);
        Assert.True(item.Descriptor.IsMapped);

        viewModel.SelectedTable = item;
        await WaitUntilAsync(() => !string.IsNullOrEmpty(viewModel.RecordCountText));

        Assert.Contains("1", viewModel.RecordCountText, StringComparison.Ordinal);
        Assert.Equal("Loaded", viewModel.DetailsText);
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
}
