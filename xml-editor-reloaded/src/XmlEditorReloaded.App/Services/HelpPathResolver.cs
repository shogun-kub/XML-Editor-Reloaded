using System.IO;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Resolves Markdown link targets relative to the help document that contains them.
/// </summary>
public static class HelpPathResolver
{
    /// <summary>
    /// Resolves a link target against a help document path.
    /// </summary>
    /// <param name="documentPath">Path of the document containing the link.</param>
    /// <param name="target">Link target as written in Markdown.</param>
    /// <returns>A normalized root-relative document path, or the original anchor or URL target.</returns>
    public static string Resolve(string? documentPath, string target)
    {
        if (string.IsNullOrWhiteSpace(target)
            || target.StartsWith('#')
            || target.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || target.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || target.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        {
            return target;
        }

        var normalizedTarget = target.Replace('\\', '/');
        if (normalizedTarget.StartsWith('/'))
        {
            return TrimSegments(normalizedTarget.TrimStart('/'));
        }

        var baseDirectory = string.IsNullOrWhiteSpace(documentPath)
            ? string.Empty
            : Path.GetDirectoryName(documentPath)?.Replace('\\', '/') ?? string.Empty;
        var combined = baseDirectory.Length == 0
            ? normalizedTarget
            : baseDirectory + "/" + normalizedTarget;
        return TrimSegments(combined);
    }

    private static string TrimSegments(string path)
    {
        var segments = new List<string>();
        foreach (var segment in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".")
            {
                continue;
            }

            if (segment == "..")
            {
                if (segments.Count > 0)
                {
                    segments.RemoveAt(segments.Count - 1);
                }

                continue;
            }

            segments.Add(segment);
        }

        return string.Join("/", segments);
    }
}
