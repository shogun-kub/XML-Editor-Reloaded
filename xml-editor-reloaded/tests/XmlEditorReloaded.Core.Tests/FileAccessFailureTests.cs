using XmlEditorReloaded.Core.Diagnostics;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for <see cref="FileAccessFailure"/>, which turns a failed write into a cause and a next step.
/// </summary>
public class FileAccessFailureTests
{
    /// <summary>
    /// A read-only target is reported as such, with the path and the attribute to clear.
    /// </summary>
    [Fact]
    public void DescribeWriteFailure_ReadOnlyFile_MentionsReadOnlyAndPath()
    {
        var path = CreateTempFile();
        File.SetAttributes(path, FileAttributes.ReadOnly);
        try
        {
            var message = FileAccessFailure.DescribeWriteFailure(path, new UnauthorizedAccessException());

            Assert.Contains(path, message, StringComparison.Ordinal);
            Assert.Contains("read-only", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Read-only", message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }
    }

    /// <summary>
    /// A writable target that still denies access is reported as a Windows permission problem, not as a
    /// read-only file.
    /// </summary>
    [Fact]
    public void DescribeWriteFailure_AccessDenied_MentionsPermissions()
    {
        var path = CreateTempFile();
        try
        {
            var message = FileAccessFailure.DescribeWriteFailure(path, new UnauthorizedAccessException());

            Assert.Contains(path, message, StringComparison.Ordinal);
            Assert.Contains("permissions", message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("read-only", message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Sharing and lock violations are recognized and explained as a file held open by another process.
    /// </summary>
    [Theory]
    [InlineData(unchecked((int)0x80070020))]
    [InlineData(unchecked((int)0x80070021))]
    public void DescribeWriteFailure_SharingViolation_MentionsAnotherProcess(int hResult)
    {
        var path = CreateTempFile();
        try
        {
            var exception = new IOException("The process cannot access the file.", hResult);

            Assert.True(FileAccessFailure.IsSharingViolation(exception));
            var message = FileAccessFailure.DescribeWriteFailure(path, exception);
            Assert.Contains(path, message, StringComparison.Ordinal);
            Assert.Contains("another process", message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Other I/O failures keep the original message and still name the path.
    /// </summary>
    [Fact]
    public void DescribeWriteFailure_OtherIoError_KeepsOriginalMessage()
    {
        var path = CreateTempFile();
        try
        {
            var exception = new IOException("The disk is full.");

            Assert.False(FileAccessFailure.IsSharingViolation(exception));
            var message = FileAccessFailure.DescribeWriteFailure(path, exception);
            Assert.Contains(path, message, StringComparison.Ordinal);
            Assert.Contains("The disk is full.", message, StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string CreateTempFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"xer-access-{Guid.NewGuid():N}.xml");
        File.WriteAllText(path, "<ROOT />");
        return path;
    }
}
