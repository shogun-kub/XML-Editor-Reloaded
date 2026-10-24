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

        return new TableMetadata(
            TableName.Parse(row.XmlTable),
            row.XmlRoot,
            row.RecordElement,
            row.IsLookup,
            recordKey,
            columns,
            listFields,
            dictFields,
            translatableFields);
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

    private sealed record TableRow(
        int Id,
        string XmlTable,
        string? DtoName,
        bool IsLookup,
        string XmlRoot,
        string RecordElement);
}
