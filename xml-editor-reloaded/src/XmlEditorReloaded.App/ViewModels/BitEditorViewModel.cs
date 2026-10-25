using System.Globalization;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Bit flags editor for one unsigned 64-bit integer value.
/// </summary>
public sealed class BitEditorViewModel
{
    private readonly List<BitItem> _bits = new(64);

    /// <summary>
    /// Initializes a new instance of the <see cref="BitEditorViewModel"/> class.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="value">Initial 64-bit value.</param>
    /// <param name="labels">Optional names for individual bits, keyed by bit number.</param>
    /// <param name="valueName">Optional name of the exact value, for example a combination entry.</param>
    public BitEditorViewModel(
        string title,
        ulong value,
        IReadOnlyDictionary<int, string>? labels = null,
        string? valueName = null)
    {
        Title = title;
        for (var bit = 0; bit < 64; bit++)
        {
            string? label = null;
            labels?.TryGetValue(bit, out label);
            _bits.Add(new BitItem(bit, (value & (1UL << bit)) != 0, label));
        }

        ValueSummary = valueName is null
            ? Strings.Format("BitEditorValueFormat", value.ToString(CultureInfo.InvariantCulture))
            : Strings.Format(
                "BitEditorValueWithNameFormat",
                value.ToString(CultureInfo.InvariantCulture),
                valueName);
        OkText = Strings.Get("BitEditorOk");
        CancelText = Strings.Get("BitEditorCancel");
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the summary of the initial value, including its exact lookup name when available.
    /// </summary>
    public string ValueSummary { get; }

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    public string OkText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the 64 bit items, least significant bit first.
    /// </summary>
    public IReadOnlyList<BitItem> Bits => _bits;

    /// <summary>
    /// Composes the unsigned 64-bit value from the current checkbox states.
    /// </summary>
    /// <returns>The composed value.</returns>
    public ulong ComposeValue()
    {
        var value = 0UL;
        foreach (var bit in Bits)
        {
            if (bit.IsSet)
            {
                value |= 1UL << bit.Number;
            }
        }

        return value;
    }
}

/// <summary>
/// One bit of a <see cref="BitEditorViewModel"/>.
/// </summary>
public sealed class BitItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitItem"/> class.
    /// </summary>
    /// <param name="number">Bit number, 0 to 63.</param>
    /// <param name="isSet">Whether the bit is set.</param>
    /// <param name="label">Optional lookup name of the bit.</param>
    public BitItem(int number, bool isSet, string? label = null)
    {
        Number = number;
        IsSet = isSet;
        Label = label;
    }

    /// <summary>
    /// Gets the bit number.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the bit is set.
    /// </summary>
    public bool IsSet { get; set; }

    /// <summary>
    /// Gets the lookup name of the bit, or <see langword="null"/> when the bit is unnamed.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Gets the checkbox label: the lookup name with the bit number, or just the number.
    /// </summary>
    public string DisplayText => Label is null
        ? Number.ToString(CultureInfo.InvariantCulture)
        : $"{Label} ({Number.ToString(CultureInfo.InvariantCulture)})";
}
