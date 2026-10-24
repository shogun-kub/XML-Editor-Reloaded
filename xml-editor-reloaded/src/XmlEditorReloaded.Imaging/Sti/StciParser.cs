using System.Buffers.Binary;
using System.Text;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging.Sti;

/// <summary>
/// Parses STI/STCI containers into a header, palette, frame directory, and raw frame data.
/// </summary>
public sealed class StciParser
{
    private const int HeaderSize = 64;
    private const int IdLength = 4;
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("STCI");

    /// <summary>
    /// Reads and parses an STI file.
    /// </summary>
    /// <param name="path">Absolute path of the STI file.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the parsed image.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>io.access_denied</c>, <c>io.error</c>,
    /// <c>sti.bad_magic</c>, <c>sti.truncated</c>, or <c>sti.unsupported_flags</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<StciImage>> ParseAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        byte[] bytes;
        try
        {
            bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The STI file '{path}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The directory of STI file '{path}' does not exist.");
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the STI file '{path}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.IoError,
                $"The STI file '{path}' could not be read: {exception.Message}");
        }

        return Parse(bytes, path);
    }

    /// <summary>
    /// Parses STI bytes that have already been read from disk.
    /// </summary>
    /// <param name="bytes">Complete STI file content.</param>
    /// <param name="filePath">Source path used for diagnostics and image identity.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the parsed image.
    /// On failure, <c>error_code</c> is <c>sti.bad_magic</c>, <c>sti.truncated</c>, or <c>sti.unsupported_flags</c>.
    /// </returns>
    public OperationResult<StciImage> Parse(byte[] bytes, string filePath)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(filePath);

        if (bytes.Length < HeaderSize)
        {
            return Truncated(filePath, $"The file is shorter than the {HeaderSize}-byte STI header.");
        }

        if (!bytes.AsSpan(0, IdLength).SequenceEqual(Magic))
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.StiBadMagic,
                $"File '{filePath}' does not start with the STCI magic.");
        }

        var originalSize = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(4));
        var storedSize = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(8));
        var transparentValue = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(12));
        var flags = (StciFlags)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(16));
        var height = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(20));
        var width = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(22));
        var numberOfColors = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(24));
        var numberOfSubImages = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(28));
        var depth = bytes[30];
        var redDepth = bytes[31];
        var greenDepth = bytes[32];
        var blueDepth = bytes[33];
        var appDataSize = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(48));

        const StciFlags supportedFlags = StciFlags.Supported;
        if ((flags & supportedFlags) != supportedFlags || (flags & ~supportedFlags) != 0)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.StiUnsupportedFlags,
                $"File '{filePath}' uses unsupported STI flags 0x{(uint)flags:X8}.");
        }

        if (numberOfColors == 0)
        {
            return OperationResult<StciImage>.Failure(
                ErrorCodes.StiUnsupportedFlags,
                $"File '{filePath}' is an RGB STI; only indexed item images are supported.");
        }

        if (numberOfColors > int.MaxValue || numberOfSubImages == 0)
        {
            return Truncated(filePath, "The STI header contains an invalid palette or frame count.");
        }

        var paletteLength = checked((int)numberOfColors * 3);
        var directoryLength = checked((int)numberOfSubImages * 16);
        var dataStart = checked(HeaderSize + paletteLength + directoryLength);
        if (dataStart > bytes.Length)
        {
            return Truncated(filePath, "The palette or frame directory extends past the end of the file.");
        }

        var palette = bytes.AsSpan(HeaderSize, paletteLength).ToArray();
        var subImages = new SubImageDescriptor[numberOfSubImages];
        var directoryOffset = HeaderSize + paletteLength;
        var frameDataLength = bytes.Length - dataStart;

        for (var index = 0; index < numberOfSubImages; index++)
        {
            var offset = directoryOffset + index * 16;
            var dataOffset = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(offset));
            var dataLength = BinaryPrimitives.ReadInt32LittleEndian(bytes.AsSpan(offset + 4));
            var offsetX = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan(offset + 8));
            var offsetY = BinaryPrimitives.ReadInt16LittleEndian(bytes.AsSpan(offset + 10));
            var frameHeight = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset + 12));
            var frameWidth = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset + 14));

            if (dataOffset < 0 || dataLength < 0
                || dataOffset > frameDataLength
                || dataLength > frameDataLength - dataOffset)
            {
                return Truncated(filePath, $"Frame directory entry {index} points outside the frame data.");
            }

            if (frameWidth == 0 || frameHeight == 0)
            {
                return Truncated(filePath, $"Frame {index} has zero width or height.");
            }

            subImages[index] = new SubImageDescriptor(
                dataOffset,
                dataLength,
                offsetX,
                offsetY,
                frameHeight,
                frameWidth);
        }

        var frameData = bytes.AsSpan(dataStart, frameDataLength).ToArray();
        var header = new StciHeader(
            originalSize,
            storedSize,
            transparentValue,
            flags,
            height,
            width,
            (int)numberOfColors,
            numberOfSubImages,
            depth,
            redDepth,
            greenDepth,
            blueDepth,
            appDataSize);

        return OperationResult<StciImage>.Success(
            new StciImage(filePath, header, palette, subImages, frameData));
    }

    private static OperationResult<StciImage> Truncated(string filePath, string message) =>
        OperationResult<StciImage>.Failure(
            ErrorCodes.StiTruncated,
            $"File '{filePath}': {message}");
}
