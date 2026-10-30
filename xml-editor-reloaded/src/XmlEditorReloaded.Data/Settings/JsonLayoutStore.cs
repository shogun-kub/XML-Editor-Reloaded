using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Layout;

namespace XmlEditorReloaded.Data.Settings;

/// <summary>
/// Stores per-table grid layouts as a versioned JSON document under the user's application-data
/// directory. Writes are atomic: a temporary file is replaced into place only after it is complete.
/// </summary>
public sealed class JsonLayoutStore : ITableLayoutStore
{
    private const int CurrentSchemaVersion = 1;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    private readonly string _layoutsPath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _unsupportedVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLayoutStore"/> class.
    /// </summary>
    /// <param name="layoutsPath">
    /// Optional layouts file path; defaults to <c>%AppData%/XmlEditorReloaded/layouts.json</c>.
    /// </param>
    public JsonLayoutStore(string? layoutsPath = null)
    {
        _layoutsPath = layoutsPath ?? DefaultLayoutsPath;
    }

    /// <summary>
    /// Gets the default layouts file path under the current user's application-data directory.
    /// </summary>
    public static string DefaultLayoutsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "XmlEditorReloaded",
        "layouts.json");

    /// <inheritdoc />
    public async Task<OperationResult<TableLayout>> LoadAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataSetDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var read = await ReadFileAsync(cancellationToken).ConfigureAwait(false);
            if (!read.IsSuccess)
            {
                return OperationResult<TableLayout>.Failure(read);
            }

            var entry = FindEntry(read.Data!, dataSetDirectory, tableName);
            return OperationResult<TableLayout>.Success(entry is null ? TableLayout.Empty : Map(entry));
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<OperationResult> SaveAsync(
        string dataSetDirectory,
        string tableName,
        TableLayout layout,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataSetDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(layout);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_unsupportedVersion)
            {
                return UnsupportedVersionFailure();
            }

            var read = await ReadFileAsync(cancellationToken).ConfigureAwait(false);
            if (!read.IsSuccess)
            {
                if (_unsupportedVersion
                    || read.ErrorCode != ErrorCodes.IoError
                    || !TryBackupUnreadableFile())
                {
                    return OperationResult.Failure(read);
                }
            }

            var file = read.IsSuccess ? read.Data! : CreateEmptyFile();
            file.SchemaVersion = CurrentSchemaVersion;
            file.Layouts ??= new List<LayoutEntryDto>();
            var entry = Map(layout, dataSetDirectory, tableName);
            var index = FindIndex(file, dataSetDirectory, tableName);
            if (index >= 0)
            {
                file.Layouts[index] = entry;
            }
            else
            {
                file.Layouts.Add(entry);
            }

            return await WriteFileAsync(file, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<OperationResult> RemoveAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataSetDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_unsupportedVersion)
            {
                return UnsupportedVersionFailure();
            }

            var read = await ReadFileAsync(cancellationToken).ConfigureAwait(false);
            if (!read.IsSuccess)
            {
                return OperationResult.Failure(read);
            }

            var index = FindIndex(read.Data!, dataSetDirectory, tableName);
            if (read.Data!.Layouts is not { } layouts || index < 0)
            {
                return OperationResult.Success();
            }

            layouts.RemoveAt(index);
            return await WriteFileAsync(read.Data, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<OperationResult> ResetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (File.Exists(_layoutsPath))
            {
                File.Delete(_layoutsPath);
            }

            _unsupportedVersion = false;
            return OperationResult.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            return OperationResult.Failure(
                ErrorCodes.IoAccessDenied,
                FileAccessFailure.DescribeWriteFailure(_layoutsPath, exception));
        }
        catch (IOException exception)
        {
            return OperationResult.Failure(
                ErrorCodes.IoError,
                $"The layouts file '{_layoutsPath}' could not be deleted: {exception.Message}");
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<OperationResult<LayoutsFileDto>> ReadFileAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_layoutsPath))
        {
            return OperationResult<LayoutsFileDto>.Success(CreateEmptyFile());
        }

        try
        {
            var json = await File.ReadAllTextAsync(_layoutsPath, cancellationToken).ConfigureAwait(false);
            var file = JsonSerializer.Deserialize<LayoutsFileDto>(json, SerializerOptions);
            if (file is null)
            {
                return OperationResult<LayoutsFileDto>.Success(CreateEmptyFile());
            }

            file.SchemaVersion = file.SchemaVersion <= 0 ? CurrentSchemaVersion : file.SchemaVersion;
            if (file.SchemaVersion > CurrentSchemaVersion)
            {
                _unsupportedVersion = true;
                return OperationResult<LayoutsFileDto>.Failure(
                    ErrorCodes.IoError,
                    $"The layouts file '{_layoutsPath}' uses schema version "
                    + $"{file.SchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture)}; "
                    + $"this build supports version {CurrentSchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture)}.");
            }

            file.Layouts ??= new List<LayoutEntryDto>();
            return OperationResult<LayoutsFileDto>.Success(file);
        }
        catch (JsonException exception)
        {
            return OperationResult<LayoutsFileDto>.Failure(
                ErrorCodes.IoError,
                $"The layouts file '{_layoutsPath}' contains invalid JSON: {exception.Message}");
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<LayoutsFileDto>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to the layouts file '{_layoutsPath}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<LayoutsFileDto>.Failure(
                ErrorCodes.IoError,
                $"The layouts file '{_layoutsPath}' could not be read: {exception.Message}");
        }
    }

    private static LayoutsFileDto CreateEmptyFile() => new()
    {
        SchemaVersion = CurrentSchemaVersion,
        Layouts = new List<LayoutEntryDto>(),
    };

    private async Task<OperationResult> WriteFileAsync(
        LayoutsFileDto file,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_layoutsPath);
        if (!string.IsNullOrEmpty(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (UnauthorizedAccessException exception)
            {
                return OperationResult.Failure(
                    ErrorCodes.IoAccessDenied,
                    FileAccessFailure.DescribeWriteFailure(directory, exception));
            }
            catch (IOException exception)
            {
                return OperationResult.Failure(
                    ErrorCodes.IoError,
                    $"The layouts directory '{directory}' could not be created: {exception.Message}");
            }
        }

        var tempPath = _layoutsPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            var json = JsonSerializer.Serialize(file, SerializerOptions);
            await File.WriteAllTextAsync(
                    tempPath,
                    json,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                    cancellationToken)
                .ConfigureAwait(false);
            File.Move(tempPath, _layoutsPath, overwrite: true);
            return OperationResult.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            return OperationResult.Failure(
                ErrorCodes.IoAccessDenied,
                FileAccessFailure.DescribeWriteFailure(_layoutsPath, exception));
        }
        catch (IOException exception)
        {
            var code = FileAccessFailure.IsSharingViolation(exception)
                ? ErrorCodes.IoAccessDenied
                : ErrorCodes.IoError;
            return OperationResult.Failure(code, FileAccessFailure.DescribeWriteFailure(_layoutsPath, exception));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (IOException)
                {
                    // A leftover temporary file does not affect the saved layouts.
                }
            }
        }
    }

    private static string NormalizePath(string path) => Path.GetFullPath(path).TrimEnd(
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar);

    private static LayoutEntryDto? FindEntry(
        LayoutsFileDto file,
        string dataSetDirectory,
        string tableName) =>
        file.Layouts?.FirstOrDefault(entry => Matches(entry, dataSetDirectory, tableName));

    private static int FindIndex(
        LayoutsFileDto file,
        string dataSetDirectory,
        string tableName) =>
        file.Layouts?.FindIndex(entry => Matches(entry, dataSetDirectory, tableName)) ?? -1;

    private static bool Matches(LayoutEntryDto entry, string dataSetDirectory, string tableName)
    {
        if (string.IsNullOrWhiteSpace(entry.DataSet)
            || !string.Equals(entry.Table, tableName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            return string.Equals(
                NormalizePath(entry.DataSet),
                NormalizePath(dataSetDirectory),
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException)
        {
            return false;
        }
    }

    private static TableLayout Map(LayoutEntryDto entry)
    {
        var columns = entry.Columns?
            .Where(static column => !string.IsNullOrWhiteSpace(column.Name))
            .Select(static column => new ColumnLayout(
                column.Name!,
                column.IsVisible,
                column.Width,
                column.DisplayOrder))
            .ToArray() ?? Array.Empty<ColumnLayout>();

        var filter = entry.Filter is null
            ? RowFilterGroup.Empty
            : new RowFilterGroup(
                entry.Filter.Operator,
                entry.Filter.Criteria?
                    .Where(static criterion => !string.IsNullOrWhiteSpace(criterion.Field))
                    .Select(static criterion => new RowFilterCriterion(
                        criterion.Field!,
                        criterion.Operator,
                        criterion.Operand ?? string.Empty))
                    .ToArray() ?? Array.Empty<RowFilterCriterion>());

        return new TableLayout(columns, entry.Sort?.Field, entry.Sort?.Direction ?? TableSortDirection.Ascending, filter);
    }

    private static LayoutEntryDto Map(TableLayout layout, string dataSetDirectory, string tableName) => new()
    {
        DataSet = dataSetDirectory,
        Table = tableName,
        Columns = layout.Columns
            .Select(static column => new ColumnDto
            {
                Name = column.Name,
                IsVisible = column.IsVisible,
                Width = column.Width,
                DisplayOrder = column.DisplayOrder,
            })
            .ToList(),
        Sort = layout.SortField is null
            ? null
            : new SortDto { Field = layout.SortField, Direction = layout.SortDirection },
        Filter = layout.Filter.IsEmpty
            ? null
            : new FilterDto
            {
                Operator = layout.Filter.Operator,
                Criteria = layout.Filter.Criteria
                    .Select(static criterion => new CriterionDto
                    {
                        Field = criterion.Field,
                        Operator = criterion.Operator,
                        Operand = criterion.Operand,
                    })
                    .ToList(),
            },
    };

    private OperationResult UnsupportedVersionFailure() => OperationResult.Failure(
        ErrorCodes.IoError,
        $"The layouts file '{_layoutsPath}' uses a newer schema version and will not be overwritten.");

    private bool TryBackupUnreadableFile()
    {
        try
        {
            File.Move(_layoutsPath, _layoutsPath + ".bad", overwrite: true);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private sealed class LayoutsFileDto
    {
        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        public List<LayoutEntryDto>? Layouts { get; set; }
    }

    private sealed class LayoutEntryDto
    {
        public string? DataSet { get; set; }

        public string? Table { get; set; }

        public List<ColumnDto>? Columns { get; set; }

        public SortDto? Sort { get; set; }

        public FilterDto? Filter { get; set; }
    }

    private sealed class ColumnDto
    {
        public string? Name { get; set; }

        public bool IsVisible { get; set; } = true;

        public double Width { get; set; }

        public int DisplayOrder { get; set; }
    }

    private sealed class SortDto
    {
        public string? Field { get; set; }

        public TableSortDirection Direction { get; set; }
    }

    private sealed class FilterDto
    {
        public RowFilterLogicalOperator Operator { get; set; }

        public List<CriterionDto>? Criteria { get; set; }
    }

    private sealed class CriterionDto
    {
        public string? Field { get; set; }

        public RowFilterOperator Operator { get; set; }

        public string? Operand { get; set; }
    }
}
