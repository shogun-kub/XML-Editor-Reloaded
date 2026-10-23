namespace XmlEditorReloaded.Core.Api;

/// <summary>
/// Standard operation result carrying either success or an error code with a human-readable text.
/// </summary>
public class OperationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class representing success.
    /// </summary>
    protected OperationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class representing failure.
    /// </summary>
    /// <param name="errorCode">Stable machine-readable error identifier, for example <c>xml.parse_error</c>.</param>
    /// <param name="errorText">Human-readable description of the failure.</param>
    protected OperationResult(string errorCode, string errorText)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorCode);
        ArgumentException.ThrowIfNullOrEmpty(errorText);
        ErrorCode = errorCode;
        ErrorText = errorText;
    }

    /// <summary>
    /// Gets the machine-readable error code, or <see langword="null"/> when the operation succeeded.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the human-readable error description, or <see langword="null"/> when the operation succeeded.
    /// </summary>
    public string? ErrorText { get; }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => ErrorCode is null;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A result with no error.</returns>
    public static OperationResult Success() => new();

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorCode">Stable machine-readable error identifier.</param>
    /// <param name="errorText">Human-readable description of the failure.</param>
    /// <returns>A failed result carrying the given error.</returns>
    public static OperationResult Failure(string errorCode, string errorText) => new(errorCode, errorText);

    /// <summary>
    /// Creates a failed result using another result's error information.
    /// </summary>
    /// <param name="other">A failed result to copy the error from.</param>
    /// <returns>A failed result with the same error code and text.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="other"/> is successful.</exception>
    public static OperationResult Failure(OperationResult other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a failure from a successful result.");
        }

        return new OperationResult(other.ErrorCode!, other.ErrorText!);
    }
}

/// <summary>
/// Standard operation result carrying a data payload on success or an error code on failure.
/// </summary>
/// <typeparam name="T">Type of the success payload.</typeparam>
public sealed class OperationResult<T> : OperationResult
{
    private OperationResult()
    {
    }

    private OperationResult(string errorCode, string errorText)
        : base(errorCode, errorText)
    {
    }

    private OperationResult(T data)
    {
        ArgumentNullException.ThrowIfNull(data);
        Data = data;
    }

    /// <summary>
    /// Gets the success payload, or <see langword="null"/> when the operation failed.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Creates a successful result carrying <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The success payload; must not be <see langword="null"/>.</param>
    /// <returns>A successful result carrying the payload.</returns>
    public static OperationResult<T> Success(T data) => new(data);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorCode">Stable machine-readable error identifier.</param>
    /// <param name="errorText">Human-readable description of the failure.</param>
    /// <returns>A failed result carrying the given error.</returns>
    public new static OperationResult<T> Failure(string errorCode, string errorText) => new(errorCode, errorText);

    /// <summary>
    /// Creates a failed result using another result's error information.
    /// </summary>
    /// <param name="other">A failed result to copy the error from.</param>
    /// <returns>A failed result with the same error code and text.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="other"/> is successful.</exception>
    public new static OperationResult<T> Failure(OperationResult other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (other.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a failure from a successful result.");
        }

        return new OperationResult<T>(other.ErrorCode!, other.ErrorText!);
    }
}
