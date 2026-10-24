using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Imaging.Etrle;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests for <see cref="EtrleDecoder"/>.
/// </summary>
public class EtrleDecoderTests
{
    /// <summary>
    /// A real big-item frame decodes with the expected dimensions, transparency, and palette color.
    /// </summary>
    [Fact]
    public async Task Decode_BigItemFrame_ProducesRgbaPixels()
    {
        var parsed = await new StciParser().ParseAsync(TestWorkspace.FixturePath("Graphics/BigItems/P2ITEM35.STI"));
        var image = parsed.Data!;

        var decoded = new EtrleDecoder().DecodeFrame(image, 0);

        Assert.True(decoded.IsSuccess, decoded.ErrorText);
        var buffer = decoded.Data!;
        Assert.Equal(39, buffer.Width);
        Assert.Equal(42, buffer.Height);
        Assert.Equal(0, buffer.Pixels[3]);
        Assert.Equal(140, buffer.Pixels[96]);
        Assert.Equal(122, buffer.Pixels[97]);
        Assert.Equal(122, buffer.Pixels[98]);
        Assert.Equal(255, buffer.Pixels[99]);
    }

    /// <summary>
    /// Medium and small sprite sheets decode their last frame without overruns.
    /// </summary>
    [Theory]
    [InlineData("Graphics/Interface/MDP3ITEMS.STI")]
    [InlineData("Graphics/tilesets/0/SMP3ITEMS.STI")]
    public async Task Decode_SpriteSheetLastFrame_Succeeds(string fixture)
    {
        var parsed = await new StciParser().ParseAsync(TestWorkspace.FixturePath(fixture));
        Assert.True(parsed.IsSuccess, parsed.ErrorText);
        var image = parsed.Data!;
        var last = image.SubImages.Count - 1;

        var decoded = new EtrleDecoder().DecodeFrame(image, last);

        Assert.True(decoded.IsSuccess, decoded.ErrorText);
        Assert.Equal(image.SubImages[last].Width, decoded.Data!.Width);
        Assert.Equal(image.SubImages[last].Height, decoded.Data.Height);
    }

    /// <summary>
    /// An out-of-range frame index is reported as missing.
    /// </summary>
    [Fact]
    public async Task Decode_InvalidFrame_ReturnsImageNotFound()
    {
        var parsed = await new StciParser().ParseAsync(TestWorkspace.FixturePath("Graphics/BigItems/P2ITEM35.STI"));

        var decoded = new EtrleDecoder().DecodeFrame(parsed.Data!, 1);

        Assert.False(decoded.IsSuccess);
        Assert.Equal(ErrorCodes.ImageNotFound, decoded.ErrorCode);
    }

    /// <summary>
    /// A literal run wider than the frame is reported as an ETRLE overrun.
    /// </summary>
    [Fact]
    public async Task Decode_OverlongLiteralRun_ReturnsOverrun()
    {
        using var workspace = new TestWorkspace();
        var bytes = await File.ReadAllBytesAsync(TestWorkspace.FixturePath("Graphics/BigItems/P2ITEM35.STI"));
        bytes[849] = 0x7F; // frame data starts at 64 + 256*3 + 16 = 848; second byte is a literal run.
        var path = Path.Combine(workspace.DirectoryPath, "overrun.sti");
        await File.WriteAllBytesAsync(path, bytes);
        var parsed = await new StciParser().ParseAsync(path);

        var decoded = new EtrleDecoder().DecodeFrame(parsed.Data!, 0);

        Assert.False(decoded.IsSuccess);
        Assert.Equal(ErrorCodes.StiEtrleOverrun, decoded.ErrorCode);
    }
}
