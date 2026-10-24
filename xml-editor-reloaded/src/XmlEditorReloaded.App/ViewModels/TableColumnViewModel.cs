using CommunityToolkit.Mvvm.ComponentModel;
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

    internal TableColumnViewModel(
        TableSchemaColumn column,
        Action<TableColumnViewModel, bool>? visibilityChanged = null)
    {
        ArgumentNullException.ThrowIfNull(column);
        Column = column;
        _visibilityChanged = visibilityChanged;
    }

    /// <summary>
    /// Gets the underlying schema column.
    /// </summary>
    public TableSchemaColumn Column { get; }

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
    /// Gets the preferred grid width.
    /// </summary>
    public double Width => Math.Clamp(Column.DisplayWidth ?? 120, 60, 320);

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

            return string.Join(", ", parts);
        }
    }

    internal void SetVisibleWithoutNotification(bool visible)
    {
        SetProperty(ref _isVisible, visible);
    }
}
