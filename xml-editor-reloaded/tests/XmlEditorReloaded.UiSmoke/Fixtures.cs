using System.Security.Cryptography;
using System.IO;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Data set a scenario works with.
/// </summary>
internal enum DatasetKind
{
    /// <summary>Russian v4 data set with language overlays.</summary>
    V4,

    /// <summary>English v5 base data set.</summary>
    V5,
}

/// <summary>
/// A resolved data set: either the read-only reference directory or a writable copy prepared for a
/// scenario.
/// </summary>
/// <param name="Kind">Data set kind.</param>
/// <param name="GameRoot">Directory that contains <c>Data-1.13</c>.</param>
/// <param name="DataDirectory">Directory that contains <c>TableData</c> and the image folders.</param>
/// <param name="TableDataDirectory">Directory that contains the XML tables.</param>
/// <param name="Label">Label the editor shows for the data set, for example <c>JA 2 1.13 v4</c>.</param>
/// <param name="IsCopy">Whether the directory is a writable copy.</param>
internal sealed record DatasetFixture(
    DatasetKind Kind,
    string GameRoot,
    string DataDirectory,
    string TableDataDirectory,
    string Label,
    bool IsCopy)
{
    /// <summary>
    /// Returns the absolute path of a table file inside the data set.
    /// </summary>
    /// <param name="relativePath">Path relative to <c>TableData</c>, for example <c>Items/Items.xml</c>.</param>
    /// <returns>Absolute file path.</returns>
    public string TableFile(string relativePath) =>
        Path.Combine(TableDataDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
}

/// <summary>
/// Creates writable data-set copies for scenarios and hashes table files for integrity checks.
/// </summary>
internal sealed class FixtureBuilder
{
    private static readonly string[] CopiedFolders = ["TableData", "BigItems", "Interface", "tilesets"];

    private readonly SmokeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixtureBuilder"/> class.
    /// </summary>
    /// <param name="options">Runner options.</param>
    public FixtureBuilder(SmokeOptions options) => _options = options;

    /// <summary>
    /// Returns the folder name of the reference data set.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <returns>Folder name such as <c>JA 2 1.13 v4</c>.</returns>
    public static string FolderName(DatasetKind kind) =>
        kind == DatasetKind.V4 ? "JA 2 1.13 v4" : "JA 2 1.13 v5";

    /// <summary>
    /// Returns the reference game root of a data set.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <returns>Absolute path of the read-only game root.</returns>
    public string ReferenceGameRoot(DatasetKind kind) =>
        Path.Combine(_options.ReferencesRoot, FolderName(kind));

    /// <summary>
    /// Resolves a data set for a scenario, copying it when the scenario may write.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <param name="scenarioDirectory">Scenario work directory.</param>
    /// <param name="copy">Whether a writable copy is required.</param>
    /// <returns>The resolved data set.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the reference data set is missing.</exception>
    public DatasetFixture Resolve(DatasetKind kind, string scenarioDirectory, bool copy)
    {
        var referenceRoot = ReferenceGameRoot(kind);
        var referenceData = Path.Combine(referenceRoot, "Data-1.13");
        if (!Directory.Exists(Path.Combine(referenceData, "TableData")))
        {
            throw new DirectoryNotFoundException(
                $"The reference data set '{referenceData}' does not contain TableData.");
        }

        if (!copy)
        {
            return new DatasetFixture(
                kind,
                referenceRoot,
                referenceData,
                Path.Combine(referenceData, "TableData"),
                FolderName(kind),
                IsCopy: false);
        }

        var targetRoot = Path.Combine(scenarioDirectory, FolderName(kind));
        var targetData = Path.Combine(targetRoot, "Data-1.13");
        if (Directory.Exists(targetData))
        {
            Directory.Delete(targetData, recursive: true);
        }

        Directory.CreateDirectory(targetData);
        foreach (var folder in CopiedFolders)
        {
            var source = Path.Combine(referenceData, folder);
            if (Directory.Exists(source))
            {
                CopyDirectory(source, Path.Combine(targetData, folder));
            }
        }

        return new DatasetFixture(
            kind,
            targetRoot,
            targetData,
            Path.Combine(targetData, "TableData"),
            FolderName(kind),
            IsCopy: true);
    }

    /// <summary>
    /// Replaces the copy's <c>Lookup/AttachmentPoint.xml</c> with a deliberately truncated XML document,
    /// so malformed-table scenarios do not depend on the mutable reference data set.
    /// </summary>
    /// <param name="fixture">Writable v5 fixture.</param>
    public static void CorruptAttachmentPoint(DatasetFixture fixture)
    {
        if (!fixture.IsCopy)
        {
            throw new InvalidOperationException("The malformed fixture requires a writable data-set copy.");
        }

        File.WriteAllText(
            fixture.TableFile("Lookup/AttachmentPoint.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<JA2Data>\n\t<AttachmentPoint>\n\t\t<id>0</id>\n");
    }

    /// <summary>
    /// Computes SHA-256 hashes of every file under the given directory.
    /// </summary>
    /// <param name="directory">Root directory.</param>
    /// <param name="searchPattern">Optional file pattern.</param>
    /// <returns>Map of normalized relative path to lowercase hexadecimal hash.</returns>
    public static Dictionary<string, string> HashTree(string directory, string searchPattern = "*")
    {
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(directory))
        {
            return hashes;
        }

        foreach (var file in Directory.EnumerateFiles(directory, searchPattern, SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(directory, file).Replace('\\', '/');
            using var stream = File.OpenRead(file);
            hashes[relative] = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
        }

        return hashes;
    }

    /// <summary>
    /// Compares two hash maps and returns the changed, added, and removed paths.
    /// </summary>
    /// <param name="before">Hashes before the run.</param>
    /// <param name="after">Hashes after the run.</param>
    /// <returns>Sorted relative paths whose content or presence changed.</returns>
    public static IReadOnlyList<string> Diff(
        IReadOnlyDictionary<string, string> before,
        IReadOnlyDictionary<string, string> after)
    {
        var changed = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in after)
        {
            if (!before.TryGetValue(pair.Key, out var hash) || !string.Equals(hash, pair.Value, StringComparison.Ordinal))
            {
                changed.Add(pair.Key);
            }
        }

        foreach (var path in before.Keys)
        {
            if (!after.ContainsKey(path))
            {
                changed.Add(path);
            }
        }

        return changed.ToList();
    }

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var file in Directory.EnumerateFiles(source))
        {
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var directory in Directory.EnumerateDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
        }
    }
}
