using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Etrle;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging;

/// <summary>
/// Reads STI files and decodes individual frames to RGBA buffers.
/// </summary>
public sealed class StiDecoder
{
    private readonly StciParser _parser;
    private readonly EtrleDecoder _etrleDecoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="StiDecoder"/> class.
    /// </summary>
    /// <param name="parser">Optional STI parser; a default parser is created when omitted.</param>
    /// <param name="etrleDecoder">Optional ETRLE decoder; a default decoder is created when omitted.</param>
    public StiDecoder(StciParser? parser = null, EtrleDecoder? etrleDecoder = null)
    {
        _parser = parser ?? new StciParser();
        _etrleDecoder = etrleDecoder ?? new EtrleDecoder();
    }

    /// <summary>
    /// Reads and parses an STI file.
    /// </summary>
    /// <param name="path">Absolute path of the STI file.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>Parsed STI image or an error with the usual <c>error_code</c>/<c>error_text</c> fields.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public Task<OperationResult<StciImage>> ParseAsync(
        string path,
        CancellationToken cancellationToken = default) =>
        _parser.ParseAsync(path, cancellationToken);

    /// <summary>
    /// Decodes one frame from a parsed STI image.
    /// </summary>
    /// <param name="image">Parsed STI image.</param>
    /// <param name="frameIndex">Zero-based frame index.</param>
    /// <returns>RGBA frame or an error with the usual <c>error_code</c>/<c>error_text</c> fields.</returns>
    public OperationResult<RgbaBuffer> DecodeFrame(StciImage image, int frameIndex) =>
        _etrleDecoder.DecodeFrame(image, frameIndex);

    /// <summary>
    /// Reads an STI file and decodes one frame.
    /// </summary>
    /// <param name="path">Absolute path of the STI file.</param>
    /// <param name="frameIndex">Zero-based frame index.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>RGBA frame or an error with the usual <c>error_code</c>/<c>error_text</c> fields.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<RgbaBuffer>> LoadFrameAsync(
        string path,
        int frameIndex,
        CancellationToken cancellationToken = default)
    {
        var parsed = await _parser.ParseAsync(path, cancellationToken).ConfigureAwait(false);
        return parsed.IsSuccess ? _etrleDecoder.DecodeFrame(parsed.Data!, frameIndex) : OperationResult<RgbaBuffer>.Failure(parsed);
    }
}
