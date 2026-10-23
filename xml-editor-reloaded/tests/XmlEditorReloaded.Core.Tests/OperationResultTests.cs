using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for <see cref="OperationResult"/> and <see cref="OperationResult{T}"/>.
/// </summary>
public class OperationResultTests
{
    /// <summary>
    /// A successful non-generic result has no error.
    /// </summary>
    [Fact]
    public void OperationResult_Success_HasNoError()
    {
        var result = OperationResult.Success();

        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorCode);
        Assert.Null(result.ErrorText);
    }

    /// <summary>
    /// A failed result carries the error code and text.
    /// </summary>
    [Fact]
    public void OperationResult_Failure_CarriesErrorCodeAndText()
    {
        var result = OperationResult.Failure(ErrorCodes.XmlParseError, "Bad XML.");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlParseError, result.ErrorCode);
        Assert.Equal("Bad XML.", result.ErrorText);
    }

    /// <summary>
    /// A successful generic result carries its data payload.
    /// </summary>
    [Fact]
    public void OperationResult_Generic_Success_CarriesData()
    {
        var result = OperationResult<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Data);
        Assert.Null(result.ErrorCode);
    }

    /// <summary>
    /// A failed generic result has no data and copies error information from another failed result.
    /// </summary>
    [Fact]
    public void OperationResult_Generic_Failure_CopiesErrorFromOther()
    {
        var source = OperationResult.Failure(ErrorCodes.MetadataNotFound, "Table missing.");
        var result = OperationResult<string>.Failure(source);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorCodes.MetadataNotFound, result.ErrorCode);
        Assert.Equal("Table missing.", result.ErrorText);
    }
}
