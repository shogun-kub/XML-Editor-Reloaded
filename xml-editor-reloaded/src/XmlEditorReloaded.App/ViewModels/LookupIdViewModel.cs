using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Free-form lookup identifier dialog: shows the current and suggested ids, validates unsigned 64-bit
/// decimal text, rejects numeric duplicates, and warns when a combination is entered for a bit-mask lookup.
/// </summary>
public sealed partial class LookupIdViewModel : ObservableObject
{
    private readonly ulong? _currentValue;
    private readonly IReadOnlyList<ulong> _existingValues;
    private readonly bool _isFlagLookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupIdViewModel"/> class.
    /// </summary>
    /// <param name="request">Lookup identity, current id, suggestion, and existing ids.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public LookupIdViewModel(LookupIdEditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Title = request.Title;
        _isFlagLookup = request.IsFlagLookup;
        _currentValue = LookupIdAllocator.TryParseId(request.CurrentId, out var current) ? current : null;
        _existingValues = request.ExistingIds
            .Select(static text => LookupIdAllocator.TryParseId(text, out var id) ? (ulong?)id : null)
            .Where(static id => id is not null)
            .Select(static id => id!.Value)
            .ToArray();
        _id = request.CurrentId;

        SummaryText = Strings.Format("LookupIdSummaryFormat", request.CurrentId, request.SuggestedId);
        KindHint = Strings.Get(request.IsFlagLookup ? "LookupIdFlagsHint" : "LookupIdListHint");
        IdLabel = Strings.Get("LookupIdLabel");
        OkText = Strings.Get("BitEditorOk");
        CancelText = Strings.Get("BitEditorCancel");
        Validate();
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the current and suggested identifier summary.
    /// </summary>
    public string SummaryText { get; }

    /// <summary>
    /// Gets the hint that explains how a free identifier is chosen for this lookup kind.
    /// </summary>
    public string KindHint { get; }

    /// <summary>
    /// Gets the identifier field label.
    /// </summary>
    public string IdLabel { get; }

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    public string OkText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets or sets the identifier text being edited.
    /// </summary>
    [ObservableProperty]
    private string _id;

    /// <summary>
    /// Gets the validation error, or an empty string when the identifier can be accepted.
    /// </summary>
    [ObservableProperty]
    private string _validationMessage = string.Empty;

    /// <summary>
    /// Gets the non-blocking warning, for example that a bit-mask lookup received a combination.
    /// </summary>
    [ObservableProperty]
    private string _warningMessage = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the current text can be accepted.
    /// </summary>
    public bool IsValid => ValidationMessage.Length == 0;

    /// <summary>
    /// Gets the accepted canonical identifier after <see cref="TryAccept"/> succeeds.
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    /// Validates the current text and stores the canonical identifier when it is accepted.
    /// </summary>
    /// <returns><see langword="true"/> when the identifier is valid; otherwise <see langword="false"/>.</returns>
    public bool TryAccept()
    {
        Validate();
        if (!IsValid)
        {
            return false;
        }

        LookupIdAllocator.TryParseId(Id, out var id);
        Value = id.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    partial void OnIdChanged(string value) => Validate();

    private void Validate()
    {
        Value = null;
        var text = Id.Trim();
        if (text.Length == 0)
        {
            SetMessages(Strings.Get("LookupIdEmpty"), string.Empty);
            return;
        }

        if (!LookupIdAllocator.TryParseId(text, out var id))
        {
            SetMessages(
                Strings.Format(
                    "LookupIdInvalidFormat",
                    ulong.MaxValue.ToString(CultureInfo.InvariantCulture)),
                string.Empty);
            return;
        }

        if (_currentValue != id && _existingValues.Contains(id))
        {
            SetMessages(
                Strings.Format("LookupIdDuplicateFormat", id.ToString(CultureInfo.InvariantCulture)),
                string.Empty);
            return;
        }

        var warning = _isFlagLookup && id != 0 && !LookupIdAllocator.IsSingleBit(id)
            ? Strings.Format("LookupIdCombinationFormat", id.ToString(CultureInfo.InvariantCulture))
            : string.Empty;
        SetMessages(string.Empty, warning);
    }

    private void SetMessages(string validation, string warning)
    {
        ValidationMessage = validation;
        WarningMessage = warning;
        OnPropertyChanged(nameof(IsValid));
    }
}
