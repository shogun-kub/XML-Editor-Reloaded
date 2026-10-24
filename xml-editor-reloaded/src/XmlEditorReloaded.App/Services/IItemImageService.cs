using System.Windows.Media.Imaging;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Loads decoded item images for display in the editor.
/// </summary>
public interface IItemImageService
{
    /// <summary>
    /// Loads one item image of the requested size.
    /// </summary>
    /// <param name="gameDataDirectory">Absolute path of the <c>Data-1.13</c> directory.</param>
    /// <param name="size">Image size class.</param>
    /// <param name="graphicType">Item graphic type (<c>ubGraphicType</c>); zero means the gun sheet.</param>
    /// <param name="graphicIndex">Graphic index (<c>ubGraphicNum</c>).</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries a frozen bitmap.
    /// On failure, <c>error_code</c> is the code reported by the imaging backend, for example
    /// <c>image.not_found</c>, <c>sti.bad_magic</c>, <c>sti.truncated</c>, or <c>sti.etrle_overrun</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="gameDataDirectory"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<BitmapSource>> GetImageAsync(
        string gameDataDirectory,
        ImageSize size,
        int graphicType,
        int graphicIndex,
        CancellationToken cancellationToken = default);
}
