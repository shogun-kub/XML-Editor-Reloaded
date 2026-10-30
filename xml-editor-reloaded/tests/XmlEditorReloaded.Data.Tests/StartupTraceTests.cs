using XmlEditorReloaded.Data.Diagnostics;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="StartupTrace"/>, the bootstrap trace written before the application log exists.
/// </summary>
public class StartupTraceTests
{
    /// <summary>
    /// Every entry is appended as one timestamped line; embedded line breaks stay on that line.
    /// </summary>
    [Fact]
    public void Write_AppendsTimestampedLines()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("XmlEditorReloaded-bootstrap.log");
        var trace = new StartupTrace(path);

        trace.Write("first entry");
        trace.Write("second\nentry");

        Assert.Null(trace.LastError);
        var text = File.ReadAllText(path);
        Assert.Contains("first entry", text, StringComparison.Ordinal);
        Assert.Contains("second entry", text, StringComparison.Ordinal);
        Assert.Equal(2, text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length);
        Assert.StartsWith("[", text, StringComparison.Ordinal);
    }

    /// <summary>
    /// A trace larger than the limit is recreated instead of growing without bound.
    /// </summary>
    [Fact]
    public void Write_OversizedFile_RecreatesIt()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("XmlEditorReloaded-bootstrap.log");
        File.WriteAllText(path, new string('x', 512));

        var trace = new StartupTrace(path, maxFileBytes: 128);
        trace.Write("fresh start");

        Assert.DoesNotContain("xxxx", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("fresh start", File.ReadAllText(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// A trace path that cannot be written is recorded on the instance instead of being thrown.
    /// </summary>
    [Fact]
    public void Write_UnwritablePath_DoesNotThrowAndRecordsError()
    {
        using var workspace = new TestWorkspace();
        var blocker = workspace.PathOf("blocker");
        File.WriteAllText(blocker, "This file occupies the directory name.");
        var trace = new StartupTrace(Path.Combine(blocker, "XmlEditorReloaded-bootstrap.log"));

        var exception = Record.Exception(() => trace.Write("cannot be written"));

        Assert.Null(exception);
        Assert.False(string.IsNullOrWhiteSpace(trace.LastError));
    }
}
