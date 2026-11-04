using System.Globalization;

namespace XmlEditorReloaded.Xml.Lookups;

/// <summary>
/// Chooses free identifiers for lookup values. Lookup identifiers are unsigned 64-bit integers because
/// <c>Lookup/ItemFlag.xml</c> already uses values above <see cref="int.MaxValue"/>.
/// </summary>
public static class LookupIdAllocator
{
    /// <summary>
    /// Parses a lookup identifier written as an unsigned decimal integer. Signs, whitespace-only input,
    /// hexadecimal notation, and values above <see cref="ulong.MaxValue"/> are rejected.
    /// </summary>
    /// <param name="text">Text to parse, for example <c>16</c>.</param>
    /// <param name="id">Parsed identifier when the method succeeds.</param>
    /// <returns><see langword="true"/> when <paramref name="text"/> is a valid identifier.</returns>
    public static bool TryParseId(string? text, out ulong id)
    {
        id = 0;
        return text is not null
            && ulong.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out id);
    }

    /// <summary>
    /// Returns the identifier a new lookup value should receive.
    /// </summary>
    /// <param name="rawIds">Identifiers already present in the lookup, in any order; invalid text is ignored.</param>
    /// <param name="isFlagLookup">
    /// When <see langword="true"/>, ids are bit flags and the lowest unused bit is returned. Otherwise the
    /// maximum id plus one is returned, so a removed identifier is never silently reused.
    /// </param>
    /// <returns>
    /// The suggested identifier, or <see langword="null"/> when the lookup has no free id left
    /// (<see cref="ulong.MaxValue"/> for a sequential lookup or all 64 bits for a flag lookup).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rawIds"/> is null.</exception>
    public static ulong? SuggestNextId(IEnumerable<string> rawIds, bool isFlagLookup)
    {
        ArgumentNullException.ThrowIfNull(rawIds);

        var maximum = 0UL;
        var usedBits = 0UL;
        var any = false;
        foreach (var rawId in rawIds)
        {
            if (!TryParseId(rawId, out var id))
            {
                continue;
            }

            any = true;
            maximum = Math.Max(maximum, id);
            usedBits |= id;
        }

        if (isFlagLookup)
        {
            // A flag lookup names single bits and prepared combinations, so the first zero bit is free
            // even when a combination with a higher value already exists (AttachmentPoint 7 before 8).
            for (var bit = 0; bit < 64; bit++)
            {
                var candidate = 1UL << bit;
                if ((usedBits & candidate) == 0)
                {
                    return candidate;
                }
            }

            return null;
        }

        if (!any)
        {
            return 0;
        }

        return maximum == ulong.MaxValue ? null : maximum + 1;
    }

    /// <summary>
    /// Returns a value indicating whether an identifier is exactly one bit, for example <c>16</c>.
    /// </summary>
    /// <param name="id">Identifier to test.</param>
    /// <returns><see langword="true"/> for <c>1</c>, <c>2</c>, <c>4</c>, and so on; <see langword="false"/> for zero and combinations.</returns>
    public static bool IsSingleBit(ulong id) => id != 0 && (id & (id - 1)) == 0;
}
