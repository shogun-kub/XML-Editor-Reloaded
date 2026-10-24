using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
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
        TableSchemaColumn column,
        LookupColumnData? lookup = null,
        RelationColumnData? relation = null)
    {
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(column);

        _editor = editor;
        Row = row;
        Column = column;
        Lookup = lookup;
        Relation = relation;
        Name = column.XmlFieldName;
        TypeText = column.DataType.ToString();
        IsReadOnly = column.IsKey
            || column.IsList
            || column.IsDict
            || column.IsNested
            || !editor.IsEditable
            || row.Key is null;
        IsEditable = !IsReadOnly;
        IsBoolean = column.DataType == ColumnDataType.Boolean;
        IsLookup = column.EditorKind == ColumnEditorKind.LookupCombo
            && lookup is { IsAvailable: true };
        HasLookupError = lookup is { IsAvailable: false };
        IsRelation = relation is { IsAvailable: true };
        HasRelationError = relation is { IsAvailable: false };
        IsPlainText = !IsBoolean && !IsLookup && !IsRelation;
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
    /// Gets the loaded lookup data of the field, or <see langword="null"/> when the field has no lookup.
    /// </summary>
    internal LookupColumnData? Lookup { get; }

    /// <summary>
    /// Gets the resolved relation data of the field, or <see langword="null"/> when the field is not
    /// a relation.
    /// </summary>
    public RelationColumnData? Relation { get; }

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
    /// Gets a value indicating whether the field accepts input.
    /// </summary>
    public bool IsEditable { get; }

    /// <summary>
    /// Gets a value indicating whether the field uses a checkbox editor.
    /// </summary>
    public bool IsBoolean { get; }

    /// <summary>
    /// Gets a value indicating whether the field uses a lookup combo.
    /// </summary>
    public bool IsLookup { get; }

    /// <summary>
    /// Gets a value indicating whether the field uses a plain text box.
    /// </summary>
    public bool IsPlainText { get; }

    /// <summary>
    /// Gets a value indicating whether a configured lookup source could not be loaded.
    /// </summary>
    public bool HasLookupError { get; }

    /// <summary>
    /// Gets a value indicating whether the field uses a searchable relation picker.
    /// </summary>
    public bool IsRelation { get; }

    /// <summary>
    /// Gets a value indicating whether a configured relation target could not be loaded.
    /// </summary>
    public bool HasRelationError { get; }

    /// <summary>
    /// Gets a value indicating whether a lookup or relation source configured for the field could not
    /// be loaded.
    /// </summary>
    public bool HasSourceError => HasLookupError || HasRelationError;

    /// <summary>
    /// Gets the lookup load error text, or an empty string when the lookup is available.
    /// </summary>
    public string LookupErrorText => HasLookupError && Lookup?.ErrorText is { } error
        ? Strings.Format("LookupUnavailableFormat", error)
        : string.Empty;

    /// <summary>
    /// Gets the relation load error text, or an empty string when the target is available.
    /// </summary>
    public string RelationErrorText => HasRelationError && Relation?.ErrorText is { } error
        ? Strings.Format("RelationUnavailableFormat", error)
        : string.Empty;

    /// <summary>
    /// Gets a value indicating whether the referenced record can be opened.
    /// </summary>
    public bool CanGoToTarget =>
        IsRelation && Relation!.TryGetTargetValue(Row[Name], out _);

    /// <summary>
    /// Gets the tooltip shown on the field name, including relation, lookup, and type information.
    /// </summary>
    public string FieldTooltip =>
        HasLookupError ? LookupErrorText
        : HasRelationError ? RelationErrorText
        : TypeText;

    /// <summary>
    /// Gets the selectable lookup options, or an empty list when the field has no lookup.
    /// </summary>
    public IReadOnlyList<LookupOption> LookupOptions =>
        Lookup?.Options ?? Array.Empty<LookupOption>();

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
            OnPropertyChanged(nameof(BooleanValue));
            OnPropertyChanged(nameof(BooleanTooltip));
            OnPropertyChanged(nameof(CanGoToTarget));
            GoToTargetCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Gets or sets the checkbox state of a Boolean field. Unchecking an existing field writes
    /// <c>0</c>; checking an absent field appends <c>1</c> without changing other sparse fields.
    /// </summary>
    public bool BooleanValue
    {
        get => Value.Trim() is "1" or "true" or "True";
        set
        {
            if (!IsBoolean || IsReadOnly)
            {
                return;
            }

            var normalized = value ? "1" : "0";
            if (string.Equals(Value, normalized, StringComparison.Ordinal))
            {
                return;
            }

            Value = normalized;
        }
    }

    /// <summary>
    /// Gets the tooltip of the boolean checkbox, which explains that an absent field is not stored.
    /// </summary>
    public string BooleanTooltip => Value.Length == 0 ? Strings.Get("DetailNotSet") : string.Empty;

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
        OnPropertyChanged(nameof(BooleanValue));
        OnPropertyChanged(nameof(BooleanTooltip));
        OnPropertyChanged(nameof(CanGoToTarget));
        GoToTargetCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void EditBits() => _editor.EditBits(this);

    [RelayCommand(CanExecute = nameof(CanGoToTarget))]
    private void GoToTarget() => _editor.NavigateToRelation(this);
}
