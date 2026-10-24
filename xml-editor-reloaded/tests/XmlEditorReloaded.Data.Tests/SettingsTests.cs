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
}
