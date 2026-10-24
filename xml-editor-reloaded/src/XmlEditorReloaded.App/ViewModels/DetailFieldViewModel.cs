using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Schema;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One editable field in a detail form of the selected record. The field may be a direct child of the
/// record or a leaf of a nested structure, for example <c>STAND_MODIFIERS/FlatBase</c>.
/// </summary>
public sealed partial class DetailFieldViewModel : ObservableObject
{
    private readonly IDetailFieldHost _host;

    private DetailFieldViewModel(
        IDetailFieldHost host,
        TableRowViewModel row,
        FieldPath path,
        ColumnDataType dataType,
        int order,
        string groupKey,
        int groupOrder,
        bool isReadOnly,
        bool isTranslatable,
        bool canEditBits,
        LookupColumnData? lookup,
        RelationColumnData? relation)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(path);

        _host = host;
        Row = row;
        Path = path;
        DataType = dataType;
        Order = order;
        GroupKey = groupKey;
        GroupOrder = groupOrder;
        IsTranslatable = isTranslatable;
        Lookup = lookup;
        Relation = relation;
        Name = path.FieldName;
        PathText = path.Text;
        TypeText = dataType.ToString();
        IsReadOnly = isReadOnly;
        IsEditable = !isReadOnly;
        IsBoolean = dataType == ColumnDataType.Boolean;
        IsLookup = lookup is { IsAvailable: true };
        HasLookupError = lookup is { IsAvailable: false };
        IsRelation = relation is { IsAvailable: true };
        HasRelationError = relation is { IsAvailable: false };
        IsPlainText = !IsBoolean && !IsLookup && !IsRelation;
        CanEditBits = canEditBits;
    }

    /// <summary>
    /// Initializes a field from a schema column.
    /// </summary>
    /// <param name="host">Form that applies edits and navigation.</param>
    /// <param name="row">Record that holds the value.</param>
    /// <param name="column">Schema column of the field.</param>
    /// <param name="lookup">Optional lookup source of the field.</param>
    /// <param name="relation">Optional relation source of the field.</param>
    internal DetailFieldViewModel(
        IDetailFieldHost host,
        TableRowViewModel row,
        TableSchemaColumn column,
        LookupColumnData? lookup = null,
        RelationColumnData? relation = null)
        : this(
            host,
            row,
            new FieldPath(new[] { column.XmlFieldName }),
            column.DataType,
            column.Order,
            column.GroupKey,
            column.GroupOrder,
            column.IsKey || column.IsList || column.IsDict || column.IsNested || !host.IsEditable || row.Key is null,
            column.IsTranslatable,
            !column.IsKey && !column.IsList && !column.IsDict && !column.IsNested
                && host.IsEditable && row.Key is not null
                && (column.DataType == ColumnDataType.Flags || column.EditorKind == ColumnEditorKind.FlagEditor),
            lookup,
            relation)
    {
        ArgumentNullException.ThrowIfNull(column);
        Column = column;
    }

    /// <summary>
    /// Initializes a field that is not a top-level schema column, for example a nested modifier.
    /// </summary>
    /// <param name="host">Form that applies edits and navigation.</param>
    /// <param name="row">Record that holds the value.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="dataType">Value type used to choose the editor and validate input.</param>
    /// <param name="order">Display order of the field inside its group.</param>
    /// <param name="groupKey">Stable group key.</param>
    /// <param name="groupOrder">Group display order.</param>
    /// <param name="isReadOnly">Whether the field cannot be edited here.</param>
    /// <param name="isTranslatable">Whether the field can be translated through an overlay.</param>
    /// <param name="lookup">Optional lookup source of the field.</param>
    /// <param name="relation">Optional relation source of the field.</param>
    internal DetailFieldViewModel(
        IDetailFieldHost host,
        TableRowViewModel row,
        FieldPath path,
        ColumnDataType dataType,
        int order,
        string groupKey,
        int groupOrder,
        bool isReadOnly,
        bool isTranslatable,
        LookupColumnData? lookup = null,
        RelationColumnData? relation = null)
        : this(
            host,
            row,
            path,
            dataType,
            order,
            groupKey,
            groupOrder,
            isReadOnly || !host.IsEditable || row.Key is null,
            isTranslatable,
            !isReadOnly && dataType == ColumnDataType.Flags,
            lookup,
            relation)
    {
    }

    /// <summary>
    /// Gets the row this field belongs to.
    /// </summary>
    internal TableRowViewModel Row { get; }

    /// <summary>
    /// Gets the schema column of a top-level field, or <see langword="null"/> for a nested field.
    /// </summary>
    internal TableSchemaColumn? Column { get; }

    /// <summary>
    /// Gets the field path relative to the record.
    /// </summary>
    public FieldPath Path { get; }

    /// <summary>
    /// Gets the leaf field name shown in the form.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the slash-separated field path, which differs from <see cref="Name"/> for nested fields.
    /// </summary>
    public string PathText { get; }

    /// <summary>
    /// Gets the value type of the field.
    /// </summary>
    public ColumnDataType DataType { get; }

    /// <summary>
    /// Gets the display order of the field inside its group.
    /// </summary>
    public int Order { get; }

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
    /// Gets the field type name.
    /// </summary>
    public string TypeText { get; }

    /// <summary>
    /// Gets the stable group key used to organize the detail form.
    /// </summary>
    public string GroupKey { get; }

    /// <summary>
    /// Gets the group display order.
    /// </summary>
    public int GroupOrder { get; }

    /// <summary>
    /// Gets a value indicating whether the field cannot be edited here. Keys, repeated fields, and
    /// nested structures are read-only unless a dedicated editor hosts them.
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
    public bool CanGoToTarget => IsRelation && Relation!.TryGetTargetValue(Value, out _);

    /// <summary>
    /// Gets the tooltip shown on the field name, including relation, lookup, and type information.
    /// </summary>
    public string FieldTooltip =>
        HasLookupError ? LookupErrorText
        : HasRelationError ? RelationErrorText
        : string.Equals(PathText, Name, StringComparison.Ordinal) ? TypeText : $"{PathText} ({TypeText})";

    /// <summary>
    /// Gets the tooltip shown for a translatable field that still uses its base value.
    /// </summary>
    public string UntranslatedTooltip => Strings.Get("EditorUntranslated");

    /// <summary>
    /// Gets the tooltip of the button that opens the record referenced by a relation field.
    /// </summary>
    public string GoToTargetTooltip => Strings.Get("EditorGoToTargetText");

    /// <summary>
    /// Gets the bit editor button text.
    /// </summary>
    public string EditBitsText => Strings.Get("DetailBitsText");

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
        get => Row[PathText];
        set
        {
            if (IsReadOnly)
            {
                return;
            }

            _host.SetDetailValue(this, value);
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
    public bool IsTranslated => Row.IsTranslated(PathText);

    /// <summary>
    /// Gets a value indicating whether the field can be translated through an overlay.
    /// </summary>
    public bool IsTranslatable { get; }

    /// <summary>
    /// Gets a value indicating whether a translation is expected but the value still comes from the
    /// base table.
    /// </summary>
    public bool IsUntranslated =>
        _host.IsTranslationMode
        && IsTranslatable
        && !Row.IsTranslated(PathText)
        && !string.IsNullOrEmpty(Row[PathText]);

    /// <summary>
    /// Re-reads the value and derived state after the underlying record changed.
    /// </summary>
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
    private void EditBits() => _host.EditBits(this);

    [RelayCommand(CanExecute = nameof(CanGoToTarget))]
    private void GoToTarget() => _host.NavigateToRelation(this);
}
