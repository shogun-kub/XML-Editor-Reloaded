namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One selectable item graphic type. Type zero is the gun sheet; other types map to the
/// <c>p&lt;type&gt;item</c> image families.
/// </summary>
public sealed class GraphicTypeOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicTypeOption"/> class.
    /// </summary>
    /// <param name="number">Graphic type value written to <c>ubGraphicType</c>.</param>
    /// <param name="text">Display text of the option.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="number"/> is negative.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
    public GraphicTypeOption(int number, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentNullException.ThrowIfNull(text);
        Number = number;
        Text = text;
    }

    /// <summary>
    /// Gets the graphic type value.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the display text.
    /// </summary>
    public string Text { get; }
}
