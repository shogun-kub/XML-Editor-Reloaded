using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One editable grid row. Values are read lazily from the workspace row, so edits made through the
/// workspace are visible without rebuilding the row.
/// </summary>
public sealed partial class TableRowViewModel : ObservableObject
{
    private readonly ITableCellEditor _editor;
    private readonly Dictionary<string, bool> _translationState = new(StringComparer.Ordinal);
    private WorkspaceRow _row;
    private string[] _translatableFields = Array.Empty<string>();
    private bool _translationMode;

    internal TableRowViewModel(WorkspaceRow row, ITableCellEditor editor)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(editor);
        _row = row;
        _editor = editor;
    }

    /// <summary>
    /// Gets the primary record key, or <see langword="null"/> when the record has no key.
    /// </summary>
    public string? Key => _row.Key;

    /// <summary>
    /// Gets a value indicating whether the row can be edited through the workspace.
    /// </summary>
    public bool CanEdit => _row.Key is not null;

    /// <summary>
    /// Gets or sets a cell value by XML field name.
    /// </summary>
    /// <param name="fieldName">XML field name of the column.</param>
    /// <returns>The effective value, or an empty string when the field is absent.</returns>
    public string this[string fieldName]
    {
        get => _row.GetValue(fieldName) ?? string.Empty;
        set
        {
            _editor.SetCellValue(this, fieldName, value);
            OnPropertyChanged("Item[]");
        }
    }

    /// <summary>
    /// Gets the stored value of a field, distinguishing an absent field from an empty one.
    /// </summary>
    /// <param name="fieldName">XML field name.</param>
    /// <returns>The stored value, or <see langword="null"/> when the field is absent.</returns>
    internal string? GetStoredValue(string fieldName) => _row.GetValue(fieldName);

    /// <summary>
    /// Gets a value indicating whether the field value comes from the language overlay.
    /// </summary>
    /// <param name="fieldName">XML field name.</param>
    /// <returns><see langword="true"/> when the field is translated; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public bool IsTranslated(string fieldName) => _row.IsTranslated(fieldName);

    /// <summary>
    /// Gets the untranslated state of translatable fields, keyed by XML field name. The dictionary is
    /// empty when no language overlay is selected. The value is <see langword="true"/> when the field
    /// is translatable and the current value still comes from the base table.
    /// </summary>
    public IReadOnlyDictionary<string, bool> TranslationState => _translationState;

    /// <summary>
    /// Configures translation markers for this row. The state dictionary is rebuilt so the grid can
    /// highlight fields that still fall back to the base table.
    /// </summary>
    /// <param name="enabled">Whether a language overlay is selected.</param>
    /// <param name="translatableFields">XML field names that can be translated.</param>
    internal void ConfigureTranslation(bool enabled, IEnumerable<string> translatableFields)
    {
        ArgumentNullException.ThrowIfNull(translatableFields);
        _translationMode = enabled;
        _translatableFields = translatableFields.ToArray();
        RecomputeTranslationState();
        OnPropertyChanged(nameof(TranslationState));
    }

    internal void Refresh(WorkspaceRow row)
    {
        ArgumentNullException.ThrowIfNull(row);
        _row = row;
        RecomputeTranslationState();
        OnPropertyChanged(nameof(TranslationState));
        OnPropertyChanged("Item[]");
    }

    private void RecomputeTranslationState()
    {
        _translationState.Clear();
        if (!_translationMode)
        {
            return;
        }

        foreach (var field in _translatableFields)
        {
            _translationState[field] = !_row.IsTranslated(field) && _row.GetValue(field) is not null;
        }
    }
}
