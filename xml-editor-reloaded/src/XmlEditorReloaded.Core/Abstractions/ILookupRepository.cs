using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// A single id/name pair from a lookup table.
/// </summary>
/// <param name="Id">Numeric identifier.</param>
/// <param name="Name">Display name.</param>
public sealed record LookupEntry(int Id, string Name);

/// <summary>
/// Provides access to lookup tables under <c>TableData/Lookup</c>, including language overlays.
/// </summary>
public interface ILookupRepository
{
    /// <summary>
    /// Reads the entries of a lookup table.
    /// </summary>
    /// <param name="lookupName">Dotted lookup table name, for example <c>Lookup.ItemClass</c>.</param>
    /// <param name="languageCode">
    /// Optional language code whose overlay should be applied on top of the base values;
    /// <see langword="null"/> returns base values only.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the entries in file order;
    /// on failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>, or <c>io.error</c>.
    /// </returns>
    Task<OperationResult<IReadOnlyList<LookupEntry>>> GetEntriesAsync(
        Metadata.TableName lookupName,
        string? languageCode = null,
        CancellationToken cancellationToken = default);
}
