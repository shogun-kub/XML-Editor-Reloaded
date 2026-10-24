using System.IO;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Builds short, user-facing labels for data sets that stay stable across tab titles.
/// </summary>
internal static class DatasetNames
{
    /// <summary>
    /// Returns the game directory name of a data set, falling back to the <c>Data-1.13</c> or
    /// <c>TableData</c> directory name when a game root is unavailable.
    /// </summary>
    /// <param name="session">Data-set session.</param>
    /// <returns>A short label such as <c>JA 2 1.13 v4</c>.</returns>
    public static string LabelFor(IDataSetSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var gameRoot = session.GameDataDirectory is { } gameData
            ? Directory.GetParent(gameData)?.FullName
            : null;
        var directory = gameRoot ?? session.GameDataDirectory ?? session.TableDataDirectory;
        var trimmed = Path.TrimEndingDirectorySeparator(directory);
        var name = Path.GetFileName(trimmed);
        return string.IsNullOrWhiteSpace(name) ? trimmed : name;
    }
}
