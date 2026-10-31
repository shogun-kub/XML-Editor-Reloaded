using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using XmlEditorReloaded.Core.Diagnostics;

namespace XmlEditorReloaded.Data.Diagnostics;

/// <summary>
/// Appends operation log entries to a UTF-8 text file under the user's application-data directory.
/// The file starts with one session header that records the application version, user, operating
/// system, runtime, and memory usage, followed by timestamped entries. The file is rotated once when
/// it reaches <see cref="DefaultMaxFileBytes"/>; the previous file is kept as <c>*.1</c>.
/// </summary>
public sealed class FileOperationLog : IOperationLog
{
    /// <summary>
    /// Default maximum size of one log file before it is rotated; 4 MiB.
    /// </summary>
    public const long DefaultMaxFileBytes = 4 * 1024 * 1024;

    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private readonly object _sync = new();
    private readonly string _filePath;
    private readonly long _maxFileBytes;
    private bool _sessionHeaderWritten;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationLog"/> class.
    /// </summary>
    /// <param name="filePath">
    /// Absolute path of the log file, or <see langword="null"/> to use <see cref="DefaultFilePath"/>.
    /// </param>
    /// <param name="maxFileBytes">Maximum size of one file before rotation; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxFileBytes"/> is not positive.</exception>
    public FileOperationLog(string? filePath = null, long maxFileBytes = DefaultMaxFileBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFileBytes);
        _filePath = string.IsNullOrWhiteSpace(filePath) ? DefaultFilePath : filePath;
        _maxFileBytes = maxFileBytes;
    }

    /// <summary>
    /// Gets the default log file path under the current user's application-data directory, or under
    /// the directory named by <c>XER_APP_DATA</c> when that variable is set.
    /// </summary>
    public static string DefaultFilePath => XmlEditorReloaded.Data.Settings.AppPaths.LogFilePath;

    /// <summary>
    /// Gets the absolute path of the log file.
    /// </summary>
    public string FilePath => _filePath;

    /// <summary>
    /// Appends one entry to the log, writing a session header first when this instance has not written
    /// one yet. A write failure is swallowed: logging must never break the operation being logged.
    /// </summary>
    /// <param name="entry">Entry to write.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entry"/> is null.</exception>
    public void Write(OperationLogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_sync)
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                RotateIfNeeded();

                var builder = new StringBuilder();
                if (!_sessionHeaderWritten)
                {
                    AppendSessionHeader(builder);
                    _sessionHeaderWritten = true;
                }

                AppendEntry(builder, entry);
                File.AppendAllText(_filePath, builder.ToString(), Utf8NoBom);
            }
            catch (Exception exception) when (
                exception is IOException
                    or UnauthorizedAccessException
                    or NotSupportedException
                    or ArgumentException
                    or SecurityException)
            {
                // A log that cannot be written is not a reason to fail the user's operation.
            }
        }
    }

    private void RotateIfNeeded()
    {
        var info = new FileInfo(_filePath);
        if (!info.Exists || info.Length < _maxFileBytes)
        {
            return;
        }

        File.Move(_filePath, _filePath + ".1", overwrite: true);
        _sessionHeaderWritten = false;
    }

    private static void AppendSessionHeader(StringBuilder builder)
    {
        var assembly = Assembly.GetEntryAssembly() ?? typeof(FileOperationLog).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "unknown";

        builder.Append("=== XML Editor Reloaded ").Append(version).Append(" | session started ")
            .Append(DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture))
            .AppendLine(" ===");
        builder.Append("User: ").Append(Environment.UserDomainName).Append('\\')
            .AppendLine(Environment.UserName);
        builder.Append("OS: ").Append(RuntimeInformation.OSDescription).Append(" (")
            .Append(RuntimeInformation.OSArchitecture).AppendLine(")");
        builder.Append("Runtime: ").AppendLine(RuntimeInformation.FrameworkDescription);
        builder.Append("Process: ").Append(Environment.ProcessId.ToString(CultureInfo.InvariantCulture))
            .Append(" (").Append(Environment.Is64BitProcess ? "64-bit" : "32-bit").AppendLine(")");
        builder.Append("Working set: ").Append(ToMegabytes(Environment.WorkingSet)).AppendLine(" MiB");
        builder.Append("Managed memory: ").Append(ToMegabytes(GC.GetTotalMemory(forceFullCollection: false)))
            .AppendLine(" MiB");
        builder.AppendLine();
    }

    private static void AppendEntry(StringBuilder builder, OperationLogEntry entry)
    {
        builder.Append('[')
            .Append(entry.TimestampUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
            .Append("] [")
            .Append(entry.Level.ToString().ToUpperInvariant())
            .Append("] ")
            .Append(entry.Source);
        if (!string.IsNullOrWhiteSpace(entry.ErrorCode))
        {
            builder.Append(" (").Append(entry.ErrorCode).Append(')');
        }

        builder.AppendLine();
        AppendIndented(builder, entry.Message);
        if (!string.IsNullOrWhiteSpace(entry.Detail))
        {
            AppendIndented(builder, entry.Detail!);
        }

        builder.AppendLine();
    }

    private static void AppendIndented(StringBuilder builder, string text)
    {
        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            builder.Append("  ").AppendLine(line);
        }
    }

    private static long ToMegabytes(long bytes) => bytes / (1024 * 1024);
}
