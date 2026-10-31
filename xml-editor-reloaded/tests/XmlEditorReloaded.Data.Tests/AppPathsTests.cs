using System.IO;
using XmlEditorReloaded.Data.Settings;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Serializes the tests that change the <c>XER_APP_DATA</c> process environment variable.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AppPathsTestCollection
{
    /// <summary>
    /// Name of the collection.
    /// </summary>
    public const string Name = "AppPaths";
}

/// <summary>
/// Tests for <see cref="AppPaths"/>, including the <c>XER_APP_DATA</c> override used by isolated
/// automated runs.
/// </summary>
[Collection(AppPathsTestCollection.Name)]
public class AppPathsTests
{
    /// <summary>
    /// The override variable redirects every derived path to the given directory.
    /// </summary>
    [Fact]
    public void Override_RedirectsAllPaths()
    {
        using var workspace = new TestWorkspace();
        var target = workspace.PathOf("appdata");
        var previous = Environment.GetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable);
        try
        {
            Environment.SetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable, target);

            Assert.Equal(Path.GetFullPath(target), AppPaths.ApplicationDataDirectory);
            Assert.Equal(Path.Combine(target, "settings.json"), AppPaths.SettingsFilePath);
            Assert.Equal(Path.Combine(target, "layouts.json"), AppPaths.LayoutsFilePath);
            Assert.Equal(Path.Combine(target, "XmlEditorReloaded.log"), AppPaths.LogFilePath);
            Assert.Equal(AppPaths.SettingsFilePath, JsonSettingsStore.DefaultSettingsPath);
            Assert.Equal(AppPaths.LayoutsFilePath, JsonLayoutStore.DefaultLayoutsPath);
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable, previous);
        }
    }

    /// <summary>
    /// A blank override is ignored and the roaming-profile default is used.
    /// </summary>
    [Fact]
    public void BlankOverride_FallsBackToProfile()
    {
        var previous = Environment.GetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable);
        try
        {
            Environment.SetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable, "  ");
            var expected = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppPaths.ApplicationFolderName);

            Assert.Equal(expected, AppPaths.ApplicationDataDirectory);
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppPaths.ApplicationDataOverrideVariable, previous);
        }
    }
}
