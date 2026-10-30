using XmlEditorReloaded.Data.Diagnostics;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="StartupDiagnostics"/>: the write probe and the environment report that explain
/// access failures without elevation.
/// </summary>
public class StartupDiagnosticsTests
{
    /// <summary>
    /// A writable directory passes the probe and leaves no probe file behind.
    /// </summary>
    [Fact]
    public void ProbeWriteAccess_WritableDirectory_LeavesNoProbeFile()
    {
        using var workspace = new TestWorkspace();

        var probe = StartupDiagnostics.ProbeWriteAccess(workspace.DirectoryPath);

        Assert.True(probe.IsWritable);
        Assert.Contains(workspace.DirectoryPath, probe.Message, StringComparison.Ordinal);
        Assert.Empty(Directory.GetFiles(workspace.DirectoryPath, ".write-probe-*"));
    }

    /// <summary>
    /// A missing application-data directory is created by the probe and then passes.
    /// </summary>
    [Fact]
    public void ProbeWriteAccess_MissingDirectory_CreatesIt()
    {
        using var workspace = new TestWorkspace();
        var directory = workspace.PathOf("appdata");

        var probe = StartupDiagnostics.ProbeWriteAccess(directory);

        Assert.True(probe.IsWritable);
        Assert.True(Directory.Exists(directory));
    }

    /// <summary>
    /// A path that cannot be used as a directory fails the probe and the message names the path, so the
    /// user can identify the location.
    /// </summary>
    [Fact]
    public void ProbeWriteAccess_PathBlockedByFile_ReportsPath()
    {
        using var workspace = new TestWorkspace();
        var blocker = workspace.PathOf("blocker");
        File.WriteAllText(blocker, "This file occupies the directory name.");

        var probe = StartupDiagnostics.ProbeWriteAccess(blocker);

        Assert.False(probe.IsWritable);
        Assert.Contains(blocker, probe.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// An existing file is described with its size, attributes, and an owner entry; a read-only file
    /// shows the read-only attribute.
    /// </summary>
    [Fact]
    public void DescribeFile_ReadOnlyFile_ReportsAttributesAndOwner()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("settings.json");
        File.WriteAllText(path, "{}");
        File.SetAttributes(path, FileAttributes.ReadOnly);
        try
        {
            var line = StartupDiagnostics.DescribeFile(path);

            Assert.Contains(path, line, StringComparison.Ordinal);
            Assert.Contains("exists = yes", line, StringComparison.Ordinal);
            Assert.Contains("ReadOnly", line, StringComparison.Ordinal);
            Assert.Contains("owner =", line, StringComparison.Ordinal);
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }

    /// <summary>
    /// A missing file is described without throwing.
    /// </summary>
    [Fact]
    public void DescribeFile_MissingFile_ReportsMissing()
    {
        using var workspace = new TestWorkspace();

        var line = StartupDiagnostics.DescribeFile(workspace.PathOf("missing.log"));

        Assert.Contains("exists = no", line, StringComparison.Ordinal);
    }

    /// <summary>
    /// The environment report records the user, the elevation state, the application-data directory,
    /// and every watched file.
    /// </summary>
    [Fact]
    public void DescribeEnvironment_RecordsUserElevationAndFiles()
    {
        using var workspace = new TestWorkspace();
        var logPath = workspace.PathOf("XmlEditorReloaded.log");
        File.WriteAllText(logPath, "log");

        var report = StartupDiagnostics.DescribeEnvironment(workspace.DirectoryPath, logPath);

        Assert.Contains(Environment.UserName, report, StringComparison.Ordinal);
        Assert.Contains("elevated = ", report, StringComparison.Ordinal);
        Assert.Contains(workspace.DirectoryPath, report, StringComparison.Ordinal);
        Assert.Contains(logPath, report, StringComparison.Ordinal);
    }
}
