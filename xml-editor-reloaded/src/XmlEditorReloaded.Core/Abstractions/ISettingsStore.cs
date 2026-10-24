using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Application settings persisted outside the game data directories.
/// </summary>
/// <param name="DataDirectories">Configured game data directories (roots containing <c>Data-1.13</c> or <c>TableData</c>).</param>
/// <param name="ActiveDataDirectory">Absolute path of the currently selected data directory, or <see langword="null"/>.</param>
/// <param name="ActiveLanguageCode">Language code used for overlay editing, or <see langword="null"/> for base-only.</param>
/// <param name="UiLanguageCode">Language code of the editor UI, for example <c>en</c> or <c>ru</c>.</param>
/// <param name="RecentDataDirectories">Recently used data directory paths, most recent first.</param>
public sealed record AppSettings(
    IReadOnlyList<string> DataDirectories,
    string? ActiveDataDirectory,
    string? ActiveLanguageCode,
    string? UiLanguageCode,
    IReadOnlyList<string> RecentDataDirectories)
{
    /// <summary>
    /// Gets the default settings used on first start.
    /// </summary>
    public static AppSettings Default { get; } = new(
        DataDirectories: Array.Empty<string>(),
        ActiveDataDirectory: null,
        ActiveLanguageCode: null,
        UiLanguageCode: "en",
        RecentDataDirectories: Array.Empty<string>());
}

/// <summary>
/// Loads and stores application settings.
/// </summary>
public interface ISettingsStore
{
    /// <summary>
    /// Loads the persisted settings, or returns <see cref="AppSettings.Default"/> when none exist.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the settings;
    /// on failure, <c>error_code</c> is <c>io.error</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<AppSettings>> LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the given settings atomically.
    /// </summary>
    /// <param name="settings">Settings to store.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>;
    /// on failure, <c>error_code</c> is <c>io.access_denied</c> or <c>io.error</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult> SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
