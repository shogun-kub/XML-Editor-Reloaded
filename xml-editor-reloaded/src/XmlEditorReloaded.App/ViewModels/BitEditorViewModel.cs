using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Bit flags editor for one 32-bit integer value.
/// </summary>
public sealed class BitEditorViewModel
{
    private readonly List<BitItem> _bits = new(32);

    /// <summary>
    /// Initializes a new instance of the <see cref="BitEditorViewModel"/> class.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="value">Initial 32-bit value.</param>
    public BitEditorViewModel(string title, uint value)
    {
        Title = title;
        for (var bit = 0; bit < 32; bit++)
        {
            _bits.Add(new BitItem(bit, (value & (1u << bit)) != 0));
        }

        OkText = Strings.Get("BitEditorOk");
        CancelText = Strings.Get("BitEditorCancel");
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
    /// Gets the 32 bit items, least significant bit first.
    /// </summary>
    public IReadOnlyList<BitItem> Bits => _bits;

    /// <summary>
    /// Composes the unsigned 32-bit value from the current checkbox states.
    /// </summary>
    /// <returns>The composed value.</returns>
    public uint ComposeValue()
    {
        var value = 0u;
        foreach (var bit in Bits)
        {
            if (bit.IsSet)
            {
                value |= 1u << bit.Number;
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
    /// <param name="number">Bit number, 0 to 31.</param>
    /// <param name="isSet">Whether the bit is set.</param>
    public BitItem(int number, bool isSet)
    {
        Number = number;
        IsSet = isSet;
    }

    /// <summary>
    /// Gets the bit number.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the bit is set.
    /// </summary>
    public bool IsSet { get; set; }
}
