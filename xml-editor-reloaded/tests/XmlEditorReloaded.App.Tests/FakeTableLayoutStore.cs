using System.IO;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Layout;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// In-memory <see cref="ITableLayoutStore"/> used by view-model tests.
/// </summary>
internal sealed class FakeTableLayoutStore : ITableLayoutStore
{
    /// <summary>
    /// Gets stored layouts keyed by normalized data-set directory and table name.
    /// </summary>
    public Dictionary<string, TableLayout> Layouts { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the layouts written through <see cref="SaveAsync"/> in call order.
    /// </summary>
    public List<TableLayout> Saved { get; } = new();

    /// <summary>
    /// Gets the number of reset calls.
    /// </summary>
    public int ResetCount { get; private set; }

    /// <inheritdoc />
    public Task<OperationResult<TableLayout>> LoadAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(OperationResult<TableLayout>.Success(
            Layouts.TryGetValue(BuildKey(dataSetDirectory, tableName), out var layout)
                ? layout
                : TableLayout.Empty));

    /// <inheritdoc />
    public Task<OperationResult> SaveAsync(
        string dataSetDirectory,
        string tableName,
        TableLayout layout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(layout);
        Layouts[BuildKey(dataSetDirectory, tableName)] = layout;
        Saved.Add(layout);
        return Task.FromResult(OperationResult.Success());
    }

    /// <inheritdoc />
    public Task<OperationResult> RemoveAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default)
    {
        Layouts.Remove(BuildKey(dataSetDirectory, tableName));
        return Task.FromResult(OperationResult.Success());
    }

    /// <inheritdoc />
    public Task<OperationResult> ResetAllAsync(CancellationToken cancellationToken = default)
    {
        Layouts.Clear();
        ResetCount++;
        return Task.FromResult(OperationResult.Success());
    }

    private static string BuildKey(string dataSetDirectory, string tableName) =>
        Path.GetFullPath(dataSetDirectory).TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar)
        + "\u0000"
        + tableName;
}
