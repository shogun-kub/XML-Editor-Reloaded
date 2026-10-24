namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Creates an isolated temporary directory for tests that need writable fixture copies.
/// </summary>
internal sealed class TestWorkspace : IDisposable
{
    /// <summary>
    /// Initializes a new workspace under the system temporary directory.
    /// </summary>
    public TestWorkspace()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), "xer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
    }

    /// <summary>
    /// Gets the absolute path of the workspace directory.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Copies one fixture file into the workspace.
    /// </summary>
    /// <param name="relativeFixturePath">Path relative to the test output <c>Fixtures</c> directory.</param>
    /// <param name="destinationFileName">Optional destination file name; the source name is used when omitted.</param>
    /// <returns>Absolute path of the copied file.</returns>
    public string CopyFixture(string relativeFixturePath, string? destinationFileName = null)
    {
        var source = Path.Combine(AppContext.BaseDirectory, "Fixtures", relativeFixturePath);
        var destination = Path.Combine(DirectoryPath, destinationFileName ?? Path.GetFileName(relativeFixturePath));
        File.Copy(source, destination, overwrite: true);
        return destination;
    }

    /// <summary>
    /// Copies a fixture directory tree into the workspace.
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
    /// Copies a directory recursively.
    /// </summary>
    /// <param name="source">Source directory.</param>
    /// <param name="destination">Destination directory.</param>
    public static void CopyDirectory(string source, string destination)
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
}
