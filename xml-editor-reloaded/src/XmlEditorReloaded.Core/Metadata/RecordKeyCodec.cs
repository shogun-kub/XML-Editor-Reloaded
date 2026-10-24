using System.Globalization;

namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Encodes and decodes the canonical textual form of a compound record key. A canonical key is built
/// from length-prefixed components, so separators, colons, punctuation, and Unicode characters inside a
/// component never create ambiguity.
/// </summary>
/// <remarks>
/// The encoding is only used for compound keys; single-field keys keep their raw XML value so overlay,
/// copy, and navigation contracts stay unchanged. A missing component is written as <c>-</c>, an empty
/// component as <c>0:</c>, and a present component as <c>{length}:{value}</c>. Components are joined with
/// <c>|</c>.
/// </remarks>
public static class RecordKeyCodec
{
    private const char ComponentSeparator = '|';
    private const char LengthSeparator = ':';
    private const char MissingComponent = '-';

    /// <summary>
    /// Builds the canonical string of a compound key.
    /// </summary>
    /// <param name="components">
    /// Component values in key-field order. <see langword="null"/> marks an absent XML field and an empty
    /// string marks a present but empty field; the two are encoded differently.
    /// </param>
    /// <returns>The canonical key text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="components"/> is null.</exception>
    public static string Compose(IReadOnlyList<string?> components)
    {
        ArgumentNullException.ThrowIfNull(components);
        if (components.Count == 0)
        {
            return string.Empty;
        }

        var parts = new string[components.Count];
        for (var index = 0; index < components.Count; index++)
        {
            var value = components[index];
            parts[index] = value is null
                ? MissingComponent.ToString()
                : string.Concat(
                    value.Length.ToString(CultureInfo.InvariantCulture),
                    LengthSeparator.ToString(),
                    value);
        }

        return string.Join(ComponentSeparator, parts);
    }

    /// <summary>
    /// Parses a canonical compound key back into its components.
    /// </summary>
    /// <param name="key">Canonical key text produced by <see cref="Compose"/>.</param>
    /// <returns>
    /// The component values in key-field order; <see langword="null"/> entries mark absent fields.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the text is not a valid canonical compound key.</exception>
    public static IReadOnlyList<string?> Decompose(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (key.Length == 0)
        {
            return Array.Empty<string?>();
        }

        var components = new List<string?>();
        var position = 0;
        while (position < key.Length)
        {
            if (key[position] == MissingComponent
                && (position + 1 == key.Length || key[position + 1] == ComponentSeparator))
            {
                components.Add(null);
                position += 2;
                continue;
            }

            var separator = key.IndexOf(LengthSeparator, position);
            if (separator < 0
                || !int.TryParse(
                    key.AsSpan(position, separator - position),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var length)
                || length < 0)
            {
                throw new FormatException($"'{key}' is not a valid compound record key.");
            }

            var valueStart = separator + 1;
            if (valueStart + length > key.Length)
            {
                throw new FormatException($"'{key}' is not a valid compound record key.");
            }

            components.Add(key.Substring(valueStart, length));
            position = valueStart + length;
            if (position < key.Length)
            {
                if (key[position] != ComponentSeparator)
                {
                    throw new FormatException($"'{key}' is not a valid compound record key.");
                }

                position++;
                if (position == key.Length)
                {
                    throw new FormatException($"'{key}' is not a valid compound record key.");
                }
            }
        }

        return components;
    }

    /// <summary>
    /// Formats compound key components for display, for example <c>12 | 37</c>.
    /// </summary>
    /// <param name="components">Component values in key-field order.</param>
    /// <returns>Components joined by <c> | </c>; absent components are shown as an empty string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="components"/> is null.</exception>
    public static string Display(IReadOnlyList<string?> components)
    {
        ArgumentNullException.ThrowIfNull(components);
        return string.Join(" | ", components.Select(static component => component ?? string.Empty));
    }
}
