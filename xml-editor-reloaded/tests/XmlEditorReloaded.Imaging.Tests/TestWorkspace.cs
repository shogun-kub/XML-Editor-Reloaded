namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Creates an isolated temporary directory for imaging tests.
/// </summary>
internal sealed class TestWorkspace : IDisposable
{
    /// <summary>
    /// Initializes a new workspace under the system temporary directory.
    /// </summary>
    public TestWorkspace()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), "xer-imaging-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
    }

    /// <summary>
    /// Gets the absolute path of the workspace directory.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Copies a fixture directory tree into the workspace and returns its path.
    /// </summary>
    /// <param name="relativeFixtureDirectory">Path relative to the test output <c>Fixtures</c> directory.</param>
    /// <returns>Absolute path of the copied directory.</returns>
    public string CopyFixtureDirectory(string relativeFixtureDirectory)
    {
        var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", relativeFixtureDirectory);
        var destination = Path.Combine(DirectoryPath, relativeFixtureDirectory);
        CopyDirectory(source, destination);
        return destination;
    }

    /// <summary>
    /// Copies one fixture file into the workspace.
    /// </summary>
    /// <param name="relativeFixturePath">Path relative to the test output <c>Fixtures</c> directory.</param>
    /// <returns>Absolute path of the copied file.</returns>
    public string CopyFixture(string relativeFixturePath)
    {
        var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", relativeFixturePath);
        var destination = Path.Combine(DirectoryPath, Path.GetFileName(relativeFixturePath));
        File.Copy(source, destination, overwrite: true);
        return destination;
    }

    /// <summary>
    /// Gets the absolute path of a fixture in the test output directory.
    /// </summary>
    /// <param name="relativeFixturePath">Path relative to <c>Fixtures</c>.</param>
    /// <returns>Absolute fixture path.</returns>
    public static string FixturePath(string relativeFixturePath) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", relativeFixturePath);

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
        catch (IOException)
        {
            // Temporary cleanup failures must not hide the test result.
        }
        catch (UnauthorizedAccessException)
        {
            // Temporary cleanup failures must not hide the test result.
        }
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var directory in Directory.EnumerateDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }
}
