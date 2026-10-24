using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging.Etrle;

/// <summary>
/// Decodes ETRLE-compressed STI frames into straight RGBA buffers.
/// </summary>
public sealed class EtrleDecoder
{
    /// <summary>
    /// Decodes one frame from a parsed STI image.
    /// </summary>
    /// <param name="image">Parsed STI container.</param>
    /// <param name="frameIndex">Zero-based frame index.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the RGBA frame.
    /// On failure, <c>error_code</c> is <c>image.not_found</c>, <c>sti.truncated</c>, or <c>sti.etrle_overrun</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="image"/> is null.</exception>
    public OperationResult<RgbaBuffer> DecodeFrame(StciImage image, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (frameIndex < 0 || frameIndex >= image.SubImages.Count)
        {
            return OperationResult<RgbaBuffer>.Failure(
                ErrorCodes.ImageNotFound,
                $"Frame {frameIndex} does not exist in '{image.FilePath}'.");
        }

        var descriptor = image.SubImages[frameIndex];
        var frameData = image.FrameData.AsSpan(descriptor.DataOffset, descriptor.DataLength);
        var width = descriptor.Width;
        var height = descriptor.Height;
        var pixels = new byte[checked(width * height * 4)];

        var palette = image.Palette;
        var colorCount = image.Header.NumberOfColors;
        var x = 0;
        var y = 0;
        var index = 0;

        while (index < frameData.Length)
        {
            var control = frameData[index++];
            if (control == 0)
            {
                x = 0;
                y++;
                if (y > height)
                {
                    return Overrun(image, frameIndex, "ETRLE contains more rows than the frame height.");
                }

                continue;
            }

            if ((control & 0x80) != 0)
            {
                var run = control & 0x7F;
                if (x + run > width)
                {
                    return Overrun(image, frameIndex, "A transparent ETRLE run exceeds the frame width.");
                }

                x += run;
                continue;
            }

            var literalRun = control;
            if (x + literalRun > width)
            {
                return Overrun(image, frameIndex, "An ETRLE literal run exceeds the frame width.");
            }

            if (y >= height)
            {
                return Overrun(image, frameIndex, "ETRLE writes past the frame height.");
            }

            for (var offset = 0; offset < literalRun; offset++)
            {
                if (index >= frameData.Length)
                {
                    return OperationResult<RgbaBuffer>.Failure(
                        ErrorCodes.StiTruncated,
                        $"Frame {frameIndex} in '{image.FilePath}' ends inside an ETRLE literal run.");
                }

                var paletteIndex = frameData[index++];
                if (paletteIndex >= colorCount || paletteIndex * 3 + 2 >= palette.Length)
                {
                    return OperationResult<RgbaBuffer>.Failure(
                        ErrorCodes.StiTruncated,
                        $"Frame {frameIndex} in '{image.FilePath}' references palette index {paletteIndex} outside the palette.");
                }

                var pixelOffset = (y * width + x) * 4;
                var paletteOffset = paletteIndex * 3;
                pixels[pixelOffset] = palette[paletteOffset];
                pixels[pixelOffset + 1] = palette[paletteOffset + 1];
                pixels[pixelOffset + 2] = palette[paletteOffset + 2];
                pixels[pixelOffset + 3] = paletteIndex == 0 ? (byte)0 : (byte)255;
                x++;
            }
        }

        return OperationResult<RgbaBuffer>.Success(new RgbaBuffer(width, height, pixels));
    }

    private static OperationResult<RgbaBuffer> Overrun(StciImage image, int frameIndex, string message) =>
        OperationResult<RgbaBuffer>.Failure(
            ErrorCodes.StiEtrleOverrun,
            $"Frame {frameIndex} in '{image.FilePath}': {message}");
}
