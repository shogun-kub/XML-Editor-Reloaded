using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Data.Settings;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="JsonSettingsStore"/>.
/// </summary>
public class SettingsTests
{
    /// <summary>
    /// Missing settings return defaults.
    /// </summary>
    [Fact]
    public async Task Load_MissingFile_ReturnsDefaults()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonSettingsStore(workspace.PathOf("settings.json"));

        var result = await store.LoadAsync();

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(AppSettings.Default, result.Data);
    }

    /// <summary>
    /// Settings round-trip through the JSON file.
    /// </summary>
    [Fact]
    public async Task SaveAndLoad_RoundTrip()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonSettingsStore(workspace.PathOf("nested/settings.json"));
        var settings = new AppSettings(
            new[] { @"E:\Games\JA2" },
            @"E:\Games\JA2",
            "ru",
            "en",
            new[] { @"E:\Games\JA2", @"E:\Games\JA2-v5" });

        var save = await store.SaveAsync(settings);
        var load = await store.LoadAsync();

        Assert.True(save.IsSuccess, save.ErrorText);
        Assert.True(load.IsSuccess, load.ErrorText);
        var actual = load.Data!;
        Assert.Equal(settings.DataDirectories, actual.DataDirectories);
        Assert.Equal(settings.ActiveDataDirectory, actual.ActiveDataDirectory);
        Assert.Equal(settings.ActiveLanguageCode, actual.ActiveLanguageCode);
        Assert.Equal(settings.UiLanguageCode, actual.UiLanguageCode);
        Assert.Equal(settings.RecentDataDirectories, actual.RecentDataDirectories);
    }

    /// <summary>
    /// Invalid JSON is reported as an I/O error rather than crashing.
    /// </summary>
    [Fact]
    public async Task Load_InvalidJson_ReturnsError()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        await File.WriteAllTextAsync(path, "{ invalid");
        var store = new JsonSettingsStore(path);

        var result = await store.LoadAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoError, result.ErrorCode);
    }

    /// <summary>
    /// The saved main-window geometry survives a save/load round-trip.
    /// </summary>
    [Fact]
    public async Task SaveAndLoad_WindowPlacement_RoundTrip()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonSettingsStore(workspace.PathOf("settings.json"));
        var placement = new WindowPlacement(-120, 40, 1280, 780, true);
        var settings = AppSettings.Default with
        {
            DataDirectories = new[] { @"E:\Games\JA2" },
            ActiveDataDirectory = @"E:\Games\JA2",
            WindowPlacement = placement,
        };

        var save = await store.SaveAsync(settings);
        var load = await store.LoadAsync();

        Assert.True(save.IsSuccess, save.ErrorText);
        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.Equal(placement, load.Data!.WindowPlacement);
        Assert.Equal(settings.ActiveDataDirectory, load.Data.ActiveDataDirectory);
    }

    /// <summary>
    /// A settings file written before window geometry existed still loads and reports no placement.
    /// </summary>
    [Fact]
    public async Task Load_SettingsWithoutWindowPlacement_ReturnsNullPlacement()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        await File.WriteAllTextAsync(
            path,
            """
            {
              "dataDirectories": ["E:\\Games\\JA2"],
              "activeDataDirectory": "E:\\Games\\JA2",
              "activeLanguageCode": "ru",
              "uiLanguageCode": "en",
              "recentDataDirectories": ["E:\\Games\\JA2"]
            }
            """);
        var store = new JsonSettingsStore(path);

        var result = await store.LoadAsync();

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Null(result.Data!.WindowPlacement);
        Assert.Equal(new[] { @"E:\Games\JA2" }, result.Data.DataDirectories);
    }

    /// <summary>
    /// An incomplete placement object is ignored instead of producing a window with zero size.
    /// </summary>
    [Fact]
    public async Task Load_PartialWindowPlacement_ReturnsNullPlacement()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        await File.WriteAllTextAsync(
            path,
            """
            {
              "dataDirectories": [],
              "uiLanguageCode": "en",
              "windowPlacement": { "left": 10, "top": 20 }
            }
            """);
        var store = new JsonSettingsStore(path);

        var result = await store.LoadAsync();

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Null(result.Data!.WindowPlacement);
    }

    /// <summary>
    /// A placement whose size cannot produce a usable window is discarded instead of being restored.
    /// </summary>
    [Fact]
    public async Task Load_UnusableWindowPlacement_ReturnsNullPlacement()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        await File.WriteAllTextAsync(
            path,
            """
            {
              "dataDirectories": [],
              "uiLanguageCode": "en",
              "windowPlacement": { "left": 10, "top": 20, "width": 0, "height": 0 }
            }
            """);
        var store = new JsonSettingsStore(path);

        var result = await store.LoadAsync();

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Null(result.Data!.WindowPlacement);
    }

    /// <summary>
    /// The saved geometry is written as a camel-case object so the settings file stays readable and
    /// stable across schema changes.
    /// </summary>
    [Fact]
    public async Task Save_WindowPlacement_WritesCamelCaseObject()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        var store = new JsonSettingsStore(path);

        Assert.True((await store.SaveAsync(AppSettings.Default with
        {
            WindowPlacement = new WindowPlacement(10, 20, 1300, 800, true),
        })).IsSuccess);

        var json = await File.ReadAllTextAsync(path);
        Assert.Contains("\"windowPlacement\"", json, StringComparison.Ordinal);
        Assert.Contains("\"left\": 10", json, StringComparison.Ordinal);
        Assert.Contains("\"top\": 20", json, StringComparison.Ordinal);
        Assert.Contains("\"width\": 1300", json, StringComparison.Ordinal);
        Assert.Contains("\"height\": 800", json, StringComparison.Ordinal);
        Assert.Contains("\"isMaximized\": true", json, StringComparison.Ordinal);
    }
}
