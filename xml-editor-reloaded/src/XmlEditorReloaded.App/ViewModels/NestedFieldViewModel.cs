using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One editable field of the nested-record form. The field keeps its own path and delegates reading and
/// writing to the workspace so nested collections can use occurrence-aware paths.
/// </summary>
public sealed partial class NestedFieldViewModel : ObservableObject
{
    private readonly Func<string?> _read;
    private readonly Action<string> _write;
    private readonly Func<bool>? _isTranslated;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedFieldViewModel"/> class.
    /// </summary>
    /// <param name="name">Element name shown in the form.</param>
    /// <param name="pathText">Path used for display and automation.</param>
    /// <param name="read">Reads the effective value.</param>
    /// <param name="write">Writes a new raw value.</param>
    /// <param name="isReadOnly">Whether the field cannot be edited.</param>
    /// <param name="isTranslatable">Whether the field can come from the language overlay.</param>
    /// <param name="isTranslated">Reports whether the current value comes from the overlay.</param>
    internal NestedFieldViewModel(
        string name,
        string pathText,
        Func<string?> read,
        Action<string> write,
        bool isReadOnly,
        bool isTranslatable,
        Func<bool>? isTranslated)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(pathText);
        ArgumentNullException.ThrowIfNull(read);
        ArgumentNullException.ThrowIfNull(write);

        Name = name;
        PathText = pathText;
        _read = read;
        _write = write;
        IsReadOnly = isReadOnly;
        IsTranslatable = isTranslatable;
        _isTranslated = isTranslated;
    }

    /// <summary>
    /// Gets the element name shown in the form.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path text used for display and automation.
    /// </summary>
    public string PathText { get; }

    /// <summary>
    /// Gets a value indicating whether the field cannot be edited.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets a value indicating whether the field accepts input.
    /// </summary>
    public bool IsEditable => !IsReadOnly;

    /// <summary>
    /// Gets a value indicating whether the field can come from a language overlay.
    /// </summary>
    public bool IsTranslatable { get; }

    /// <summary>
    /// Gets a value indicating whether the current value comes from the overlay.
    /// </summary>
    public bool IsTranslated => IsTranslatable && _isTranslated?.Invoke() == true;

    /// <summary>
    /// Gets a value indicating whether a translatable field still shows its base value.
    /// </summary>
    public bool IsUntranslated => IsTranslatable && !IsTranslated;

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public string Value
    {
        get => _read() ?? string.Empty;
        set
        {
            if (IsReadOnly || string.Equals(value, Value, StringComparison.Ordinal))
            {
                return;
            }

            _write(value);
            Refresh();
        }
    }

    /// <summary>
    /// Gets the field type text used in the detail tooltip.
    /// </summary>
    public string TypeText => ColumnDataType.String.ToString();

    /// <summary>
    /// Gets the tooltip of the field.
    /// </summary>
    public string FieldTooltip => PathText;

    /// <summary>
    /// Gets the tooltip shown for a translatable field that still uses its base value.
    /// </summary>
    public string UntranslatedTooltip => Strings.Get("EditorUntranslated");

    /// <summary>
    /// Gets a value indicating whether the field uses a checkbox editor.
    /// </summary>
    public bool IsBoolean => false;

    /// <summary>
    /// Gets a value indicating whether the field uses a lookup combo.
    /// </summary>
    public bool IsLookup => false;

    /// <summary>
    /// Gets a value indicating whether the field uses a relation picker.
    /// </summary>
    public bool IsRelation => false;

    /// <summary>
    /// Gets a value indicating whether the field uses a plain text box.
    /// </summary>
    public bool IsPlainText => true;

    /// <summary>
    /// Gets a value indicating whether a configured source could not be loaded.
    /// </summary>
    public bool HasSourceError => false;

    /// <summary>
    /// Gets the lookup options; nested fields do not use lookups.
    /// </summary>
    public IReadOnlyList<LookupOption> LookupOptions => Array.Empty<LookupOption>();

    /// <summary>
    /// Gets the relation source; nested fields do not use relations.
    /// </summary>
    public RelationColumnData? Relation => null;

    /// <summary>
    /// Gets the maximum input length; zero means unlimited.
    /// </summary>
    public int MaxTextLength => 0;

    /// <summary>
    /// Gets a value indicating whether a character counter is shown.
    /// </summary>
    public bool ShowCharacterCounter => false;

    /// <summary>
    /// Gets the automation identifier of the character counter.
    /// </summary>
    public string CounterAutomationId => PathText + "_counter";

    /// <summary>
    /// Gets a value indicating whether the bit editor is available.
    /// </summary>
    public bool CanEditBits => false;

    /// <summary>
    /// Gets the bit editor button text.
    /// </summary>
    public string EditBitsText => Strings.Get("DetailBitsText");

    /// <summary>
    /// Gets the tooltip of the relation navigation button.
    /// </summary>
    public string GoToTargetTooltip => Strings.Get("EditorGoToTargetText");

    /// <summary>
    /// Gets a value indicating whether the field can navigate to a relation target.
    /// </summary>
    public bool CanGoToTarget => false;

    /// <summary>
    /// Gets the checkbox state of a Boolean field.
    /// </summary>
    public bool BooleanValue
    {
        get => Value.Trim() is "1" or "true" or "True";
        set => Value = value ? "1" : "0";
    }

    /// <summary>
    /// Gets the checkbox tooltip.
    /// </summary>
    public string BooleanTooltip => PathText;

    /// <summary>
    /// Gets the command that opens a relation target; it is not used by nested fields.
    /// </summary>
    public IRelayCommand GoToTargetCommand { get; } = new RelayCommand(static () => { });

    /// <summary>
    /// Gets the command that opens the bit editor; it is not used by nested fields.
    /// </summary>
    public IRelayCommand EditBitsCommand { get; } = new RelayCommand(static () => { });

    /// <summary>
    /// Raises change notifications after the underlying value changed.
    /// </summary>
    public void Refresh()
    {
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(BooleanValue));
        OnPropertyChanged(nameof(IsTranslated));
        OnPropertyChanged(nameof(IsUntranslated));
    }
}
