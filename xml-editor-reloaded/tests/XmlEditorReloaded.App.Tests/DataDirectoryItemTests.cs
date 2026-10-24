using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the data-directory selector entries.
/// </summary>
public class DataDirectoryItemTests
{
    /// <summary>
    /// Existing directories are shown by path, missing directories stay visible with a localized marker.
    /// </summary>
    [Fact]
    public void DisplayName_MarksMissingDirectories()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var existing = new DataDirectoryItem(workspace.DirectoryPath);
        var missing = new DataDirectoryItem(workspace.PathOf("missing"));

        Assert.True(existing.IsAvailable);
        Assert.Equal(workspace.DirectoryPath, existing.DisplayName);
        Assert.False(missing.IsAvailable);
        Assert.Equal($"{missing.Path} (missing)", missing.DisplayName);
    }
}
