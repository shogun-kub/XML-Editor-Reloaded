using XmlEditorReloaded.Core.Diagnostics;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for the process-wide <see cref="OperationLog"/> facade.
/// </summary>
public class OperationLogTests
{
    /// <summary>
    /// A call without an installed sink is a safe no-op and reports that logging is disabled.
    /// </summary>
    [Fact]
    public void Write_WithoutSink_IsNoOp()
    {
        var previous = OperationLog.Sink;
        OperationLog.Sink = null;

        try
        {
            Assert.False(OperationLog.IsEnabled);

            OperationLog.Info("Test", "No sink is installed.");

            Assert.False(OperationLog.IsEnabled);
            Assert.Null(OperationLog.Sink);
        }
        finally
        {
            OperationLog.Sink = previous;
        }
    }

    /// <summary>
    /// One entry carries the level, source, message, error code, UTC timestamp and the full exception text.
    /// </summary>
    [Fact]
    public void Write_WithSink_CapturesEntry()
    {
        var sink = new CapturingSink();
        var previous = OperationLog.Sink;
        OperationLog.Sink = sink;
        var before = DateTimeOffset.UtcNow;
        bool enabledWhileInstalled;

        try
        {
            enabledWhileInstalled = OperationLog.IsEnabled;
            OperationLog.Error(
                "TableFileSaver",
                "The table file could not be written.",
                "io.error",
                new InvalidOperationException("Disk on fire."));
        }
        finally
        {
            OperationLog.Sink = previous;
        }

        var entry = Assert.Single(sink.Entries);
        Assert.True(enabledWhileInstalled);
        Assert.Equal(OperationLogLevel.Error, entry.Level);
        Assert.Equal("TableFileSaver", entry.Source);
        Assert.Equal("The table file could not be written.", entry.Message);
        Assert.Equal("io.error", entry.ErrorCode);
        Assert.InRange(entry.TimestampUtc, before, DateTimeOffset.UtcNow);
        Assert.Equal(TimeSpan.Zero, entry.TimestampUtc.Offset);
        Assert.Contains("System.InvalidOperationException", entry.Detail, StringComparison.Ordinal);
        Assert.Contains("Disk on fire.", entry.Detail, StringComparison.Ordinal);
    }

    /// <summary>
    /// The helper methods map every level to the matching entry.
    /// </summary>
    [Fact]
    public void Helpers_MapLevels()
    {
        var sink = new CapturingSink();
        var previous = OperationLog.Sink;
        OperationLog.Sink = sink;

        try
        {
            OperationLog.Info("Test", "info");
            OperationLog.Warning("Test", "warning");
            OperationLog.Error("Test", "error");
            OperationLog.Fatal("Test", "fatal");
        }
        finally
        {
            OperationLog.Sink = previous;
        }

        Assert.Equal(
            new[]
            {
                OperationLogLevel.Info,
                OperationLogLevel.Warning,
                OperationLogLevel.Error,
                OperationLogLevel.Fatal,
            },
            sink.Entries.Select(static entry => entry.Level));
    }

    /// <summary>
    /// A sink that throws must not turn the reported failure into a new exception for the caller.
    /// </summary>
    [Fact]
    public void Write_ThrowingSink_DoesNotThrow()
    {
        var previous = OperationLog.Sink;
        OperationLog.Sink = new ThrowingSink();

        try
        {
            OperationLog.Error("Test", "A failure that must stay reported.", "test.error");
        }
        finally
        {
            OperationLog.Sink = previous;
        }
    }

    /// <summary>
    /// A missing source or message is a programming error and is rejected before the sink is touched.
    /// </summary>
    /// <param name="source">Source passed to the facade.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Write_EmptySource_Throws(string source)
    {
        var previous = OperationLog.Sink;
        OperationLog.Sink = new CapturingSink();

        try
        {
            Assert.Throws<ArgumentException>(() => OperationLog.Write(OperationLogLevel.Info, source, "message"));
        }
        finally
        {
            OperationLog.Sink = previous;
        }
    }

    /// <summary>
    /// A null message is rejected.
    /// </summary>
    [Fact]
    public void Write_NullMessage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            OperationLog.Write(OperationLogLevel.Info, "Test", null!));
    }

    private sealed class CapturingSink : IOperationLog
    {
        public List<OperationLogEntry> Entries { get; } = new();

        public void Write(OperationLogEntry entry) => Entries.Add(entry);
    }

    private sealed class ThrowingSink : IOperationLog
    {
        public void Write(OperationLogEntry entry) => throw new IOException("The log target disappeared.");
    }
}
