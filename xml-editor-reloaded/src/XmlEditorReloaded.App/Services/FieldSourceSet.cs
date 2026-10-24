using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Lookup values, relation pickers, and relation target key sets for the fields of one loaded table.
/// </summary>
internal sealed class FieldSourceSet
{
    private static readonly IReadOnlyDictionary<string, LookupColumnData> NoLookups =
        new Dictionary<string, LookupColumnData>(StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, RelationColumnData> NoRelations =
        new Dictionary<string, RelationColumnData>(StringComparer.Ordinal);

    private FieldSourceSet(
        IReadOnlyDictionary<string, LookupColumnData> lookups,
        IReadOnlyDictionary<string, RelationColumnData> relations,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? relationTargetValues)
    {
        Lookups = lookups;
        Relations = relations;
        RelationTargetValues = relationTargetValues;
    }

    /// <summary>
    /// Gets lookup data keyed by XML field name.
    /// </summary>
    public IReadOnlyDictionary<string, LookupColumnData> Lookups { get; }

    /// <summary>
    /// Gets relation picker data keyed by XML field name.
    /// </summary>
    public IReadOnlyDictionary<string, RelationColumnData> Relations { get; }

    /// <summary>
    /// Gets the complete target key set of every resolved relation, keyed by source field, or
    /// <see langword="null"/> when no relation resolved.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlySet<string>>? RelationTargetValues { get; }

    /// <summary>
    /// Loads lookup and relation sources for the fields of a workspace.
    /// </summary>
    /// <param name="session">Data-set session that supplies lookups and the relation resolver.</param>
    /// <param name="workspace">Loaded table workspace.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>The loaded sources; failures are represented as unavailable picker data.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public static async Task<FieldSourceSet> LoadAsync(
        IDataSetSession session,
        TableWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        var lookups = await LoadLookupsAsync(session, workspace, cancellationToken).ConfigureAwait(false);
        var relations = await LoadRelationsAsync(session, workspace, cancellationToken).ConfigureAwait(false);
        return new FieldSourceSet(lookups, relations.Relations, relations.TargetValues);
    }

    private static async Task<IReadOnlyDictionary<string, LookupColumnData>> LoadLookupsAsync(
        IDataSetSession session,
        TableWorkspace workspace,
        CancellationToken cancellationToken)
    {
        var columns = workspace.Schema.Columns
            .Where(static column => !string.IsNullOrWhiteSpace(column.LookupTable))
            .ToArray();
        if (columns.Length == 0)
        {
            return NoLookups;
        }

        var valuesByField = columns.ToDictionary(
            static column => column.XmlFieldName,
            static _ => new List<string?>(),
            StringComparer.Ordinal);
        foreach (var row in workspace.GetRows())
        {
            foreach (var column in columns)
            {
                valuesByField[column.XmlFieldName].Add(row.GetValue(column.XmlFieldName));
            }
        }

        var result = new Dictionary<string, LookupColumnData>(StringComparer.Ordinal);
        var cache = new Dictionary<string, OperationResult<IReadOnlyList<LookupEntry>>>(StringComparer.Ordinal);
        foreach (var column in columns)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var lookupTable = column.LookupTable!;
            var tableName = TableName.Parse(lookupTable);
            if (!cache.TryGetValue(lookupTable, out var entries))
            {
                entries = await session.Lookups
                    .GetEntriesAsync(tableName, workspace.Language?.Code, cancellationToken)
                    .ConfigureAwait(false);
                cache.Add(lookupTable, entries);
            }

            result[column.XmlFieldName] = entries.IsSuccess
                ? LookupColumnData.Create(tableName, entries.Data!, valuesByField[column.XmlFieldName])
                : LookupColumnData.Failed(
                    tableName,
                    entries.ErrorCode ?? ErrorCodes.XmlInvalidLookupEntry,
                    entries.ErrorText ?? $"Lookup table '{lookupTable}' could not be loaded.");
        }

        return result;
    }

    private static async Task<(IReadOnlyDictionary<string, RelationColumnData> Relations,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? TargetValues)> LoadRelationsAsync(
        IDataSetSession session,
        TableWorkspace workspace,
        CancellationToken cancellationToken)
    {
        OperationResult<IReadOnlyList<TableRelation>> relations;
        try
        {
            relations = await session
                .GetRelationsAsync(workspace.Descriptor.TableName, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // Relation metadata must never prevent the table from opening.
            return (NoRelations, null);
        }

        if (!relations.IsSuccess)
        {
            return (NoRelations, null);
        }

        var references = relations.Data!
            .Where(static relation => relation.Kind == TableRelationKind.Reference)
            .GroupBy(static relation => relation.SourceField, StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToArray();
        if (references.Length == 0)
        {
            return (NoRelations, null);
        }

        var valuesByField = references.ToDictionary(
            static relation => relation.SourceField,
            static _ => new List<string?>(),
            StringComparer.Ordinal);
        foreach (var row in workspace.GetRows())
        {
            foreach (var relation in references)
            {
                valuesByField[relation.SourceField].Add(row.GetValue(relation.SourceField));
            }
        }

        var result = new Dictionary<string, RelationColumnData>(StringComparer.Ordinal);
        var targetValues = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal);
        foreach (var relation in references)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var target = await session.RelationResolver
                .ResolveAsync(relation, workspace.Language?.Code, cancellationToken)
                .ConfigureAwait(false);
            if (target.IsSuccess)
            {
                result[relation.SourceField] = RelationColumnData.Create(
                    relation,
                    target.Data!,
                    valuesByField[relation.SourceField]);
                targetValues[relation.SourceField] = target.Data!.Entries
                    .Select(static entry => entry.Value)
                    .ToHashSet(StringComparer.Ordinal);
            }
            else
            {
                result[relation.SourceField] = RelationColumnData.Failed(
                    relation,
                    target.ErrorCode ?? ErrorCodes.XmlInvalidLookupEntry,
                    target.ErrorText ?? $"Relation target '{relation.TargetTable}' could not be loaded.");
            }
        }

        return (result, targetValues.Count == 0 ? null : targetValues);
    }
}
