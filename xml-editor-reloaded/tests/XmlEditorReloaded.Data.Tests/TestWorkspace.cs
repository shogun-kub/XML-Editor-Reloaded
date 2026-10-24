namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Creates an isolated temporary directory for data-layer tests.
/// </summary>
internal sealed class TestWorkspace : IDisposable
{
    /// <summary>
    /// Initializes a new workspace under the system temporary directory.
    /// </summary>
    public TestWorkspace()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), "xer-data-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
    }

    /// <summary>
    /// Gets the absolute path of the workspace directory.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Gets a path inside the workspace.
    /// </summary>
    /// <param name="relativePath">Relative path to combine.</param>
    /// <returns>Absolute path.</returns>
    public string PathOf(string relativePath) => Path.Combine(DirectoryPath, relativePath);

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
