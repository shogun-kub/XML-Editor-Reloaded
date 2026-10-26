using Microsoft.Data.Sqlite;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Data.Metadata;

/// <summary>
/// SQLite-backed implementation of <see cref="ITableMetadataProvider"/>.
/// </summary>
public sealed class SqliteMetadataProvider : ITableMetadataProvider
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteMetadataProvider"/> class.
    /// </summary>
    /// <param name="databasePath">Absolute path of the migrated metadata database.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="databasePath"/> is empty.</exception>
    public SqliteMetadataProvider(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWrite,
            ForeignKeys = true,
        }.ToString();
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<TableMetadata>> GetTableAsync(
        TableName tableName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var metadata = await ReadTableAsync(connection, tableName, cancellationToken).ConfigureAwait(false);
            return metadata is null
                ? OperationResult<TableMetadata>.Failure(
                    ErrorCodes.MetadataNotFound,
                    $"Table '{tableName}' is not present in the metadata database.")
                : OperationResult<TableMetadata>.Success(metadata);
        }
        catch (SqliteException exception)
        {
            return OperationResult<TableMetadata>.Failure(
                ErrorCodes.IoError,
                $"Metadata for table '{tableName}' could not be read: {exception.Message}");
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var tables = new List<TableMetadata>();
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT Id, XmlTable, DtoName, IsLookup, XmlRoot, RecordElement
                FROM Tables
                ORDER BY XmlTable;
                """;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            var rows = new List<TableRow>();
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                rows.Add(ReadTableRow(reader));
            }

            await reader.DisposeAsync().ConfigureAwait(false);

            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await ReadTableDetailsAsync(connection, row, cancellationToken).ConfigureAwait(false);
                tables.Add(metadata);
            }

            return OperationResult<IReadOnlyList<TableMetadata>>.Success(tables);
        }
        catch (SqliteException exception)
        {
            return OperationResult<IReadOnlyList<TableMetadata>>.Failure(
                ErrorCodes.IoError,
                $"Table metadata could not be read: {exception.Message}");
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var languages = new List<LanguageDescriptor>();
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT Code, DisplayName, OverlayPrefix, Ordinal
                FROM Languages
                ORDER BY Ordinal, Code;
                """;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                languages.Add(new LanguageDescriptor(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3)));
            }

            return OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(languages);
        }
        catch (SqliteException exception)
        {
            return OperationResult<IReadOnlyList<LanguageDescriptor>>.Failure(
                ErrorCodes.IoError,
                $"Language metadata could not be read: {exception.Message}");
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
        TableName sourceTable,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var relations = new List<TableRelation>();
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT s.XmlTable, r.SourceField, t.XmlTable, r.TargetValueField, r.TargetDisplayField,
                       r.Kind, r.FilterField, r.FilterOperator, r.FilterValue, r.LabelKey,
                       r.RemapByText, r.Ordinal, r.IncludeKeyZero, r.NoneValue, r.NoneDisplay
                FROM TableRelations r
                INNER JOIN Tables s ON s.Id = r.SourceTableId
                INNER JOIN Tables t ON t.Id = r.TargetTableId
                WHERE s.XmlTable = $name
                ORDER BY r.Ordinal, r.SourceField, r.Id;
                """;
            command.Parameters.AddWithValue("$name", sourceTable.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (!Enum.TryParse<TableRelationKind>(reader.GetString(5), ignoreCase: true, out var kind))
                {
                    continue;
                }

                RelationFilter? filter = null;
                if (!reader.IsDBNull(6)
                    && !reader.IsDBNull(7)
                    && !reader.IsDBNull(8)
                    && Enum.TryParse<RelationFilterOperator>(reader.GetString(7), ignoreCase: true, out var filterOperator))
                {
                    filter = new RelationFilter(reader.GetString(6), filterOperator, reader.GetString(8));
                }

                TableName sourceName;
                TableName targetName;
                try
                {
                    sourceName = TableName.Parse(reader.GetString(0));
                    targetName = TableName.Parse(reader.GetString(2));
                }
                catch (ArgumentException)
                {
                    // A relation row with an unusable table name is metadata corruption; skip the row
                    // rather than failing the whole table.
                    continue;
                }

                relations.Add(new TableRelation(
                    sourceName,
                    reader.GetString(1),
                    targetName,
                    kind,
                    reader.GetString(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    filter)
                {
                    LabelKey = reader.IsDBNull(9) ? null : reader.GetString(9),
                    RemapByText = reader.GetInt32(10) != 0,
                    Ordinal = reader.GetInt32(11),
                    IncludeKeyZero = reader.GetInt32(12) != 0,
                    NoneValue = reader.IsDBNull(13) ? null : reader.GetString(13),
                    NoneDisplay = reader.IsDBNull(14) ? null : reader.GetString(14),
                });
            }

            return OperationResult<IReadOnlyList<TableRelation>>.Success(relations);
        }
        catch (SqliteException exception)
        {
            return OperationResult<IReadOnlyList<TableRelation>>.Failure(
                ErrorCodes.IoError,
                $"Relations for table '{sourceTable}' could not be read: {exception.Message}");
        }
    }

    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<IReadOnlyList<LookupEntry>>> GetLookupValuesAsync(
        TableName lookupName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var entries = new List<LookupEntry>();
            await using var command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT Value, DisplayName
                FROM LookupValues
                WHERE LookupName = $name
                ORDER BY Ordinal, Value;
                """;
            command.Parameters.AddWithValue("$name", lookupName.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                entries.Add(new LookupEntry((ulong)reader.GetInt64(0), reader.GetString(1)));
            }

            return OperationResult<IReadOnlyList<LookupEntry>>.Success(entries);
        }
        catch (SqliteException exception)
        {
            return OperationResult<IReadOnlyList<LookupEntry>>.Failure(
                ErrorCodes.IoError,
                $"Lookup values for '{lookupName}' could not be read: {exception.Message}");
        }
    }

    private static async Task<TableMetadata?> ReadTableAsync(
        SqliteConnection connection,
        TableName tableName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, XmlTable, DtoName, IsLookup, XmlRoot, RecordElement
            FROM Tables
            WHERE XmlTable = $name;
            """;
        command.Parameters.AddWithValue("$name", tableName.Value);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var row = ReadTableRow(reader);
        await reader.DisposeAsync().ConfigureAwait(false);
        return await ReadTableDetailsAsync(connection, row, cancellationToken).ConfigureAwait(false);
    }

    private static TableRow ReadTableRow(SqliteDataReader reader) => new(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.IsDBNull(2) ? null : reader.GetString(2),
        reader.GetInt32(3) != 0,
        reader.GetString(4),
        reader.GetString(5));

    private static async Task<TableMetadata> ReadTableDetailsAsync(
        SqliteConnection connection,
        TableRow row,
        CancellationToken cancellationToken)
    {
        var recordKey = await ReadRecordKeyAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var columns = await ReadColumnsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var listFields = await ReadListFieldsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var dictFields = await ReadDictFieldsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var translatableFields = await ReadTranslatableFieldsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var columnHints = await ReadColumnHintsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var recordKeyFields = await ReadRecordKeyFieldsAsync(connection, row.Id, cancellationToken).ConfigureAwait(false);
        var visibilityRules = await ReadClassVisibilityRulesAsync(connection, row.Id, cancellationToken)
            .ConfigureAwait(false);
        var symmetricKeyPair = await ReadSymmetricKeyPairAsync(connection, row.Id, cancellationToken)
            .ConfigureAwait(false);
        var exclusiveFlagGroups = await ReadExclusiveFlagGroupsAsync(connection, row.Id, cancellationToken)
            .ConfigureAwait(false);
        var classValueLimits = await ReadClassValueLimitsAsync(connection, row.Id, cancellationToken)
            .ConfigureAwait(false);
        var textLimits = await ReadColumnTextLimitsAsync(connection, row.Id, cancellationToken)
            .ConfigureAwait(false);

        return new TableMetadata(
            TableName.Parse(row.XmlTable),
            row.XmlRoot,
            row.RecordElement,
            row.IsLookup,
            recordKey,
            columns,
            listFields,
            dictFields,
            translatableFields,
            columnHints,
            recordKeyFields,
            visibilityRules,
            symmetricKeyPair,
            exclusiveFlagGroups,
            classValueLimits,
            textLimits);
    }

    private static async Task<IReadOnlyList<ColumnTextLimit>> ReadColumnTextLimitsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var limits = new List<ColumnTextLimit>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldName, LanguageCode, MaxLength, ShowCounter
            FROM ColumnTextLimits
            WHERE TableId = $tableId
            ORDER BY XmlFieldName, LanguageCode;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            limits.Add(new ColumnTextLimit(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetInt32(3) != 0));
        }

        return limits;
    }

    private static async Task<IReadOnlyList<ExclusiveFlagGroup>> ReadExclusiveFlagGroupsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var groups = new List<ExclusiveFlagGroup>();
        var fieldsByGroup = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var order = new List<string>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT GroupKey, XmlFieldName
            FROM ExclusiveFlagGroups
            WHERE TableId = $tableId
            ORDER BY GroupKey, Ordinal, XmlFieldName;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var groupKey = reader.GetString(0);
            if (!fieldsByGroup.TryGetValue(groupKey, out var fields))
            {
                fields = new List<string>();
                fieldsByGroup.Add(groupKey, fields);
                order.Add(groupKey);
            }

            fields.Add(reader.GetString(1));
        }

        foreach (var groupKey in order)
        {
            groups.Add(new ExclusiveFlagGroup(groupKey, fieldsByGroup[groupKey]));
        }

        return groups;
    }

    private static async Task<IReadOnlyList<ClassValueLimit>> ReadClassValueLimitsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var limits = new List<ClassValueLimit>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT ClassField, ClassValue, FieldMask, MaxOrdinal
            FROM ClassValueLimits
            WHERE TableId = $tableId
            ORDER BY ClassField, MaxOrdinal, ClassValue;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            limits.Add(new ClassValueLimit(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return limits;
    }

    private static async Task<SymmetricKeyPair?> ReadSymmetricKeyPairAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT FirstField, SecondField
            FROM SymmetricKeyPairs
            WHERE TableId = $tableId;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false)
            ? new SymmetricKeyPair(reader.GetString(0), reader.GetString(1))
            : null;
    }

    private static async Task<IReadOnlyList<ClassVisibilityRule>> ReadClassVisibilityRulesAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var rules = new List<ClassVisibilityRule>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Kind, SectionKey, VisibleClasses
            FROM ClassVisibilityRules
            WHERE TableId = $tableId
            ORDER BY Kind, SectionKey;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!Enum.TryParse<ClassVisibilitySectionKind>(reader.GetString(0), ignoreCase: true, out var kind))
            {
                continue;
            }

            rules.Add(new ClassVisibilityRule(kind, reader.GetString(1), reader.GetInt64(2)));
        }

        return rules;
    }

    private static async Task<IReadOnlyList<string>> ReadRecordKeyFieldsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var fields = new List<string>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldName
            FROM RecordKeyFields
            WHERE TableId = $tableId
            ORDER BY Ordinal;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            fields.Add(reader.GetString(0));
        }

        return fields;
    }

    private static async Task<RecordKeySpec?> ReadRecordKeyAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var segments = new List<RecordKeySegment>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Path, Kind, KeyField, IsPrimary
            FROM TableKeys
            WHERE TableId = $tableId
            ORDER BY SegmentIndex;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var path = reader.GetString(0);
            if (!Enum.TryParse<RecordKeyKind>(reader.GetString(1), ignoreCase: true, out var kind))
            {
                continue;
            }

            var keyField = reader.IsDBNull(2) ? null : reader.GetString(2);
            if (kind == RecordKeyKind.Scalar && string.IsNullOrWhiteSpace(keyField))
            {
                continue;
            }

            segments.Add(new RecordKeySegment(path, kind, keyField, reader.GetInt32(3) != 0));
        }

        return segments.Count == 0 ? null : new RecordKeySpec(segments);
    }

    private static async Task<IReadOnlyList<ColumnMetadata>> ReadColumnsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var columns = new List<ColumnMetadata>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldName, DataType, EditorKind, DisplayWidth, LookupTable, TooltipKey, Flags
            FROM Columns
            WHERE TableId = $tableId
            ORDER BY XmlFieldName;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var name = reader.GetString(0);
            _ = Enum.TryParse<ColumnDataType>(reader.GetString(1), ignoreCase: true, out var dataType);
            _ = Enum.TryParse<ColumnEditorKind>(reader.GetString(2), ignoreCase: true, out var editorKind);
            columns.Add(new ColumnMetadata(name, dataType, editorKind)
            {
                DisplayWidth = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                LookupTable = reader.IsDBNull(4) ? null : reader.GetString(4),
                TooltipKey = reader.IsDBNull(5) ? null : reader.GetString(5),
                Flags = (ColumnFlags)reader.GetInt32(6),
            });
        }

        return columns;
    }

    private static async Task<IReadOnlyList<ListFieldDefinition>> ReadListFieldsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var fields = new List<ListFieldDefinition>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldName, DisplayNameKey
            FROM ListFields
            WHERE TableId = $tableId
            ORDER BY XmlFieldName;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            fields.Add(new ListFieldDefinition(reader.GetString(0))
            {
                DisplayNameKey = reader.IsDBNull(1) ? null : reader.GetString(1),
            });
        }

        return fields;
    }

    private static async Task<IReadOnlyList<DictFieldDefinition>> ReadDictFieldsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var fields = new List<DictFieldDefinition>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldMask, DisplayNameKey
            FROM DictFields
            WHERE TableId = $tableId
            ORDER BY XmlFieldMask;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            fields.Add(new DictFieldDefinition(reader.GetString(0))
            {
                DisplayNameKey = reader.IsDBNull(1) ? null : reader.GetString(1),
            });
        }

        return fields;
    }

    private static async Task<IReadOnlyList<string>> ReadTranslatableFieldsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var fields = new List<string>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT FieldPath
            FROM TranslatableFields
            WHERE TableId = $tableId
            ORDER BY FieldPath;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            fields.Add(reader.GetString(0));
        }

        return fields;
    }

    private static async Task<IReadOnlyList<ColumnHint>> ReadColumnHintsAsync(
        SqliteConnection connection,
        int tableId,
        CancellationToken cancellationToken)
    {
        var hints = new List<ColumnHint>();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT XmlFieldName, DataType, EditorKind, DisplayWidth, LookupTable, TooltipKey, Flags, GroupKey, GroupOrder, Minimum, Maximum
            FROM ColumnHints
            WHERE TableId = $tableId
            ORDER BY XmlFieldName;
            """;
        command.Parameters.AddWithValue("$tableId", tableId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var name = reader.GetString(0);
            hints.Add(new ColumnHint(name)
            {
                DataType = ReadNullableEnum<ColumnDataType>(reader, 1),
                EditorKind = ReadNullableEnum<ColumnEditorKind>(reader, 2),
                DisplayWidth = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                LookupTable = reader.IsDBNull(4) ? null : reader.GetString(4),
                TooltipKey = reader.IsDBNull(5) ? null : reader.GetString(5),
                Flags = reader.IsDBNull(6) ? null : (ColumnFlags)reader.GetInt32(6),
                GroupKey = reader.IsDBNull(7) ? null : reader.GetString(7),
                GroupOrder = reader.GetInt32(8),
                Minimum = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                Maximum = reader.IsDBNull(10) ? null : reader.GetInt64(10),
            });
        }

        return hints;
    }

    private static TEnum? ReadNullableEnum<TEnum>(SqliteDataReader reader, int ordinal)
        where TEnum : struct, Enum
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        return Enum.TryParse<TEnum>(reader.GetString(ordinal), ignoreCase: true, out var value)
            ? value
            : null;
    }

    private sealed record TableRow(
        int Id,
        string XmlTable,
        string? DtoName,
        bool IsLookup,
        string XmlRoot,
        string RecordElement);
}
