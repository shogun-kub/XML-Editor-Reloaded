using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Schema;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One editable field in the detail panel of the selected row.
/// </summary>
public sealed partial class DetailFieldViewModel : ObservableObject
{
    private readonly TableEditorViewModel _editor;

    internal DetailFieldViewModel(
        TableEditorViewModel editor,
        TableRowViewModel row,
        TableSchemaColumn column)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(column);

        _editor = editor;
        Row = row;
        Column = column;
        Name = column.XmlFieldName;
        TypeText = column.DataType.ToString();
        IsReadOnly = column.IsKey
            || column.IsList
            || column.IsDict
            || column.IsNested
            || !editor.IsEditable
            || row.Key is null;
        CanEditBits = !IsReadOnly
            && (column.DataType == ColumnDataType.Flags
                || column.EditorKind == ColumnEditorKind.FlagEditor);
    }

    /// <summary>
    /// Gets the row this field belongs to.
    /// </summary>
    internal TableRowViewModel Row { get; }

    /// <summary>
    /// Gets the schema column of the field.
    /// </summary>
    internal TableSchemaColumn Column { get; }

    /// <summary>
    /// Gets the XML field name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the field type name.
    /// </summary>
    public string TypeText { get; }

    /// <summary>
    /// Gets the stable group key used to organize the detail form.
    /// </summary>
    public string GroupKey => Column.GroupKey;

    /// <summary>
    /// Gets the group display order.
    /// </summary>
    public int GroupOrder => Column.GroupOrder;

    /// <summary>
    /// Gets a value indicating whether the field cannot be edited here. Keys, repeated fields, and
    /// nested structures are read-only until dedicated editors are available.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets a value indicating whether the bit editor is available for this field.
    /// </summary>
    public bool CanEditBits { get; }

    /// <summary>
    /// Gets or sets the current field value.
    /// </summary>
    public string Value
    {
        get => Row[Name];
        set
        {
            if (IsReadOnly)
            {
                return;
            }

            _editor.SetDetailValue(this, value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTranslated));
            OnPropertyChanged(nameof(IsUntranslated));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the value comes from the language overlay.
    /// </summary>
    public bool IsTranslated => Row.IsTranslated(Name);

    /// <summary>
    /// Gets a value indicating whether the field can be translated through an overlay.
    /// </summary>
    public bool IsTranslatable => Column.IsTranslatable;

    /// <summary>
    /// Gets a value indicating whether a translation is expected but the value still comes from the
    /// base table.
    /// </summary>
    public bool IsUntranslated =>
        _editor.IsTranslationMode
        && Column.IsTranslatable
        && !Row.IsTranslated(Name)
        && !string.IsNullOrEmpty(Row[Name]);

    internal void Refresh()
    {
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(IsTranslated));
        OnPropertyChanged(nameof(IsUntranslated));
    }

    [RelayCommand]
    private void EditBits() => _editor.EditBits(this);
}
