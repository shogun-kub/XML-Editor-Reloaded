namespace XmlEditorReloaded.Core.Diagnostics;

/// <summary>
/// Severity of an operation log entry.
/// </summary>
public enum OperationLogLevel
{
    /// <summary>Informational message about a completed operation.</summary>
    Info,

    /// <summary>A recoverable problem that the user should be able to solve.</summary>
    Warning,

    /// <summary>An operation failed and the failure has no direct user interaction to explain it.</summary>
    Error,

    /// <summary>The application cannot continue, for example an unhandled exception or a failed start-up.</summary>
    Fatal,
}

/// <summary>
/// One entry of the application operation log.
/// </summary>
/// <param name="TimestampUtc">Moment the entry was created, in UTC.</param>
/// <param name="Level">Severity of the entry.</param>
/// <param name="Source">Component that produced the entry, for example <c>TableFileSaver</c>.</param>
/// <param name="Message">Human-readable description of the event.</param>
/// <param name="ErrorCode">Optional stable machine-readable code, for example <c>io.error</c>.</param>
/// <param name="Detail">Optional technical detail such as a full exception including its stack trace.</param>
public sealed record OperationLogEntry(
    DateTimeOffset TimestampUtc,
    OperationLogLevel Level,
    string Source,
    string Message,
    string? ErrorCode = null,
    string? Detail = null);

/// <summary>
/// Sink that persists operation log entries. Implementations must never throw on a write failure:
/// logging must not break the operation that is being logged.
/// </summary>
public interface IOperationLog
{
    /// <summary>
    /// Writes one entry to the sink.
    /// </summary>
    /// <param name="entry">Entry to write.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entry"/> is null.</exception>
    void Write(OperationLogEntry entry);
}

/// <summary>
/// Process-wide operation log facade. The application installs a persistent sink during start-up; when
/// no sink is installed, for example in unit tests, every call is a no-op. A failing sink never throws,
/// so callers can log a failure from an error branch without masking the original result; invalid
/// arguments are still rejected by the usual argument checks.
/// </summary>
public static class OperationLog
{
    private static IOperationLog? _sink;

    /// <summary>
    /// Gets or sets the active sink; <see langword="null"/> disables logging.
    /// </summary>
    public static IOperationLog? Sink
    {
        get => Volatile.Read(ref _sink);
        set => Volatile.Write(ref _sink, value);
    }

    /// <summary>
    /// Gets a value indicating whether a sink is installed.
    /// </summary>
    public static bool IsEnabled => Volatile.Read(ref _sink) is not null;

    /// <summary>
    /// Logs an informational entry.
    /// </summary>
    /// <param name="source">Component that produced the entry.</param>
    /// <param name="message">Human-readable description.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is empty.</exception>
    public static void Info(string source, string message) =>
        Write(OperationLogLevel.Info, source, message);

    /// <summary>
    /// Logs a warning.
    /// </summary>
    /// <param name="source">Component that produced the entry.</param>
    /// <param name="message">Human-readable description.</param>
    /// <param name="errorCode">Optional stable machine-readable code.</param>
    /// <param name="exception">Optional exception that explains the warning.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is empty.</exception>
    public static void Warning(
        string source,
        string message,
        string? errorCode = null,
        Exception? exception = null) =>
        Write(OperationLogLevel.Warning, source, message, errorCode, exception);

    /// <summary>
    /// Logs an error.
    /// </summary>
    /// <param name="source">Component that produced the entry.</param>
    /// <param name="message">Human-readable description.</param>
    /// <param name="errorCode">Optional stable machine-readable code.</param>
    /// <param name="exception">Optional exception that explains the error.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is empty.</exception>
    public static void Error(
        string source,
        string message,
        string? errorCode = null,
        Exception? exception = null) =>
        Write(OperationLogLevel.Error, source, message, errorCode, exception);

    /// <summary>
    /// Logs a fatal error.
    /// </summary>
    /// <param name="source">Component that produced the entry.</param>
    /// <param name="message">Human-readable description.</param>
    /// <param name="errorCode">Optional stable machine-readable code.</param>
    /// <param name="exception">Optional exception that explains the error.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is empty.</exception>
    public static void Fatal(
        string source,
        string message,
        string? errorCode = null,
        Exception? exception = null) =>
        Write(OperationLogLevel.Fatal, source, message, errorCode, exception);

    /// <summary>
    /// Logs one entry when a sink is installed.
    /// </summary>
    /// <param name="level">Severity of the entry.</param>
    /// <param name="source">Component that produced the entry.</param>
    /// <param name="message">Human-readable description.</param>
    /// <param name="errorCode">Optional stable machine-readable code.</param>
    /// <param name="exception">Optional exception; its full text, including the stack trace, is stored as detail.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public static void Write(
        OperationLogLevel level,
        string source,
        string message,
        string? errorCode = null,
        Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentNullException.ThrowIfNull(message);

        var sink = Volatile.Read(ref _sink);
        if (sink is null)
        {
            return;
        }

        try
        {
            sink.Write(new OperationLogEntry(
                DateTimeOffset.UtcNow,
                level,
                source,
                message,
                errorCode,
                exception?.ToString()));
        }
        catch (Exception)
        {
            // A broken log sink must not turn a reported failure into a new unhandled exception.
        }
    }
}
