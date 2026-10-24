using System.IO;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Resolves the game directories used by the editor from a path selected by the user.
/// </summary>
public static class DataSetPaths
{
    /// <summary>
    /// Finds the <c>TableData</c> directory for a selected path. The path may point at
    /// <c>TableData</c> itself, at a <c>Data-1.13</c> directory, or at a game root that contains it.
    /// </summary>
    /// <param name="path">Selected directory path, or <see langword="null"/>.</param>
    /// <returns>Absolute <c>TableData</c> path, or <see langword="null"/> when none exists.</returns>
    public static string? ResolveTableDataDirectory(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        if (string.Equals(Path.GetFileName(path), "TableData", StringComparison.OrdinalIgnoreCase)
            && Directory.Exists(path))
        {
            return path;
        }

        var direct = Path.Combine(path, "TableData");
        if (Directory.Exists(direct))
        {
            return direct;
        }

        var nested = Path.Combine(path, "Data-1.13", "TableData");
        return Directory.Exists(nested) ? nested : null;
    }

    /// <summary>
    /// Finds the game data directory (the parent of <c>TableData</c>) that contains the
    /// <c>BigItems</c>, <c>Interface</c>, and <c>tilesets</c> folders.
    /// </summary>
    /// <param name="path">Selected directory path, or <see langword="null"/>.</param>
    /// <returns>Absolute data directory, or <see langword="null"/> when <c>TableData</c> cannot be resolved.</returns>
    public static string? ResolveGameDataDirectory(string? path)
    {
        var tableData = ResolveTableDataDirectory(path);
        return tableData is null ? null : Directory.GetParent(tableData)?.FullName;
    }
}
