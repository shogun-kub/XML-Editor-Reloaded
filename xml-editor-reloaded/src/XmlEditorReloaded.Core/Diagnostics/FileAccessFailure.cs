namespace XmlEditorReloaded.Core.Diagnostics;

/// <summary>
/// Explains a failed file write in terms of the most likely cause, including the affected path and a
/// next step that does not require elevation. The class only reads file metadata; it never changes
/// attributes or ACLs of game files.
/// </summary>
public static class FileAccessFailure
{
    private const int SharingViolationHResult = unchecked((int)0x80070020);
    private const int LockViolationHResult = unchecked((int)0x80070021);

    /// <summary>
    /// Determines whether an I/O exception was caused by another process that holds the file open.
    /// </summary>
    /// <param name="exception">Exception thrown by the file operation.</param>
    /// <returns><see langword="true"/> for a sharing or lock violation; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is null.</exception>
    public static bool IsSharingViolation(IOException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return exception.HResult == SharingViolationHResult || exception.HResult == LockViolationHResult;
    }

    /// <summary>
    /// Builds a user-facing explanation for a failed write to <paramref name="path"/>. A read-only
    /// attribute, a Windows permission problem, and a file held open by another process are reported
    /// with different next steps; any other I/O failure keeps the original message.
    /// </summary>
    /// <param name="path">Path the operation tried to write.</param>
    /// <param name="exception">Exception thrown by the write.</param>
    /// <returns>A message that names the path and the most likely cause.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is null.</exception>
    public static string DescribeWriteFailure(string path, Exception exception)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(exception);

        if (exception is UnauthorizedAccessException)
        {
            return IsReadOnlyFile(path)
                ? $"Access to '{path}' was denied because the file is read-only. "
                    + "Clear the 'Read-only' attribute in its properties or choose another folder."
                : $"Access to '{path}' was denied by Windows permissions. "
                    + "Grant write access to this file or its folder, or run the editor under the user "
                    + "who owns these files.";
        }

        if (exception is IOException io && IsSharingViolation(io))
        {
            return $"The file '{path}' is in use by another process. "
                + "Close the program that has it open and try again.";
        }

        return $"The file '{path}' could not be written: {exception.Message}";
    }

    private static bool IsReadOnlyFile(string path)
    {
        try
        {
            return File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly);
        }
        catch (Exception exception) when (
            exception is IOException
                or UnauthorizedAccessException
                or ArgumentException
                or NotSupportedException)
        {
            return false;
        }
    }
}
