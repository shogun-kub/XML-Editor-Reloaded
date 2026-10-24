using System.Text;
using System.Text.RegularExpressions;

namespace XmlEditorReloaded.Xml.IO;

/// <summary>
/// Describes the on-disk format of an XML table file: text encoding, byte-order mark presence,
/// newline style, and whether the file starts with an XML declaration.
/// </summary>
public sealed class FileFormatInfo
{
    private static readonly Regex DeclarationEncodingPattern = new(
        @"<\?xml[^>]*\bencoding\s*=\s*[""'](?<encoding>[^""']+)[""']",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private FileFormatInfo(string encodingName, bool hasByteOrderMark, string newLine, bool hasXmlDeclaration)
    {
        EncodingName = encodingName;
        HasByteOrderMark = hasByteOrderMark;
        NewLine = newLine;
        HasXmlDeclaration = hasXmlDeclaration;
    }

    /// <summary>
    /// Gets the encoding name, for example <c>utf-8</c>. The value comes from the XML declaration when present,
    /// otherwise from the byte-order mark, otherwise it defaults to UTF-8.
    /// </summary>
    public string EncodingName { get; }

    /// <summary>
    /// Gets a value indicating whether the file starts with a byte-order mark.
    /// </summary>
    public bool HasByteOrderMark { get; }

    /// <summary>
    /// Gets the dominant newline sequence found in the file: <c>\r\n</c>, <c>\n</c>, or <c>\r</c>.
    /// </summary>
    public string NewLine { get; }

    /// <summary>
    /// Gets a value indicating whether the file starts with an XML declaration.
    /// </summary>
    public bool HasXmlDeclaration { get; }

    /// <summary>
    /// Creates an <see cref="Encoding"/> instance matching the detected name and BOM setting.
    /// </summary>
    /// <returns>
    /// A UTF-8, UTF-16, or UTF-32 encoding with the original BOM preference, or UTF-8 without BOM
    /// when the declared encoding is not available in the runtime.
    /// </returns>
    public Encoding CreateEncoding()
    {
        try
        {
            return EncodingName.ToLowerInvariant() switch
            {
                "utf-8" or "utf8" => new UTF8Encoding(HasByteOrderMark),
                "utf-16" or "unicode" => new UnicodeEncoding(bigEndian: false, byteOrderMark: HasByteOrderMark),
                "utf-16be" => new UnicodeEncoding(bigEndian: true, byteOrderMark: HasByteOrderMark),
                "utf-32" => new UTF32Encoding(bigEndian: false, byteOrderMark: HasByteOrderMark),
                "utf-32be" => new UTF32Encoding(bigEndian: true, byteOrderMark: HasByteOrderMark),
                _ => Encoding.GetEncoding(EncodingName),
            };
        }
        catch (ArgumentException)
        {
            return new UTF8Encoding(HasByteOrderMark);
        }
    }

    /// <summary>
    /// Detects the format of an already loaded XML file.
    /// </summary>
    /// <param name="bytes">The complete file content; only the leading probe is inspected.</param>
    /// <returns>Detected encoding, BOM, newline, and declaration information.</returns>
    public static FileFormatInfo Detect(ReadOnlySpan<byte> bytes)
    {
        var hasBom = false;
        Encoding? bomEncoding = null;

        if (bytes.StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }))
        {
            hasBom = true;
            bomEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        }
        else if (bytes.StartsWith(new byte[] { 0xFF, 0xFE, 0x00, 0x00 }))
        {
            hasBom = true;
            bomEncoding = new UTF32Encoding(bigEndian: false, byteOrderMark: true);
        }
        else if (bytes.StartsWith(new byte[] { 0x00, 0x00, 0xFE, 0xFF }))
        {
            hasBom = true;
            bomEncoding = new UTF32Encoding(bigEndian: true, byteOrderMark: true);
        }
        else if (bytes.StartsWith(new byte[] { 0xFF, 0xFE }))
        {
            hasBom = true;
            bomEncoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
        }
        else if (bytes.StartsWith(new byte[] { 0xFE, 0xFF }))
        {
            hasBom = true;
            bomEncoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: true);
        }

        var probe = bytes.Length > 4096 ? bytes[..4096] : bytes;
        var probeText = (bomEncoding ?? Encoding.UTF8).GetString(probe).TrimStart('\uFEFF');
        var declarationMatch = DeclarationEncodingPattern.Match(probeText);

        var encodingName = declarationMatch.Success
            ? declarationMatch.Groups["encoding"].Value
            : bomEncoding?.WebName ?? "utf-8";

        var newLine = probeText.Contains("\r\n", StringComparison.Ordinal)
            ? "\r\n"
            : probeText.Contains('\n')
                ? "\n"
                : probeText.Contains('\r')
                    ? "\r"
                    : "\n";

        var hasDeclaration = probeText.TrimStart().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase);

        return new FileFormatInfo(encodingName, hasBom, newLine, hasDeclaration);
    }

    /// <summary>
    /// Reads the beginning of a file and detects its XML text format.
    /// </summary>
    /// <param name="path">Absolute path of the XML file.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>Detected encoding, BOM, newline, and declaration information.</returns>
    /// <exception cref="IOException">Thrown when the file cannot be read.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public static async Task<FileFormatInfo> DetectAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        var buffer = new byte[4096];
        var read = await stream.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
        return Detect(buffer.AsSpan(0, read));
    }
}
