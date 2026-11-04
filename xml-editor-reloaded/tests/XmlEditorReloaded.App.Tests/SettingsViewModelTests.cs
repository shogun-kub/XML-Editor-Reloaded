using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the settings dialog view model: directory management, the active directory, the interface
/// language, and the stored-layout reset flag.
/// </summary>
public class SettingsViewModelTests
{
    /// <summary>
    /// Adding a valid directory appends it, and the selected directory can be made active.
    /// </summary>
    [Fact]
    public void AddDirectory_ThenSetActive_BuildsExpectedResult()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var second = CreateDataDirectory(workspace, "second");
        var dialog = new FakeDialogService { FolderResult = second };
        var viewModel = CreateViewModel(dialog, first, first);

        viewModel.AddDirectoryCommand.Execute(null);

        Assert.Equal(2, viewModel.Directories.Count);
        Assert.True(viewModel.Directories[0].IsActive);
        Assert.False(viewModel.Directories[1].IsActive);
        Assert.Equal(second, viewModel.SelectedDirectory!.Path);

        viewModel.SetActiveDirectoryCommand.Execute(null);
        var result = viewModel.CreateResult();

        Assert.Equal(new[] { first, second }, result.DataDirectories);
        Assert.Equal(second, result.ActiveDataDirectory);
        Assert.True(viewModel.Directories[1].IsActive);
        Assert.Equal("en", result.UiLanguageCode);
        Assert.False(result.ResetLayouts);
    }

    /// <summary>
    /// A folder that does not contain TableData is rejected without changing the list.
    /// </summary>
    [Fact]
    public void AddDirectory_InvalidFolder_IsRejected()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var invalid = workspace.PathOf("plain");
        Directory.CreateDirectory(invalid);
        var dialog = new FakeDialogService { FolderResult = invalid };
        var viewModel = CreateViewModel(dialog, first, first);

        viewModel.AddDirectoryCommand.Execute(null);

        Assert.Single(viewModel.Directories);
        Assert.Equal("The selected folder does not contain TableData or Data-1.13\\TableData.", viewModel.StatusText);
    }

    /// <summary>
    /// Removing the active directory makes the first remaining directory active.
    /// </summary>
    [Fact]
    public void RemoveActiveDirectory_ActivatesFirstRemaining()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var second = CreateDataDirectory(workspace, "second");
        var viewModel = CreateViewModel(new FakeDialogService(), second, first, second);

        viewModel.SelectedDirectory = viewModel.Directories[1];
        viewModel.RemoveDirectoryCommand.Execute(null);

        var result = viewModel.CreateResult();
        Assert.Equal(new[] { first }, result.DataDirectories);
        Assert.Equal(first, result.ActiveDataDirectory);
        Assert.True(viewModel.Directories[0].IsActive);
    }

    /// <summary>
    /// The reset flag is only set after confirmation and is carried into the dialog result.
    /// </summary>
    [Fact]
    public void ResetLayouts_RequiresConfirmation()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var dialog = new FakeDialogService { ConfirmResult = false };
        var viewModel = CreateViewModel(dialog, first, first);

        viewModel.ResetLayoutsCommand.Execute(null);
        Assert.False(viewModel.IsResetLayoutsPending);
        Assert.False(viewModel.CreateResult().ResetLayouts);

        dialog.ConfirmResult = true;
        viewModel.ResetLayoutsCommand.Execute(null);

        Assert.True(viewModel.IsResetLayoutsPending);
        Assert.True(viewModel.CreateResult().ResetLayouts);
        Assert.Equal("All stored layouts will be removed when you click OK.", viewModel.StatusText);
    }

    /// <summary>
    /// The selected interface language is carried into the dialog result.
    /// </summary>
    [Fact]
    public void SelectedLanguage_IsCarriedIntoResult()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var viewModel = new SettingsViewModel(
            new SettingsRequest(new[] { first }, first, "ru", Array.Empty<string>()),
            new FakeDialogService());

        Assert.Equal("ru", viewModel.SelectedLanguage!.Code);

        viewModel.SelectedLanguage = viewModel.UiLanguageOptions[0];
        Assert.Equal("en", viewModel.CreateResult().UiLanguageCode);
    }

    /// <summary>
    /// The selected theme is carried into the dialog result and defaults to the system theme.
    /// </summary>
    [Fact]
    public void SelectedTheme_IsCarriedIntoResult()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var first = CreateDataDirectory(workspace, "first");
        var viewModel = new SettingsViewModel(
            new SettingsRequest(
                new[] { first },
                first,
                "en",
                Array.Empty<string>(),
                EditorThemeMode.Dark),
            new FakeDialogService());

        Assert.Equal(EditorThemeMode.Dark, viewModel.SelectedTheme!.Mode);
        Assert.Equal(EditorThemeMode.Dark, viewModel.CreateResult().ThemeMode);

        viewModel.SelectedTheme = viewModel.ThemeOptions[0];
        Assert.Equal(EditorThemeMode.System, viewModel.CreateResult().ThemeMode);
    }

    private static SettingsViewModel CreateViewModel(
        FakeDialogService dialog,
        string active,
        params string[] directories) =>
        new(
            new SettingsRequest(directories, active, "en", Array.Empty<string>()),
            dialog);

    private static string CreateDataDirectory(TestWorkspace workspace, string name)
    {
        var path = workspace.PathOf(name);
        Directory.CreateDirectory(Path.Combine(path, "Data-1.13", "TableData"));
        return path;
    }

    private sealed class FakeDialogService : IDialogService
    {
        public string? FolderResult { get; set; }

        public bool ConfirmResult { get; set; } = true;

        public string? PickFolder(string title, string? initialDirectory = null) => FolderResult;

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public bool Confirm(string message, string title) => ConfirmResult;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesChoice.Cancel;

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

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => null;
    }
}
