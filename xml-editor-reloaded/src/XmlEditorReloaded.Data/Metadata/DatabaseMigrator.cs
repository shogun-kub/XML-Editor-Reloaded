using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Data.Sqlite;
using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Data.Metadata;

/// <summary>
/// Applies embedded, versioned SQL migrations to the metadata SQLite database.
/// </summary>
public sealed class DatabaseMigrator
{
    private const string MigrationResourceMarker = ".Metadata.Migrations.";
    private const string MigrationResourceExtension = ".sql";

    /// <summary>
    /// Creates or upgrades the metadata database at <paramref name="databasePath"/>.
    /// </summary>
    /// <param name="databasePath">Absolute path of the SQLite database file.</param>
    /// <param name="cancellationToken">Token to cancel the migration.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> is the number of applied migrations.
    /// On failure, <c>error_code</c> is <c>metadata.migration_error</c>, <c>metadata.downgrade_unsupported</c>,
    /// <c>io.access_denied</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="databasePath"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<int>> MigrateAsync(string databasePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        try
        {
            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the metadata directory for '{databasePath}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.IoError,
                $"The metadata directory for '{databasePath}' could not be created: {exception.Message}");
        }

        var migrations = LoadMigrations();
        if (migrations.Count == 0)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.MetadataMigrationError,
                "No embedded metadata migrations were found.");
        }

        try
        {
            await using var connection = new SqliteConnection(BuildConnectionString(databasePath));
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            await CreateMigrationTableAsync(connection, cancellationToken).ConfigureAwait(false);
            var appliedVersions = await ReadAppliedVersionsAsync(connection, cancellationToken).ConfigureAwait(false);

            var latestAvailable = migrations[^1].Version;
            var latestApplied = appliedVersions.Count == 0 ? 0 : appliedVersions.Max();
            if (latestApplied > latestAvailable)
            {
                return OperationResult<int>.Failure(
                    ErrorCodes.MetadataDowngradeUnsupported,
                    $"The metadata database schema version {latestApplied} is newer than the supported version {latestAvailable}.");
            }

            var appliedCount = 0;
            foreach (var migration in migrations)
            {
                if (appliedVersions.Contains(migration.Version))
                {
                    continue;
                }

                await using var transaction = (SqliteTransaction)await connection
                    .BeginTransactionAsync(cancellationToken)
                    .ConfigureAwait(false);
                try
                {
                    await using var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = migration.Sql;
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    await using var record = connection.CreateCommand();
                    record.Transaction = transaction;
                    record.CommandText =
                        "INSERT INTO SchemaMigrations (Version, Name, AppliedAtUtc) VALUES ($version, $name, $appliedAt);";
                    record.Parameters.AddWithValue("$version", migration.Version);
                    record.Parameters.AddWithValue("$name", migration.Name);
                    record.Parameters.AddWithValue("$appliedAt", DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture));
                    await record.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                    appliedCount++;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

            return OperationResult<int>.Success(appliedCount);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the metadata database '{databasePath}' was denied.");
        }
        catch (SqliteException exception)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.MetadataMigrationError,
                $"Metadata migration failed: {exception.Message}");
        }
        catch (IOException exception)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.IoError,
                $"The metadata database '{databasePath}' could not be accessed: {exception.Message}");
        }
    }

    private static string BuildConnectionString(string databasePath) =>
        new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
        }.ToString();

    private static async Task CreateMigrationTableAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS SchemaMigrations (
                Version INTEGER NOT NULL PRIMARY KEY,
                Name TEXT NOT NULL,
                AppliedAtUtc TEXT NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<HashSet<int>> ReadAppliedVersionsAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        var versions = new HashSet<int>();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT Version FROM SchemaMigrations;";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            versions.Add(reader.GetInt32(0));
        }

        return versions;
    }

    private static List<EmbeddedMigration> LoadMigrations()
    {
        var assembly = typeof(DatabaseMigrator).Assembly;
        var migrations = new List<EmbeddedMigration>();

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.Contains(MigrationResourceMarker, StringComparison.Ordinal)
                || !resourceName.EndsWith(MigrationResourceExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var markerIndex = resourceName.IndexOf(MigrationResourceMarker, StringComparison.Ordinal);
            var fileName = resourceName[(markerIndex + MigrationResourceMarker.Length)..^MigrationResourceExtension.Length];
            var separatorIndex = fileName.IndexOf('_');
            if (separatorIndex <= 0
                || !int.TryParse(fileName[..separatorIndex], NumberStyles.None, CultureInfo.InvariantCulture, out var version))
            {
                continue;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream, Encoding.UTF8);
            migrations.Add(new EmbeddedMigration(version, fileName, reader.ReadToEnd()));
        }

        return migrations.OrderBy(static migration => migration.Version).ToList();
    }

    private sealed record EmbeddedMigration(int Version, string Name, string Sql);
}
