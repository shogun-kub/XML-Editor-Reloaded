using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests for <see cref="StciParser"/>.
/// </summary>
public class StciParserTests
{
    /// <summary>
    /// A real big-item STI exposes the canvas header, palette, frame directory, and frame data.
    /// </summary>
    [Fact]
    public async Task Parse_BigItem_ReadsHeaderAndFrame()
    {
        var parsed = await new StciParser().ParseAsync(TestWorkspace.FixturePath("Graphics/BigItems/P2ITEM35.STI"));

        Assert.True(parsed.IsSuccess, parsed.ErrorText);
        var image = parsed.Data!;
        Assert.Equal(640, image.Header.Width);
        Assert.Equal(480, image.Header.Height);
        Assert.Equal(256, image.Header.NumberOfColors);
        Assert.Equal(1, image.Header.NumberOfSubImages);
        Assert.Equal(8, image.Header.Depth);
        Assert.Equal(StciFlags.Supported, image.Header.Flags);
        Assert.Equal(768, image.Palette.Length);
        Assert.Equal(242, image.FrameData.Length);

        var frame = Assert.Single(image.SubImages);
        Assert.Equal(39, frame.Width);
        Assert.Equal(42, frame.Height);
        Assert.Equal(12, frame.OffsetX);
        Assert.Equal(3, frame.OffsetY);
    }

    /// <summary>
    /// A file with a wrong magic is rejected.
    /// </summary>
    [Fact]
    public async Task Parse_BadMagic_ReturnsBadMagic()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "bad.sti");
        await File.WriteAllBytesAsync(path, new byte[64]);

        var parsed = await new StciParser().ParseAsync(path);

        Assert.False(parsed.IsSuccess);
        Assert.Equal(ErrorCodes.StiBadMagic, parsed.ErrorCode);
    }

    /// <summary>
    /// A file shorter than the header is rejected as truncated.
    /// </summary>
    [Fact]
    public async Task Parse_ShortFile_ReturnsTruncated()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "short.sti");
        await File.WriteAllBytesAsync(path, new byte[32]);

        var parsed = await new StciParser().ParseAsync(path);

        Assert.False(parsed.IsSuccess);
        Assert.Equal(ErrorCodes.StiTruncated, parsed.ErrorCode);
    }

    /// <summary>
    /// Unsupported flag combinations are rejected.
    /// </summary>
    [Fact]
    public async Task Parse_UnsupportedFlags_ReturnsUnsupported()
    {
        using var workspace = new TestWorkspace();
        var bytes = await File.ReadAllBytesAsync(TestWorkspace.FixturePath("Graphics/BigItems/P2ITEM35.STI"));
        bytes[16] = 0x20;
        var path = Path.Combine(workspace.DirectoryPath, "flags.sti");
        await File.WriteAllBytesAsync(path, bytes);

        var parsed = await new StciParser().ParseAsync(path);

        Assert.False(parsed.IsSuccess);
        Assert.Equal(ErrorCodes.StiUnsupportedFlags, parsed.ErrorCode);
    }

    /// <summary>
    /// A missing file is reported as an I/O failure.
    /// </summary>
    [Fact]
    public async Task Parse_MissingFile_ReturnsFileNotFound()
    {
        var parsed = await new StciParser().ParseAsync(
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.sti"));

        Assert.False(parsed.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, parsed.ErrorCode);
    }
}
