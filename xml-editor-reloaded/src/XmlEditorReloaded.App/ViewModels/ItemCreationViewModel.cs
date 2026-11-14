using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model of the new-item dialog: identifier, name, and item class.
/// </summary>
public sealed partial class ItemCreationViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCreationViewModel"/> class.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="suggestedId">Initial identifier, usually the next free key.</param>
    /// <param name="suggestedName">Initial item name.</param>
    /// <param name="classes">Selectable item classes.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ItemCreationViewModel(
        string title,
        string suggestedId,
        string suggestedName,
        IReadOnlyList<LookupOption> classes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(suggestedId);
        ArgumentNullException.ThrowIfNull(suggestedName);
        ArgumentNullException.ThrowIfNull(classes);

        Title = title;
        _id = suggestedId;
        _name = suggestedName;
        Classes = classes;
        _selectedClass = classes.Count > 0 ? classes[0] : null;
        OkText = Strings.Get("BitEditorOk");
        CancelText = Strings.Get("BitEditorCancel");
        IdLabel = Strings.Get("ItemNewIdLabel");
        NameLabel = Strings.Get("ItemNewNameLabel");
        ClassLabel = Strings.Get("ItemNewClassLabel");
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    public string OkText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the identifier label.
    /// </summary>
    public string IdLabel { get; }

    /// <summary>
    /// Gets the name label.
    /// </summary>
    public string NameLabel { get; }

    /// <summary>
    /// Gets the class label.
    /// </summary>
    public string ClassLabel { get; }

    /// <summary>
    /// Gets the selectable item classes.
    /// </summary>
    public IReadOnlyList<LookupOption> Classes { get; }

    /// <summary>
    /// Gets or sets the item identifier.
    /// </summary>
    [ObservableProperty]
    private string _id;

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// Gets or sets the selected item class.
    /// </summary>
    [ObservableProperty]
    private LookupOption? _selectedClass;

    /// <summary>
    /// Gets a value indicating whether the dialog can be accepted.
    /// </summary>
    public bool IsValid =>
        int.TryParse(Id.Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var id)
        && id >= 0
        && !string.IsNullOrWhiteSpace(Name)
        && SelectedClass is not null;

    /// <summary>
    /// Creates the request from the current values.
    /// </summary>
    /// <returns>The creation request.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the values are not valid.</exception>
    public ItemCreationRequest ToRequest() => IsValid
        ? new ItemCreationRequest(Id.Trim(), Name.Trim(), SelectedClass!.IdText)
        : throw new InvalidOperationException("The item values are not valid.");

    partial void OnIdChanged(string value) => OnPropertyChanged(nameof(IsValid));

    partial void OnNameChanged(string value) => OnPropertyChanged(nameof(IsValid));

    partial void OnSelectedClassChanged(LookupOption? value) => OnPropertyChanged(nameof(IsValid));
}
