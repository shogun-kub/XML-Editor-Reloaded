using System.IO;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One entry of the data-directory selector. Paths are kept exactly as configured; missing directories
/// remain visible so the user can see and correct a stale settings entry.
/// </summary>
public sealed class DataDirectoryItem
{
    /// <summary>
    /// Initializes a new item for a configured data directory.
    /// </summary>
    /// <param name="path">Absolute path selected by the user.</param>
    public DataDirectoryItem(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Path = path;
    }

    /// <summary>
    /// Gets the configured absolute path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a value indicating whether the directory currently exists.
    /// </summary>
    public bool IsAvailable => Directory.Exists(Path);

    /// <summary>
    /// Gets the text shown in the selector; missing directories carry a localized marker.
    /// </summary>
    public string DisplayName => IsAvailable ? Path : $"{Path} ({Strings.Get("DataDirectoryMissing")})";

    /// <inheritdoc />
    public override string ToString() => DisplayName;
}
