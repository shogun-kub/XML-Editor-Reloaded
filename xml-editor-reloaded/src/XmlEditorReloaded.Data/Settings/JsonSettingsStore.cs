using System.Text;
using System.Text.Json;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;

namespace XmlEditorReloaded.Data.Settings;

/// <summary>
/// Stores application settings as JSON under the user's application-data directory.
/// </summary>
public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private readonly string _settingsPath;
    private readonly SemaphoreSlim _fileGate = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSettingsStore"/> class.
    /// </summary>
    /// <param name="settingsPath">
    /// Optional settings file path; defaults to <c>%AppData%/XmlEditorReloaded/settings.json</c>.
    /// </param>
    public JsonSettingsStore(string? settingsPath = null)
    {
        _settingsPath = settingsPath ?? DefaultSettingsPath;
    }

    /// <summary>
    /// Gets the default settings file path under the current user's application-data directory, or
    /// under the directory named by <c>XER_APP_DATA</c> when that variable is set.
    /// </summary>
    public static string DefaultSettingsPath => AppPaths.SettingsFilePath;

    /// <inheritdoc />
    public async Task<OperationResult<AppSettings>> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _fileGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await LoadCoreAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _fileGate.Release();
        }
    }

    private async Task<OperationResult<AppSettings>> LoadCoreAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_settingsPath))
        {
            return OperationResult<AppSettings>.Success(AppSettings.Default);
        }

        try
        {
            var json = await File.ReadAllTextAsync(_settingsPath, cancellationToken).ConfigureAwait(false);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json, SerializerOptions);
            return OperationResult<AppSettings>.Success(dto is null ? AppSettings.Default : Map(dto));
        }
        catch (JsonException exception)
        {
            return OperationResult<AppSettings>.Failure(
                ErrorCodes.IoError,
                $"The settings file '{_settingsPath}' contains invalid JSON: {exception.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<AppSettings>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the settings file '{_settingsPath}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<AppSettings>.Failure(
                ErrorCodes.IoError,
                $"The settings file '{_settingsPath}' could not be read: {exception.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<OperationResult> SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        await _fileGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await SaveCoreAsync(settings, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _fileGate.Release();
        }
    }

    private async Task<OperationResult> SaveCoreAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var directory = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrEmpty(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (UnauthorizedAccessException exception)
            {
                return OperationResult.Failure(
                    ErrorCodes.IoAccessDenied,
                    FileAccessFailure.DescribeWriteFailure(directory, exception));
            }
            catch (IOException exception)
            {
                return OperationResult.Failure(
                    ErrorCodes.IoError,
                    $"The settings directory '{directory}' could not be created: {exception.Message}");
            }
        }

        var tempPath = _settingsPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            var json = JsonSerializer.Serialize(Map(settings), SerializerOptions);
            await File.WriteAllTextAsync(tempPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), cancellationToken)
                .ConfigureAwait(false);
            File.Move(tempPath, _settingsPath, overwrite: true);
            return OperationResult.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            return OperationResult.Failure(
                ErrorCodes.IoAccessDenied,
                FileAccessFailure.DescribeWriteFailure(_settingsPath, exception));
        }
        catch (IOException exception)
        {
            var code = FileAccessFailure.IsSharingViolation(exception)
                ? ErrorCodes.IoAccessDenied
                : ErrorCodes.IoError;
            return OperationResult.Failure(code, FileAccessFailure.DescribeWriteFailure(_settingsPath, exception));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (IOException)
                {
                    // A leftover temporary file does not affect the saved settings.
                }
            }
        }
    }

    private static AppSettings Map(SettingsDto dto) => new(
        dto.DataDirectories ?? new List<string>(),
        dto.ActiveDataDirectory,
        dto.ActiveLanguageCode,
        dto.UiLanguageCode ?? "en",
        dto.RecentDataDirectories ?? new List<string>(),
        Map(dto.WindowPlacement),
        Enum.TryParse<EditorThemeMode>(dto.ThemeMode, ignoreCase: true, out var themeMode)
            ? themeMode
            : EditorThemeMode.System);

    private static WindowPlacement? Map(WindowPlacementDto? dto)
    {
        if (dto is not { Left: { } left, Top: { } top, Width: { } width, Height: { } height })
        {
            return null;
        }

        var placement = new WindowPlacement(left, top, width, height, dto.IsMaximized ?? false);
        return placement.HasUsableSize ? placement : null;
    }

    private static SettingsDto Map(AppSettings settings) => new()
    {
        DataDirectories = settings.DataDirectories.ToList(),
        ActiveDataDirectory = settings.ActiveDataDirectory,
        ActiveLanguageCode = settings.ActiveLanguageCode,
        UiLanguageCode = settings.UiLanguageCode,
        RecentDataDirectories = settings.RecentDataDirectories.ToList(),
        ThemeMode = settings.ThemeMode.ToString(),
        WindowPlacement = settings.WindowPlacement is { } placement
            ? new WindowPlacementDto
            {
                Left = placement.Left,
                Top = placement.Top,
                Width = placement.Width,
                Height = placement.Height,
                IsMaximized = placement.IsMaximized,
            }
            : null,
    };

    private sealed class SettingsDto
    {
        public List<string>? DataDirectories { get; set; }

        public string? ActiveDataDirectory { get; set; }

        public string? ActiveLanguageCode { get; set; }

        public string? UiLanguageCode { get; set; }

        public List<string>? RecentDataDirectories { get; set; }

        public string? ThemeMode { get; set; }

        public WindowPlacementDto? WindowPlacement { get; set; }
    }

    private sealed class WindowPlacementDto
    {
        public double? Left { get; set; }

        public double? Top { get; set; }

        public double? Width { get; set; }

        public double? Height { get; set; }

        public bool? IsMaximized { get; set; }
    }
}
