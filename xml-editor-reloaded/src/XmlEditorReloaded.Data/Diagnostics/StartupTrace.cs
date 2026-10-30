using System.Globalization;
using System.Security;
using System.Text;

namespace XmlEditorReloaded.Data.Diagnostics;

/// <summary>
/// Append-only bootstrap trace written to the temporary directory before the operation log and the
/// dependency-injection container exist. It makes a failure during early start-up visible even when
/// the application-data log cannot be created or written. A write failure is swallowed; the trace is
/// diagnostics only and must never prevent the application from starting.
/// </summary>
public sealed class StartupTrace
{
    /// <summary>
    /// Default maximum size of the trace file before it is recreated; 64 KiB.
    /// </summary>
    public const long DefaultMaxFileBytes = 64 * 1024;

    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private readonly object _sync = new();
    private readonly string _filePath;
    private readonly long _maxFileBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupTrace"/> class.
    /// </summary>
    /// <param name="filePath">
    /// Absolute path of the trace file, or <see langword="null"/> to use <see cref="DefaultFilePath"/>.
    /// </param>
    /// <param name="maxFileBytes">Maximum size of the file before it is recreated; must be positive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxFileBytes"/> is not positive.</exception>
    public StartupTrace(string? filePath = null, long maxFileBytes = DefaultMaxFileBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxFileBytes);
        _filePath = string.IsNullOrWhiteSpace(filePath) ? DefaultFilePath : filePath;
        _maxFileBytes = maxFileBytes;
        RecreateIfOversized();
    }

    /// <summary>
    /// Gets the default trace file path in the current user's temporary directory.
    /// </summary>
    public static string DefaultFilePath => Path.Combine(
        Path.GetTempPath(),
        "XmlEditorReloaded-bootstrap.log");

    /// <summary>
    /// Gets the absolute path of the trace file.
    /// </summary>
    public string FilePath => _filePath;

    /// <summary>
    /// Gets the message of the last failed write, or <see langword="null"/> when the last write
    /// succeeded.
    /// </summary>
    public string? LastError { get; private set; }

    /// <summary>
    /// Appends one timestamped line to the trace. Line breaks are flattened so every entry stays on
    /// one line.
    /// </summary>
    /// <param name="message">Text to record.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public void Write(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        lock (_sync)
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var entry = new StringBuilder()
                    .Append('[')
                    .Append(DateTimeOffset.Now.ToString("O", CultureInfo.InvariantCulture))
                    .Append("] ")
                    .Append(message.Replace("\r\n", " ", StringComparison.Ordinal).Replace('\n', ' '))
                    .AppendLine()
                    .ToString();
                File.AppendAllText(_filePath, entry, Utf8NoBom);
                LastError = null;
            }
            catch (Exception exception) when (
                exception is IOException
                    or UnauthorizedAccessException
                    or NotSupportedException
                    or ArgumentException
                    or SecurityException)
            {
                LastError = exception.Message;
            }
        }
    }

    private void RecreateIfOversized()
    {
        try
        {
            var info = new FileInfo(_filePath);
            if (info.Exists && info.Length > _maxFileBytes)
            {
                info.Delete();
            }
        }
        catch (Exception exception) when (
            exception is IOException
                or UnauthorizedAccessException
                or NotSupportedException
                or ArgumentException
                or SecurityException)
        {
            // The next write reports the failure through LastError.
        }
    }
}
