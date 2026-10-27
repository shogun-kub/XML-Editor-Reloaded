using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Data.Diagnostics;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="FileOperationLog"/>.
/// </summary>
public class FileOperationLogTests
{
    /// <summary>
    /// The first write creates a UTF-8 file without a byte-order mark, preceded by one session header that
    /// records the application version, user, operating system, runtime and memory usage.
    /// </summary>
    [Fact]
    public void Write_FirstEntry_WritesSessionHeaderOnce()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("logs/XmlEditorReloaded.log");
        var log = new FileOperationLog(path);

        log.Write(Entry(OperationLogLevel.Info, "First entry."));
        log.Write(Entry(OperationLogLevel.Warning, "Second entry."));

        var bytes = File.ReadAllBytes(path);
        Assert.NotEmpty(bytes);
        Assert.False(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);

        var text = File.ReadAllText(path);
        Assert.Contains("=== XML Editor Reloaded", text, StringComparison.Ordinal);
        Assert.Contains("User:", text, StringComparison.Ordinal);
        Assert.Contains("OS:", text, StringComparison.Ordinal);
        Assert.Contains("Runtime:", text, StringComparison.Ordinal);
        Assert.Contains("Working set:", text, StringComparison.Ordinal);
        Assert.Contains("Managed memory:", text, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(text, "=== XML Editor Reloaded"));
        Assert.Equal(1, CountOccurrences(text, "[INFO] Test (test.code)"));
        Assert.Equal(1, CountOccurrences(text, "[WARNING] Test (test.code)"));
        Assert.Contains("First entry.", text, StringComparison.Ordinal);
        Assert.Contains("Second entry.", text, StringComparison.Ordinal);
    }

    /// <summary>
    /// The technical detail keeps the full exception text, including the stack trace, indented under the
    /// message.
    /// </summary>
    [Fact]
    public void Write_WithException_IncludesStackAndErrorCode()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("XmlEditorReloaded.log");
        var log = new FileOperationLog(path);
        Exception exception;
        try
        {
            throw new InvalidOperationException("The table could not be read.");
        }
        catch (InvalidOperationException caught)
        {
            exception = caught;
        }

        log.Write(new OperationLogEntry(
            DateTimeOffset.UtcNow,
            OperationLogLevel.Error,
            "TableFileLoader",
            "Loading failed.",
            "io.error",
            exception.ToString()));

        var text = File.ReadAllText(path);
        Assert.Contains("[ERROR] TableFileLoader (io.error)", text, StringComparison.Ordinal);
        Assert.Contains("Loading failed.", text, StringComparison.Ordinal);
        Assert.Contains("System.InvalidOperationException", text, StringComparison.Ordinal);
        Assert.Contains("The table could not be read.", text, StringComparison.Ordinal);
        Assert.Contains("at ", text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Reaching the configured size rotates the previous file to <c>*.1</c> and starts the new file with a
    /// fresh session header.
    /// </summary>
    [Fact]
    public void Write_AtSizeLimit_RotatesToBackup()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("XmlEditorReloaded.log");
        var log = new FileOperationLog(path, maxFileBytes: 256);

        log.Write(Entry(OperationLogLevel.Info, "Before rotation."));
        Assert.True(new FileInfo(path).Length > 256);

        log.Write(Entry(OperationLogLevel.Info, "After rotation."));

        var backupPath = path + ".1";
        Assert.True(File.Exists(backupPath));
        Assert.Contains("Before rotation.", File.ReadAllText(backupPath), StringComparison.Ordinal);
        Assert.DoesNotContain("After rotation.", File.ReadAllText(backupPath), StringComparison.Ordinal);

        var current = File.ReadAllText(path);
        Assert.Contains("After rotation.", current, StringComparison.Ordinal);
        Assert.DoesNotContain("Before rotation.", current, StringComparison.Ordinal);
        Assert.Equal(1, CountOccurrences(current, "=== XML Editor Reloaded"));
    }

    /// <summary>
    /// A log path that cannot be prepared is swallowed instead of failing the logged operation.
    /// </summary>
    [Fact]
    public void Write_UnwritableDirectory_DoesNotThrow()
    {
        using var workspace = new TestWorkspace();
        var blocker = workspace.PathOf("blocker");
        File.WriteAllText(blocker, "This file occupies the directory name.");
        var log = new FileOperationLog(Path.Combine(blocker, "XmlEditorReloaded.log"));

        var exception = Record.Exception(() => log.Write(Entry(OperationLogLevel.Error, "Cannot be written.")));

        Assert.Null(exception);
    }

    /// <summary>
    /// Concurrent writers serialize on the instance lock, so every entry and exactly one session header
    /// reach the file.
    /// </summary>
    [Fact]
    public void Write_FromManyThreads_WritesEveryEntryOnce()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("XmlEditorReloaded.log");
        var log = new FileOperationLog(path);

        Parallel.For(0, 40, index => log.Write(Entry(OperationLogLevel.Info, $"Entry {index}.")));

        var text = File.ReadAllText(path);
        Assert.Equal(1, CountOccurrences(text, "=== XML Editor Reloaded"));
        for (var index = 0; index < 40; index++)
        {
            Assert.Equal(1, CountOccurrences(text, $"Entry {index}."));
        }
    }

    /// <summary>
    /// The default path lives under the current user's application-data directory.
    /// </summary>
    [Fact]
    public void DefaultFilePath_IsUnderApplicationData()
    {
        var expectedDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XmlEditorReloaded");

        Assert.Equal(
            Path.Combine(expectedDirectory, "XmlEditorReloaded.log"),
            FileOperationLog.DefaultFilePath);
        Assert.Equal(FileOperationLog.DefaultFilePath, new FileOperationLog().FilePath);
    }

    private static OperationLogEntry Entry(OperationLogLevel level, string message) => new(
        DateTimeOffset.UtcNow,
        level,
        "Test",
        message,
        "test.code");

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
