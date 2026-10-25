using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Editor for the seven enemy and militia ammunition rows. Legacy moved the <c>uiIndex=19</c> row out of
/// each item-choice table into a synthetic <c>ENEMYAMMOCHOICES</c> table whose fifty <c>bItemNoN</c>
/// fields referenced <c>Items.AmmoTypes</c> (<c>EnemyItemTable.vb</c>). The rewrite keeps the rows in
/// their real files but presents the same seven-row view with ammunition-type pickers.
/// </summary>
public sealed partial class EnemyAmmoViewModel : ObservableObject
{
    private const string AmmoRowKey = "19";
    private const string AmmoTypesTable = "Items.AmmoTypes";
    private const string AmmoChoicesLookup = "Lookup.AmmoChoices";
    private const int AmmoFieldCount = 50;

    private static readonly (string Table, int ChoiceId)[] ChoiceTables =
    {
        ("Inventory.EnemyItemChoices", 19),
        ("Inventory.ItemChoices_Enemy_Admin", 29),
        ("Inventory.ItemChoices_Enemy_Regular", 39),
        ("Inventory.ItemChoices_Enemy_Elite", 49),
        ("Inventory.ItemChoices_Militia_Green", 59),
        ("Inventory.ItemChoices_Militia_Regular", 69),
        ("Inventory.ItemChoices_Militia_Elite", 79),
    };

    private readonly IDataSetSession _session;
    private readonly List<TableWorkspace> _workspaces = new();

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyAmmoViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that owns the item-choice files.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="session"/> is null.</exception>
    public EnemyAmmoViewModel(IDataSetSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
        Title = Strings.Get("EnemyAmmoTitle");
        SaveText = Strings.Get("EditorSaveText");
        ReloadText = Strings.Get("EditorReloadText");
        CloseText = Strings.Get("EnemyAmmoCloseText");
        HintText = Strings.Get("EnemyAmmoHint");
    }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the save button text.
    /// </summary>
    public string SaveText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the explanation shown above the grid.
    /// </summary>
    public string HintText { get; }

    /// <summary>
    /// Gets the seven ammunition rows, one per item-choice table.
    /// </summary>
    public ObservableCollection<EnemyAmmoRowViewModel> Rows { get; } = new();

    /// <summary>
    /// Gets non-fatal load problems, for example a missing item-choice file.
    /// </summary>
    public ObservableCollection<string> Warnings { get; } = new();

    /// <summary>
    /// Gets a value indicating whether at least one warning was reported.
    /// </summary>
    public bool HasWarnings => Warnings.Count > 0;

    /// <summary>
    /// Loads the seven choice tables, resolves ammunition-type names, and builds the row views.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the rows are ready.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Rows.Clear();
            Warnings.Clear();
            _workspaces.Clear();
            OnPropertyChanged(nameof(HasWarnings));

            var catalog = await _session.Catalog
                .GetTablesAsync(_session.TableDataDirectory, cancellationToken)
                .ConfigureAwait(true);
            if (!catalog.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{catalog.ErrorCode}: {catalog.ErrorText}");
                return;
            }

            var descriptors = catalog.Data!.ToDictionary(
                static descriptor => descriptor.TableName.Value,
                static descriptor => descriptor,
                StringComparer.Ordinal);

            var relation = new TableRelation(
                TableName.Parse(ChoiceTables[0].Table),
                "bItemNo1",
                TableName.Parse(AmmoTypesTable),
                TableRelationKind.Reference,
                targetValueField: "uiIndex",
                targetDisplayField: "name");
            var target = await _session.RelationResolver
                .ResolveAsync(relation, languageCode: null, cancellationToken)
                .ConfigureAwait(true);
            if (!target.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{target.ErrorCode}: {target.ErrorText}");
                return;
            }

            var choiceNames = await LoadChoiceNamesAsync(cancellationToken).ConfigureAwait(true);
            foreach (var (tableName, choiceId) in ChoiceTables)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!descriptors.TryGetValue(tableName, out var descriptor))
                {
                    AddWarning(Strings.Format("EnemyAmmoMissingTable", tableName));
                    continue;
                }

                var opened = await _session
                    .OpenTableAsync(descriptor, languageCode: null, cancellationToken)
                    .ConfigureAwait(true);
                if (!opened.IsSuccess)
                {
                    AddWarning($"{tableName}: {opened.ErrorCode}: {opened.ErrorText}");
                    continue;
                }

                var workspace = opened.Data!;
                var record = workspace.BaseDocument.FindRecord(AmmoRowKey);
                if (record is null)
                {
                    AddWarning(Strings.Format("EnemyAmmoMissingRow", tableName, AmmoRowKey));
                    continue;
                }

                var values = Enumerable.Range(1, AmmoFieldCount)
                    .Select(index => record.GetRawValue("bItemNo" + index.ToString(CultureInfo.InvariantCulture))?.Trim())
                    .ToArray();
                var relationData = RelationColumnData.Create(relation, target.Data!, values);
                var cells = new List<EnemyAmmoCellViewModel>(AmmoFieldCount);
                for (var index = 1; index <= AmmoFieldCount; index++)
                {
                    var fieldName = "bItemNo" + index.ToString(CultureInfo.InvariantCulture);
                    cells.Add(new EnemyAmmoCellViewModel(
                        this,
                        workspace,
                        AmmoRowKey,
                        fieldName,
                        index,
                        relationData,
                        record.GetRawValue(fieldName)?.Trim() ?? string.Empty));
                }

                _workspaces.Add(workspace);
                var choiceName = choiceNames.GetValueOrDefault(
                    choiceId.ToString(CultureInfo.InvariantCulture),
                    tableName[(tableName.IndexOf('.') + 1)..]);
                Rows.Add(new EnemyAmmoRowViewModel(choiceName, tableName, cells));
            }

            IsDirty = false;
            StatusText = Rows.Count == 0
                ? Strings.Get("EnemyAmmoNothingLoaded")
                : Strings.Format("EnemyAmmoLoadedFormat", Rows.Count);
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves every dirty choice table. Files without changes are not rewritten.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save.</param>
    /// <returns><see langword="true"/> when every dirty file was saved.</returns>
    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            var failures = new List<string>();
            foreach (var workspace in _workspaces)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!workspace.IsDirty)
                {
                    continue;
                }

                var result = await workspace.SaveAsync(cancellationToken).ConfigureAwait(true);
                if (!result.IsSuccess)
                {
                    failures.Add($"{workspace.Descriptor.TableName}: {result.ErrorCode}: {result.ErrorText}");
                }
                else
                {
                    _session.RelationResolver.Invalidate(workspace.Descriptor.TableName);
                }
            }

            IsDirty = _workspaces.Any(static workspace => workspace.IsDirty);
            if (failures.Count == 0)
            {
                StatusText = Strings.Get("EnemyAmmoSaved");
                return true;
            }

            StatusText = Strings.Format("StatusError", string.Join("; ", failures));
            return false;
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Discards in-memory edits and reads the seven files again.
    /// </summary>
    /// <returns>A task that completes when the rows are reloaded.</returns>
    public Task ReloadAsync() => InitializeAsync();

    /// <summary>
    /// Saves the current edits and keeps the window open.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    [RelayCommand]
    private Task Save() => SaveAsync();

    /// <summary>
    /// Reloads the seven files, discarding in-memory edits.
    /// </summary>
    /// <returns>A task that completes when reloading finishes.</returns>
    [RelayCommand]
    private Task Reload() => ReloadAsync();

    internal bool TrySetValue(EnemyAmmoCellViewModel cell, string value)
    {
        ArgumentNullException.ThrowIfNull(cell);
        ArgumentNullException.ThrowIfNull(value);

        var result = cell.Workspace.SetValue(
            cell.RecordKey,
            cell.FieldName,
            value,
            addIfMissing: true);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return false;
        }

        IsDirty = true;
        StatusText = Strings.Format("EditorCellUpdated", cell.FieldName, cell.RecordKey);
        return true;
    }

    private async Task<IReadOnlyDictionary<string, string>> LoadChoiceNamesAsync(
        CancellationToken cancellationToken)
    {
        var entries = await _session.Lookups
            .GetEntriesAsync(TableName.Parse(AmmoChoicesLookup), languageCode: null, cancellationToken)
            .ConfigureAwait(true);
        if (!entries.IsSuccess)
        {
            AddWarning($"{AmmoChoicesLookup}: {entries.ErrorCode}: {entries.ErrorText}");
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        return entries.Data!.ToDictionary(
            static entry => entry.Id.ToString(CultureInfo.InvariantCulture),
            static entry => entry.Name,
            StringComparer.Ordinal);
    }

    private void AddWarning(string warning)
    {
        Warnings.Add(warning);
        OnPropertyChanged(nameof(HasWarnings));
    }
}

/// <summary>
/// One of the seven ammunition rows, labelled with the name from <c>Lookup.AmmoChoices</c>.
/// </summary>
/// <param name="ChoiceName">Ammunition profile name, for example "Default Enemy Ammo".</param>
/// <param name="TableName">Real item-choice table that stores the row.</param>
/// <param name="Cells">Fifty ammunition-type cells.</param>
public sealed record EnemyAmmoRowViewModel(
    string ChoiceName,
    string TableName,
    IReadOnlyList<EnemyAmmoCellViewModel> Cells);

/// <summary>
/// One <c>bItemNoN</c> cell of an ammunition row. The value is an <c>Items.AmmoTypes</c> key, not an
/// item key, so the picker shows ammunition names such as Ball/HP/AP.
/// </summary>
public sealed partial class EnemyAmmoCellViewModel : ObservableObject
{
    private readonly EnemyAmmoViewModel _owner;
    private string _value;

    internal EnemyAmmoCellViewModel(
        EnemyAmmoViewModel owner,
        TableWorkspace workspace,
        string recordKey,
        string fieldName,
        int index,
        RelationColumnData relation,
        string value)
    {
        _owner = owner;
        Workspace = workspace;
        RecordKey = recordKey;
        FieldName = fieldName;
        Index = index;
        Relation = relation;
        _value = value;
    }

    /// <summary>
    /// Gets the workspace of the choice table that stores this cell.
    /// </summary>
    public TableWorkspace Workspace { get; }

    /// <summary>
    /// Gets the key of the ammunition row, always <c>19</c> in the source file.
    /// </summary>
    public string RecordKey { get; }

    /// <summary>
    /// Gets the XML field name, for example <c>bItemNo12</c>.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the one-based ammunition slot shown in the header.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the localized slot label.
    /// </summary>
    public string FieldLabel => Strings.Format("EnemyAmmoFieldFormat", Index);

    /// <summary>
    /// Gets the picker data resolved against <c>Items.AmmoTypes</c>.
    /// </summary>
    public RelationColumnData Relation { get; }

    /// <summary>
    /// Gets a value indicating whether the source file accepts edits.
    /// </summary>
    public bool IsReadOnly => !Workspace.CanEdit;

    /// <summary>
    /// Gets the display text of the current ammunition type.
    /// </summary>
    public string Display => Relation.Display(_value);

    /// <summary>
    /// Gets or sets the raw ammunition-type identifier. The edit is written immediately to the choice
    /// file; an invalid value is rejected and the cell keeps its previous text.
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            if (string.Equals(_value, value, StringComparison.Ordinal))
            {
                return;
            }

            var previous = _value;
            _value = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Display));
            if (!_owner.TrySetValue(this, value))
            {
                _value = previous;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Display));
            }
        }
    }
}
