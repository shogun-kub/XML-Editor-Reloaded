using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One configured data directory in the settings dialog.
/// </summary>
public sealed class SettingsDirectoryItem : ObservableObject
{
    private bool _isActive;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsDirectoryItem"/> class.
    /// </summary>
    /// <param name="path">Absolute directory path.</param>
    /// <param name="isActive">Whether the directory is the active one.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    public SettingsDirectoryItem(string path, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Path = path;
        _isActive = isActive;
    }

    /// <summary>
    /// Gets the absolute directory path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this directory is the active one.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
}

/// <summary>
/// One selectable interface language.
/// </summary>
/// <param name="Code">Language code stored in settings.</param>
/// <param name="DisplayName">Text shown in the language selector.</param>
public sealed record UiLanguageOption(string Code, string DisplayName);

/// <summary>
/// One selectable visual theme.
/// </summary>
/// <param name="Mode">Theme mode stored in settings.</param>
/// <param name="DisplayName">Text shown in the theme selector.</param>
public sealed record ThemeOption(EditorThemeMode Mode, string DisplayName);

/// <summary>
/// View model of the settings dialog: configured data directories, the active directory, recently used
/// directories, the interface language, and the stored-layout reset.
/// </summary>
public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private string? _activeDirectory;
    private SettingsDirectoryItem? _selectedDirectory;
    private UiLanguageOption? _selectedLanguage;
    private ThemeOption? _selectedTheme;
    private string _statusText = string.Empty;
    private bool _resetLayoutsRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="request">Initial settings state.</param>
    /// <param name="dialogService">Dialog service used for folder picking and confirmations.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public SettingsViewModel(SettingsRequest request, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(dialogService);

        _dialogService = dialogService;
        Title = Strings.Get("SettingsTitle");
        DataDirectoriesLabel = Strings.Get("SettingsDataDirectoriesLabel");
        RecentDirectoriesLabel = Strings.Get("SettingsRecentLabel");
        UiLanguageLabel = Strings.Get("SettingsUiLanguageLabel");
        UiLanguageNote = Strings.Get("SettingsUiLanguageNote");
        ThemeLabel = Strings.Get("SettingsThemeLabel");
        AppearanceLabel = Strings.Get("SettingsAppearanceLabel");
        ResetLayoutsText = Strings.Get("SettingsResetLayouts");
        ResetLayoutsPendingText = Strings.Get("SettingsResetLayoutsPending");
        AddDirectoryText = Strings.Get("SettingsAddDirectory");
        RemoveDirectoryText = Strings.Get("SettingsRemoveDirectory");
        SetActiveText = Strings.Get("SettingsSetActive");
        ActiveColumn = Strings.Get("SettingsActiveColumn");
        PathColumn = Strings.Get("SettingsPathColumn");
        OkText = Strings.Get("SettingsOk");
        CancelText = Strings.Get("SettingsCancel");
        ActiveDirectoryLabel = Strings.Get("SettingsActiveDirectoryLabel");
        UiLanguageOptions = new ObservableCollection<UiLanguageOption>
        {
            new("en", Strings.Get("LanguageEnglish")),
            new("ru", Strings.Get("LanguageRussian")),
        };
        _selectedLanguage = UiLanguageOptions.FirstOrDefault(option =>
            string.Equals(option.Code, request.UiLanguageCode, StringComparison.OrdinalIgnoreCase))
            ?? UiLanguageOptions[0];

        ThemeOptions = new ObservableCollection<ThemeOption>
        {
            new(EditorThemeMode.System, Strings.Get("ThemeSystem")),
            new(EditorThemeMode.Light, Strings.Get("ThemeLight")),
            new(EditorThemeMode.Dark, Strings.Get("ThemeDark")),
        };
        _selectedTheme = ThemeOptions.FirstOrDefault(option => option.Mode == request.ThemeMode)
            ?? ThemeOptions[0];

        foreach (var directory in request.DataDirectories)
        {
            if (string.IsNullOrWhiteSpace(directory)
                || Directories.Any(existing => SamePath(existing.Path, directory)))
            {
                continue;
            }

            Directories.Add(new SettingsDirectoryItem(directory, isActive: false));
        }

        _activeDirectory = Directories
            .Select(static item => item.Path)
            .FirstOrDefault(path => SamePath(path, request.ActiveDataDirectory));
        if (_activeDirectory is null && Directories.Count > 0)
        {
            _activeDirectory = Directories[0].Path;
        }

        foreach (var recent in request.RecentDataDirectories)
        {
            if (!string.IsNullOrWhiteSpace(recent) && !RecentDirectories.Contains(recent))
            {
                RecentDirectories.Add(recent);
            }
        }

        RefreshActiveFlags();
        _selectedDirectory = Directories.FirstOrDefault();
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the data-directories label.
    /// </summary>
    public string DataDirectoriesLabel { get; }

    /// <summary>
    /// Gets the active-directory label.
    /// </summary>
    public string ActiveDirectoryLabel { get; }

    /// <summary>
    /// Gets the recently-used-directories label.
    /// </summary>
    public string RecentDirectoriesLabel { get; }

    /// <summary>
    /// Gets the interface-language label.
    /// </summary>
    public string UiLanguageLabel { get; }

    /// <summary>
    /// Gets the note explaining that the interface language changes after a restart.
    /// </summary>
    public string UiLanguageNote { get; }

    /// <summary>
    /// Gets the visual-theme label.
    /// </summary>
    public string ThemeLabel { get; }

    /// <summary>
    /// Gets the appearance group header.
    /// </summary>
    public string AppearanceLabel { get; }

    /// <summary>
    /// Gets the reset-layouts button text.
    /// </summary>
    public string ResetLayoutsText { get; }

    /// <summary>
    /// Gets the text shown after the reset was requested.
    /// </summary>
    public string ResetLayoutsPendingText { get; }

    /// <summary>
    /// Gets the add-directory button text.
    /// </summary>
    public string AddDirectoryText { get; }

    /// <summary>
    /// Gets the remove-directory button text.
    /// </summary>
    public string RemoveDirectoryText { get; }

    /// <summary>
    /// Gets the set-active button text.
    /// </summary>
    public string SetActiveText { get; }

    /// <summary>
    /// Gets the active-column header.
    /// </summary>
    public string ActiveColumn { get; }

    /// <summary>
    /// Gets the path-column header.
    /// </summary>
    public string PathColumn { get; }

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    public string OkText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the configured data directories.
    /// </summary>
    public ObservableCollection<SettingsDirectoryItem> Directories { get; } = new();

    /// <summary>
    /// Gets the recently used directories; they are shown for reference and are not edited here.
    /// </summary>
    public ObservableCollection<string> RecentDirectories { get; } = new();

    /// <summary>
    /// Gets the selectable interface languages.
    /// </summary>
    public ObservableCollection<UiLanguageOption> UiLanguageOptions { get; }

    /// <summary>
    /// Gets the selectable visual themes.
    /// </summary>
    public ObservableCollection<ThemeOption> ThemeOptions { get; }

    /// <summary>
    /// Gets or sets the selected directory row.
    /// </summary>
    public SettingsDirectoryItem? SelectedDirectory
    {
        get => _selectedDirectory;
        set => SetProperty(ref _selectedDirectory, value);
    }

    /// <summary>
    /// Gets or sets the selected interface language.
    /// </summary>
    public UiLanguageOption? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    /// <summary>
    /// Gets or sets the selected visual theme.
    /// </summary>
    public ThemeOption? SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    /// <summary>
    /// Gets the absolute path of the active data directory, or a localized placeholder when none is
    /// configured.
    /// </summary>
    public string ActiveDirectoryText => _activeDirectory
        ?? Strings.Get("SettingsNoActiveDirectory");

    /// <summary>
    /// Gets a value indicating whether all stored layouts will be removed when the dialog is accepted.
    /// </summary>
    public bool IsResetLayoutsPending => _resetLayoutsRequested;

    /// <summary>
    /// Gets or sets the status text shown at the bottom of the dialog.
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    /// <summary>
    /// Adds a data directory selected with the folder picker. Folders that do not contain a
    /// <c>TableData</c> directory are rejected without changing the list.
    /// </summary>
    [RelayCommand]
    private void AddDirectory()
    {
        var folder = _dialogService.PickFolder(
            Strings.Get("SettingsChooseDirectory"),
            _activeDirectory ?? SelectedDirectory?.Path);
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        if (DataSetPaths.ResolveTableDataDirectory(folder) is null)
        {
            StatusText = Strings.Get("InvalidDataDirectory");
            return;
        }

        var existing = Directories.FirstOrDefault(item => SamePath(item.Path, folder));
        if (existing is not null)
        {
            SelectDirectory(existing);
            return;
        }

        var added = new SettingsDirectoryItem(folder, isActive: false);
        Directories.Add(added);
        if (_activeDirectory is null)
        {
            _activeDirectory = folder;
        }

        RefreshActiveFlags();
        SelectDirectory(added);
        StatusText = Strings.Get("SettingsDirectoryAdded");
    }

    /// <summary>
    /// Removes the selected data directory. Removing the active directory makes the first remaining
    /// directory active.
    /// </summary>
    [RelayCommand]
    private void RemoveDirectory()
    {
        if (SelectedDirectory is null)
        {
            StatusText = Strings.Get("SettingsNoDirectorySelected");
            return;
        }

        var removed = SelectedDirectory;
        var index = Directories.IndexOf(removed);
        Directories.Remove(removed);
        if (SamePath(_activeDirectory, removed.Path))
        {
            _activeDirectory = Directories.FirstOrDefault()?.Path;
        }

        RefreshActiveFlags();
        SelectedDirectory = Directories.Count == 0
            ? null
            : Directories[Math.Min(index, Directories.Count - 1)];
        StatusText = Strings.Get("SettingsDirectoryRemoved");
    }

    /// <summary>
    /// Makes the selected directory the active one.
    /// </summary>
    [RelayCommand]
    private void SetActiveDirectory()
    {
        if (SelectedDirectory is null)
        {
            StatusText = Strings.Get("SettingsNoDirectorySelected");
            return;
        }

        _activeDirectory = SelectedDirectory.Path;
        RefreshActiveFlags();
        StatusText = Strings.Get("SettingsActiveDirectoryChanged");
    }

    /// <summary>
    /// Marks every stored layout for removal when the dialog is accepted.
    /// </summary>
    [RelayCommand]
    private void ResetLayouts()
    {
        if (!_dialogService.Confirm(
                Strings.Get("SettingsResetLayoutsConfirm"),
                Strings.Get("SettingsResetLayoutsTitle")))
        {
            return;
        }

        _resetLayoutsRequested = true;
        OnPropertyChanged(nameof(IsResetLayoutsPending));
        StatusText = ResetLayoutsPendingText;
    }

    /// <summary>
    /// Builds the confirmed settings.
    /// </summary>
    /// <returns>Settings carrying the edited directories, active directory, and language.</returns>
    public SettingsResult CreateResult()
    {
        var paths = Directories.Select(static item => item.Path).ToArray();
        var active = paths.FirstOrDefault(path => SamePath(path, _activeDirectory));
        return new SettingsResult(
            paths,
            active,
            SelectedLanguage?.Code ?? "en",
            _resetLayoutsRequested,
            SelectedTheme?.Mode ?? EditorThemeMode.System);
    }

    private void SelectDirectory(SettingsDirectoryItem item)
    {
        SelectedDirectory = item;
        OnPropertyChanged(nameof(ActiveDirectoryText));
    }

    private void RefreshActiveFlags()
    {
        foreach (var directory in Directories)
        {
            directory.IsActive = SamePath(directory.Path, _activeDirectory);
        }

        OnPropertyChanged(nameof(ActiveDirectoryText));
    }

    private static bool SamePath(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
        {
            return false;
        }

        return string.Equals(
            Path.TrimEndingDirectorySeparator(first),
            Path.TrimEndingDirectorySeparator(second),
            StringComparison.OrdinalIgnoreCase);
    }
}
