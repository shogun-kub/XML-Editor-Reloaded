using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Schema;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One schema column offered by the grid, including its visibility in the column chooser.
/// </summary>
public sealed class TableColumnViewModel : ObservableObject
{
    private readonly Action<TableColumnViewModel, bool>? _visibilityChanged;
    private bool _isVisible;
    private double _displayWidth;
    private int _displayOrder;

    internal TableColumnViewModel(
        TableSchemaColumn column,
        Action<TableColumnViewModel, bool>? visibilityChanged = null,
        LookupColumnData? lookup = null,
        RelationColumnData? relation = null)
    {
        ArgumentNullException.ThrowIfNull(column);
        Column = column;
        _visibilityChanged = visibilityChanged;
        _displayOrder = column.Order;
        Lookup = lookup;
        Relation = relation;
    }

    /// <summary>
    /// Gets the underlying schema column.
    /// </summary>
    public TableSchemaColumn Column { get; }

    /// <summary>
    /// Gets the loaded lookup data of the column, or <see langword="null"/> when the column has no
    /// lookup source.
    /// </summary>
    public LookupColumnData? Lookup { get; }

    /// <summary>
    /// Gets the resolved relation data of the column, or <see langword="null"/> when the field is not
    /// a relation.
    /// </summary>
    public RelationColumnData? Relation { get; }

    /// <summary>
    /// Gets the XML field name used as the column header and binding path.
    /// </summary>
    public string Name => Column.XmlFieldName;

    /// <summary>
    /// Gets or sets a value indicating whether the column is shown in the grid.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (SetProperty(ref _isVisible, value))
            {
                _visibilityChanged?.Invoke(this, value);
            }
        }
    }

    /// <summary>
    /// Gets the field type.
    /// </summary>
    public ColumnDataType DataType => Column.DataType;

    /// <summary>
    /// Gets a short description of the field type and role for the column chooser.
    /// </summary>
    public string TypeText => Column.DataType.ToString();

    /// <summary>
    /// Gets a value indicating whether this column identifies records.
    /// </summary>
    public bool IsKey => Column.IsKey;

    /// <summary>
    /// Gets a value indicating whether the field repeats inside a record.
    /// </summary>
    public bool IsList => Column.IsList;

    /// <summary>
    /// Gets a value indicating whether the field is an index-masked dictionary.
    /// </summary>
    public bool IsDict => Column.IsDict;

    /// <summary>
    /// Gets a value indicating whether the field can be translated through an overlay.
    /// </summary>
    public bool IsTranslatable => Column.IsTranslatable;

    /// <summary>
    /// Gets a value indicating whether the field is hidden by default.
    /// </summary>
    public bool IsHidden => Column.IsHidden;

    /// <summary>
    /// Gets a value indicating whether the grid must not edit this column. Keys, repeated fields, and
    /// nested structures are read-only until a dedicated editor is available.
    /// </summary>
    public bool IsReadOnly => Column.IsKey || Column.IsList || Column.IsDict || Column.IsNested;

    /// <summary>
    /// Gets a value indicating whether the grid can show a lookup combo for this column.
    /// </summary>
    public bool HasLookupCombo =>
        Column.EditorKind == ColumnEditorKind.LookupCombo
        && Lookup is { IsAvailable: true };

    /// <summary>
    /// Gets a value indicating whether a lookup source was configured but could not be loaded. Such a
    /// column falls back to plain text editing and shows a warning in its header.
    /// </summary>
    public bool HasLookupError => Lookup is { IsAvailable: false };

    /// <summary>
    /// Gets a value indicating whether the grid can show a searchable relation picker for this column.
    /// </summary>
    public bool HasRelationPicker =>
        Relation is { IsAvailable: true } && !IsReadOnly;

    /// <summary>
    /// Gets a value indicating whether a relation target was configured but could not be loaded. Such
    /// a column falls back to plain text editing and shows a warning in its header.
    /// </summary>
    public bool HasRelationError => Relation is { IsAvailable: false };

    /// <summary>
    /// Gets a value indicating whether the grid uses a checkbox editor for this column.
    /// </summary>
    public bool IsBoolean => Column.DataType == ColumnDataType.Boolean;

    /// <summary>
    /// Gets the selectable lookup options, or an empty list when no lookup is available.
    /// </summary>
    public IReadOnlyList<LookupOption> LookupOptions =>
        Lookup?.Options ?? Array.Empty<LookupOption>();

    /// <summary>
    /// Gets the column header tooltip, including a lookup or relation load error when one occurred.
    /// </summary>
    public string HeaderTooltip
    {
        get
        {
            if (HasLookupError)
            {
                return $"{Tooltip}{Environment.NewLine}{Strings.Format("LookupUnavailableFormat", Lookup!.ErrorText ?? string.Empty)}";
            }

            return HasRelationError
                ? $"{Tooltip}{Environment.NewLine}{Strings.Format("RelationUnavailableFormat", Relation!.ErrorText ?? string.Empty)}"
                : Tooltip;
        }
    }

    /// <summary>
    /// Gets the preferred grid width. A width chosen by the user takes precedence over the schema
    /// suggestion and is clamped to a readable range.
    /// </summary>
    public double Width => Math.Clamp(
        _displayWidth > 0 ? _displayWidth : Column.DisplayWidth ?? 120,
        60,
        320);

    /// <summary>
    /// Gets the zero-based display order of the column in the grid.
    /// </summary>
    public int DisplayOrder
    {
        get => _displayOrder;
        internal set => _displayOrder = value;
    }

    /// <summary>
    /// Gets the width persisted in a layout; zero means that the schema width is still in use.
    /// </summary>
    internal double PersistedWidth => _displayWidth > 0 ? _displayWidth : Width;

    /// <summary>
    /// Applies a user-resized width. The schema width is used again when <paramref name="width"/> is
    /// zero or invalid.
    /// </summary>
    /// <param name="width">Width in device-independent pixels.</param>
    internal void SetDisplayWidth(double width)
    {
        var normalized = double.IsFinite(width) && width > 0
            ? Math.Clamp(width, 60, 320)
            : 0;
        if (Math.Abs(_displayWidth - normalized) < 0.5)
        {
            return;
        }

        _displayWidth = normalized;
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(PersistedWidth));
    }

    /// <summary>
    /// Gets the tooltip text for the column header.
    /// </summary>
    public string Tooltip => Column.TooltipKey ?? Column.XmlFieldName;

    /// <summary>
    /// Gets the column description shown in the chooser, for example <c>Integer, key</c>.
    /// </summary>
    public string Description
    {
        get
        {
            var parts = new List<string> { TypeText };
            if (IsKey)
            {
                parts.Add("key");
            }

            if (IsList)
            {
                parts.Add("list");
            }

            if (IsDict)
            {
                parts.Add("dict");
            }

            if (Column.IsNested)
            {
                parts.Add("nested");
            }

            if (IsTranslatable)
            {
                parts.Add("translatable");
            }

            if (Relation is not null)
            {
                parts.Add("reference");
            }

            return string.Join(", ", parts);
        }
    }

    internal void SetVisibleWithoutNotification(bool visible)
    {
        SetProperty(ref _isVisible, visible);
    }
}
