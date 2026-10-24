using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Saving;

/// <summary>
/// Applies scalar text edits directly to the original file bytes, preserving declaration casing,
/// attribute spacing, entity references, and empty-element style.
/// </summary>
internal sealed class LosslessDocumentPatcher
{
    /// <summary>
    /// Attempts to build patched file bytes for a document whose only changes are scalar text values.
    /// </summary>
    /// <param name="document">The document whose current tree is compared with its original snapshot.</param>
    /// <param name="bytes">Patched bytes on success; an empty array otherwise.</param>
    /// <returns>
    /// <see langword="true"/> when all changes can be applied without reserializing the document;
    /// <see langword="false"/> when a structural change requires the regular serializer.
    /// </returns>
    public bool TryPatch(TableDocument document, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        var originalRoot = document.OriginalXml.Root;
        var currentRoot = document.Xml.Root;
        if (originalRoot is null || currentRoot is null)
        {
            return false;
        }

        var changes = new List<TextChange>();
        if (!CollectChanges(originalRoot, currentRoot, new List<int>(), changes))
        {
            return false;
        }

        if (changes.Count == 0)
        {
            bytes = document.OriginalBytes.ToArray();
            return true;
        }

        var encoding = document.Format.CreateEncoding();
        var preamble = document.Format.HasByteOrderMark ? encoding.GetPreamble() : Array.Empty<byte>();
        var text = encoding.GetString(
            document.OriginalBytes,
            preamble.Length,
            document.OriginalBytes.Length - preamble.Length);

        var spanRoot = BuildSpanTree(text);
        if (spanRoot is null)
        {
            return false;
        }

        var replacements = new List<Replacement>();
        foreach (var change in changes)
        {
            var node = FindNode(spanRoot, change.Path);
            if (node is null)
            {
                return false;
            }

            if (node.IsEmpty)
            {
                var openTag = text[node.Start..node.TagEnd];
                if (!openTag.EndsWith("/>", StringComparison.Ordinal))
                {
                    return false;
                }

                openTag = openTag[..^2].TrimEnd() + ">";
                var replacement = openTag + Escape(change.NewText) + "</" + node.Name + ">";
                replacements.Add(new Replacement(node.Start, node.TagEnd - node.Start, replacement));
            }
            else
            {
                replacements.Add(new Replacement(
                    node.ContentStart,
                    node.ContentEnd - node.ContentStart,
                    Escape(change.NewText)));
            }
        }

        foreach (var replacement in replacements.OrderByDescending(static item => item.Start))
        {
            text = text.Remove(replacement.Start, replacement.Length)
                .Insert(replacement.Start, replacement.Text);
        }

        var encoded = encoding.GetBytes(text);
        bytes = preamble.Length == 0 ? encoded : preamble.Concat(encoded).ToArray();
        return true;
    }

    private static bool CollectChanges(
        XElement original,
        XElement current,
        List<int> path,
        List<TextChange> changes)
    {
        if (!string.Equals(original.Name.LocalName, current.Name.LocalName, StringComparison.Ordinal))
        {
            return false;
        }

        if (!AttributesEqual(original, current))
        {
            return false;
        }

        var originalChildren = original.Elements().ToArray();
        var currentChildren = current.Elements().ToArray();
        if (originalChildren.Length != currentChildren.Length)
        {
            return false;
        }

        if (originalChildren.Length == 0 && currentChildren.Length == 0)
        {
            if (!CommentsEqual(original, current))
            {
                return false;
            }

            if (!string.Equals(original.Value, current.Value, StringComparison.Ordinal))
            {
                changes.Add(new TextChange(path.ToArray(), current.Value));
            }

            return true;
        }

        if (!NonElementContentEqual(original, current))
        {
            return false;
        }

        for (var index = 0; index < originalChildren.Length; index++)
        {
            path.Add(index);
            if (!CollectChanges(originalChildren[index], currentChildren[index], path, changes))
            {
                return false;
            }

            path.RemoveAt(path.Count - 1);
        }

        return true;
    }

    private static bool AttributesEqual(XElement original, XElement current)
    {
        var originalAttributes = original.Attributes()
            .Select(static attribute => (attribute.Name.LocalName, attribute.Value))
            .ToArray();
        var currentAttributes = current.Attributes()
            .Select(static attribute => (attribute.Name.LocalName, attribute.Value))
            .ToArray();

        return originalAttributes.AsSpan().SequenceEqual(currentAttributes);
    }

    private static bool CommentsEqual(XElement original, XElement current) =>
        original.Nodes().OfType<XComment>().Select(static comment => comment.Value)
            .SequenceEqual(current.Nodes().OfType<XComment>().Select(static comment => comment.Value));

    private static bool NonElementContentEqual(XElement original, XElement current)
    {
        var originalNodes = original.Nodes().Where(static node => node is not XElement).Select(static node => node.ToString()).ToArray();
        var currentNodeList = current.Nodes().Where(static node => node is not XElement).Select(static node => node.ToString()).ToArray();
        return originalNodes.AsSpan().SequenceEqual(currentNodeList);
    }

    private static SpanNode? BuildSpanTree(string text)
    {
        var lineStarts = BuildLineStarts(text);
        using var stringReader = new StringReader(text);
        using var reader = XmlReader.Create(stringReader, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreWhitespace = false,
            IgnoreComments = false,
            CloseInput = false,
        });

        var stack = new Stack<SpanNode>();
        SpanNode? root = null;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                var start = FindTagStart(text, GetOffset(reader, lineStarts));
                if (start < 0)
                {
                    return null;
                }

                var tagEnd = FindTagEnd(text, start);
                var node = new SpanNode(reader.LocalName, start, tagEnd, reader.IsEmptyElement);

                if (stack.Count > 0)
                {
                    stack.Peek().Children.Add(node);
                }
                else
                {
                    root ??= node;
                }

                if (!reader.IsEmptyElement)
                {
                    stack.Push(node);
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                if (stack.Count == 0)
                {
                    return null;
                }

                var node = stack.Pop();
                var endStart = FindTagStart(text, GetOffset(reader, lineStarts));
                if (endStart < 0)
                {
                    return null;
                }

                node.ContentEnd = endStart;
                node.End = FindTagEnd(text, endStart);
            }
        }

        return stack.Count == 0 ? root : null;
    }

    private static int[] BuildLineStarts(string text)
    {
        var starts = new List<int> { 0 };
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == '\n')
            {
                starts.Add(index + 1);
            }
        }

        return starts.ToArray();
    }

    private static int GetOffset(XmlReader reader, int[] lineStarts)
    {
        if (reader is not IXmlLineInfo lineInfo || !lineInfo.HasLineInfo())
        {
            return 0;
        }

        var lineIndex = Math.Clamp(lineInfo.LineNumber - 1, 0, lineStarts.Length - 1);
        return lineStarts[lineIndex] + Math.Max(0, lineInfo.LinePosition - 1);
    }

    private static int FindTagStart(string text, int offset)
    {
        if (text.Length == 0)
        {
            return -1;
        }

        var index = Math.Clamp(offset, 0, text.Length - 1);
        if (text[index] == '<')
        {
            return index;
        }

        return text.LastIndexOf('<', index);
    }

    private static int FindTagEnd(string text, int start)
    {
        if (start < 0 || start >= text.Length)
        {
            return text.Length;
        }

        var quote = '\0';
        for (var index = start; index < text.Length; index++)
        {
            var character = text[index];
            if (quote != '\0')
            {
                if (character == quote)
                {
                    quote = '\0';
                }

                continue;
            }

            if (character is '\'' or '"')
            {
                quote = character;
                continue;
            }

            if (character == '>')
            {
                return index + 1;
            }
        }

        return text.Length;
    }

    private static SpanNode? FindNode(SpanNode node, IReadOnlyList<int> path)
    {
        var current = node;
        foreach (var index in path)
        {
            if (index < 0 || index >= current.Children.Count)
            {
                return null;
            }

            current = current.Children[index];
        }

        return current;
    }

    private static string Escape(string text) =>
        text.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

    private sealed class SpanNode
    {
        public SpanNode(string name, int start, int tagEnd, bool isEmpty)
        {
            Name = name;
            Start = start;
            TagEnd = tagEnd;
            ContentStart = tagEnd;
            ContentEnd = tagEnd;
            End = tagEnd;
            IsEmpty = isEmpty;
        }

        public string Name { get; }

        public int Start { get; }

        public int TagEnd { get; }

        public int ContentStart { get; }

        public int ContentEnd { get; set; }

        public int End { get; set; }

        public bool IsEmpty { get; }

        public List<SpanNode> Children { get; } = new();
    }

    private sealed record TextChange(int[] Path, string NewText);

    private sealed record Replacement(int Start, int Length, string Text);
}
