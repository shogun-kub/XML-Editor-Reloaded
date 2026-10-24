using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.IO;

namespace XmlEditorReloaded.Xml.Saving;

/// <summary>
/// Applies scalar text edits and localized structural edits directly to the original file bytes,
/// preserving declaration casing, attribute spacing, entity references, empty-element style, and the
/// indentation of neighboring elements.
/// </summary>
internal sealed class LosslessDocumentPatcher
{
    /// <summary>
    /// Attempts to build patched file bytes for a document whose changes are scalar values and
    /// insertions or deletions of single elements.
    /// </summary>
    /// <param name="document">The document whose current tree is compared with its original snapshot.</param>
    /// <param name="bytes">Patched bytes on success; an empty array otherwise.</param>
    /// <returns>
    /// <see langword="true"/> when all changes can be applied without reserializing the document;
    /// <see langword="false"/> for ambiguous structural changes, which require the regular serializer.
    /// </returns>
    public bool TryPatch(TableDocument document, out byte[] bytes)
    {
        try
        {
            return TryPatchCore(document, out bytes);
        }
        catch (Exception exception) when (exception is InvalidOperationException or XmlException or ArgumentException)
        {
            // Structural patching is an optimization; the regular serializer remains the safe fallback.
            bytes = Array.Empty<byte>();
            return false;
        }
    }

    private bool TryPatchCore(TableDocument document, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        var originalRoot = document.OriginalXml.Root;
        var currentRoot = document.Xml.Root;
        if (originalRoot is null || currentRoot is null)
        {
            return false;
        }

        var operations = new List<PatchOperation>();
        var keyField = document.Metadata?.RecordKey?.Primary.KeyField;
        if (!CollectOperations(
                originalRoot,
                currentRoot,
                new List<int>(),
                document.RecordElementName,
                keyField,
                operations))
        {
            return false;
        }

        if (operations.Count == 0)
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

        var splices = new List<Splice>();
        foreach (var operation in operations)
        {
            var applied = operation switch
            {
                ScalarChange scalar => TryCreateScalarSplice(spanRoot, text, scalar, splices),
                ElementDeletion deletion => TryCreateDeletionSplice(spanRoot, text, deletion, splices),
                ElementInsertion insertion => TryCreateInsertionSplice(
                    spanRoot,
                    text,
                    document.Format,
                    insertion,
                    splices),
                _ => false,
            };

            if (!applied)
            {
                return false;
            }
        }

        if (!CanApplySplices(splices))
        {
            return false;
        }

        foreach (var splice in splices.OrderByDescending(static item => item.Start))
        {
            text = text.Remove(splice.Start, splice.Length).Insert(splice.Start, splice.Text);
        }

        var encoded = encoding.GetBytes(text);
        var patched = preamble.Length == 0 ? encoded : preamble.Concat(encoded).ToArray();
        if (!MatchesCurrentTree(document, patched))
        {
            return false;
        }

        bytes = patched;
        return true;
    }

    private static bool CollectOperations(
        XElement original,
        XElement current,
        List<int> path,
        string recordElementName,
        string? keyField,
        List<PatchOperation> operations)
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

        if (originalChildren.Length == 0 && currentChildren.Length == 0)
        {
            if (!CommentsEqual(original, current))
            {
                return false;
            }

            if (!string.Equals(original.Value, current.Value, StringComparison.Ordinal))
            {
                operations.Add(new ScalarChange(path.ToArray(), current.Value));
            }

            return true;
        }

        var originalSignatures = BuildSignatures(originalChildren, recordElementName, keyField);
        var currentSignatures = BuildSignatures(currentChildren, recordElementName, keyField);

        var prefix = 0;
        while (prefix < originalChildren.Length
               && prefix < currentChildren.Length
               && string.Equals(originalSignatures[prefix], currentSignatures[prefix], StringComparison.Ordinal))
        {
            prefix++;
        }

        var suffix = 0;
        while (suffix < originalChildren.Length - prefix
               && suffix < currentChildren.Length - prefix
               && string.Equals(
                   originalSignatures[originalChildren.Length - 1 - suffix],
                   currentSignatures[currentChildren.Length - 1 - suffix],
                   StringComparison.Ordinal))
        {
            suffix++;
        }

        var structural = prefix + suffix < originalChildren.Length
            || prefix + suffix < currentChildren.Length;

        if (!structural)
        {
            if (!NonElementContentEqual(original, current))
            {
                return false;
            }

            for (var index = 0; index < originalChildren.Length; index++)
            {
                path.Add(index);
                var collected = CollectOperations(
                    originalChildren[index],
                    currentChildren[index],
                    path,
                    recordElementName,
                    keyField,
                    operations);
                path.RemoveAt(path.Count - 1);
                if (!collected)
                {
                    return false;
                }
            }

            return true;
        }

        for (var index = 0; index < prefix; index++)
        {
            path.Add(index);
            var collected = CollectOperations(
                originalChildren[index],
                currentChildren[index],
                path,
                recordElementName,
                keyField,
                operations);
            path.RemoveAt(path.Count - 1);
            if (!collected)
            {
                return false;
            }
        }

        for (var offset = 0; offset < suffix; offset++)
        {
            var originalIndex = originalChildren.Length - 1 - offset;
            var currentIndex = currentChildren.Length - 1 - offset;
            path.Add(originalIndex);
            var collected = CollectOperations(
                originalChildren[originalIndex],
                currentChildren[currentIndex],
                path,
                recordElementName,
                keyField,
                operations);
            path.RemoveAt(path.Count - 1);
            if (!collected)
            {
                return false;
            }
        }

        var originalMiddle = originalChildren.Length - suffix - prefix;
        var currentMiddle = currentChildren.Length - suffix - prefix;
        if (originalMiddle > 0 && currentMiddle > 0)
        {
            // A replacement block cannot be expressed as an insertion or a deletion; reserialize.
            return false;
        }

        for (var index = 0; index < originalMiddle; index++)
        {
            operations.Add(new ElementDeletion(Append(path, prefix + index)));
        }

        for (var index = 0; index < currentMiddle; index++)
        {
            operations.Add(new ElementInsertion(
                path.ToArray(),
                prefix,
                currentChildren[prefix + index]));
        }

        return true;
    }

    private static string[] BuildSignatures(
        XElement[] children,
        string recordElementName,
        string? keyField)
    {
        var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
        var signatures = new string[children.Length];
        for (var index = 0; index < children.Length; index++)
        {
            var element = children[index];
            var name = element.Name.LocalName;
            occurrences.TryGetValue(name, out var occurrence);
            occurrences[name] = occurrence + 1;

            if (string.Equals(name, recordElementName, StringComparison.Ordinal))
            {
                var key = keyField is null
                    ? element.Elements().FirstOrDefault()?.Value
                    : element.Elements()
                        .FirstOrDefault(child => string.Equals(child.Name.LocalName, keyField, StringComparison.Ordinal))
                        ?.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    signatures[index] = "R|" + name + "|" + key;
                    continue;
                }
            }

            signatures[index] = "E|" + name + "#" + occurrence;
        }

        return signatures;
    }

    private static int[] Append(List<int> path, int index)
    {
        var result = new int[path.Count + 1];
        path.CopyTo(result);
        result[^1] = index;
        return result;
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

    private static bool TryCreateScalarSplice(
        SpanNode root,
        string text,
        ScalarChange change,
        List<Splice> splices)
    {
        var node = FindNode(root, change.Path);
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
            splices.Add(new Splice(node.Start, node.TagEnd - node.Start, replacement, IsInsertion: false));
            return true;
        }

        splices.Add(new Splice(
            node.ContentStart,
            node.ContentEnd - node.ContentStart,
            Escape(change.NewText),
            IsInsertion: false));
        return true;
    }

    private static bool TryCreateDeletionSplice(
        SpanNode root,
        string text,
        ElementDeletion deletion,
        List<Splice> splices)
    {
        var node = FindNode(root, deletion.Path);
        if (node is null)
        {
            return false;
        }

        var parent = FindNode(root, deletion.Path[..^1]);
        if (parent is null)
        {
            return false;
        }

        var index = deletion.Path[^1];
        var boundary = index > 0 ? parent.Children[index - 1].End : parent.TagEnd;
        var start = node.Start;
        while (start > boundary && IsXmlWhitespace(text[start - 1]))
        {
            start--;
        }

        splices.Add(new Splice(start, node.End - start, string.Empty, IsInsertion: false));
        return true;
    }

    private static bool TryCreateInsertionSplice(
        SpanNode root,
        string text,
        FileFormatInfo format,
        ElementInsertion insertion,
        List<Splice> splices)
    {
        var parent = FindNode(root, insertion.ParentPath);
        if (parent is null || parent.Children.Count == 0 || insertion.Index > parent.Children.Count)
        {
            return false;
        }

        int position;
        string indentation;
        if (insertion.Index < parent.Children.Count)
        {
            var child = parent.Children[insertion.Index];
            var boundary = insertion.Index > 0 ? parent.Children[insertion.Index - 1].End : parent.TagEnd;
            var whitespaceStart = child.Start;
            while (whitespaceStart > boundary && IsXmlWhitespace(text[whitespaceStart - 1]))
            {
                whitespaceStart--;
            }

            indentation = text[whitespaceStart..child.Start];
            position = whitespaceStart;
        }
        else
        {
            var last = parent.Children[^1];
            var boundary = parent.Children.Count > 1 ? parent.Children[^2].End : parent.TagEnd;
            var whitespaceStart = last.Start;
            while (whitespaceStart > boundary && IsXmlWhitespace(text[whitespaceStart - 1]))
            {
                whitespaceStart--;
            }

            indentation = text[whitespaceStart..last.Start];
            position = last.End;
        }

        var indentUnit = ResolveIndentUnit(text, parent.Start, indentation);
        var baseIndentation = IndentationAfterLastNewLine(indentation);
        AddInsertion(
            splices,
            position,
            indentation + SerializeElement(insertion.Element, format, indentUnit, baseIndentation));
        return true;
    }

    private static string ResolveIndentUnit(string text, int parentStart, string childIndentation)
    {
        var childIndent = IndentationAfterLastNewLine(childIndentation);
        if (childIndent.Length == 0)
        {
            return string.Empty;
        }

        var parentIndent = GetLineIndent(text, parentStart);
        return childIndent.StartsWith(parentIndent, StringComparison.Ordinal)
               && childIndent.Length > parentIndent.Length
            ? childIndent[parentIndent.Length..]
            : string.Empty;
    }

    private static string IndentationAfterLastNewLine(string indentation)
    {
        var lastNewLine = indentation.LastIndexOf('\n');
        return lastNewLine < 0 ? indentation : indentation[(lastNewLine + 1)..];
    }

    private static string GetLineIndent(string text, int position)
    {
        var start = position;
        while (start > 0 && text[start - 1] is ' ' or '\t')
        {
            start--;
        }

        return text[start..position];
    }

    private static void AddInsertion(List<Splice> splices, int position, string text)
    {
        for (var index = 0; index < splices.Count; index++)
        {
            if (splices[index].IsInsertion && splices[index].Start == position)
            {
                splices[index] = splices[index] with { Text = splices[index].Text + text };
                return;
            }
        }

        splices.Add(new Splice(position, 0, text, IsInsertion: true));
    }

    private static bool CanApplySplices(List<Splice> splices)
    {
        var ordered = splices.OrderBy(static splice => splice.Start).ToArray();
        for (var index = 1; index < ordered.Length; index++)
        {
            var previous = ordered[index - 1];
            var current = ordered[index];
            if (current.Start == previous.Start)
            {
                // Insertions at the same position are merged; any remaining overlap is ambiguous.
                return false;
            }

            if (current.Start < previous.Start + previous.Length)
            {
                return false;
            }
        }

        return true;
    }

    private static string SerializeElement(
        XElement element,
        FileFormatInfo format,
        string indentUnit,
        string baseIndentation)
    {
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = indentUnit.Length > 0,
            IndentChars = indentUnit,
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars = format.NewLine,
            ConformanceLevel = ConformanceLevel.Document,
            CloseOutput = false,
        };

        var clone = CloneWithoutFormattingWhitespace(element);
        using var writer = new StringWriter();
        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
            clone.Save(xmlWriter);
        }

        var serialized = writer.ToString();
        if (indentUnit.Length > 0 && serialized.Contains(format.NewLine, StringComparison.Ordinal))
        {
            var lines = serialized.Split(format.NewLine);
            for (var index = 1; index < lines.Length; index++)
            {
                lines[index] = baseIndentation + lines[index];
            }

            serialized = string.Join(format.NewLine, lines);
        }

        return serialized.Replace(" />", "/>", StringComparison.Ordinal);
    }

    private static XElement CloneWithoutFormattingWhitespace(XElement element)
    {
        var clone = new XElement(element);
        RemoveFormattingWhitespace(clone);
        return clone;
    }

    private static void RemoveFormattingWhitespace(XElement element)
    {
        if (!element.HasElements)
        {
            return;
        }

        foreach (var text in element.Nodes()
            .OfType<XText>()
            .Where(static node => node is not XCData && string.IsNullOrWhiteSpace(node.Value))
            .ToList())
        {
            text.Remove();
        }

        foreach (var child in element.Elements())
        {
            RemoveFormattingWhitespace(child);
        }
    }

    private static bool MatchesCurrentTree(TableDocument document, byte[] bytes)
    {
        try
        {
            using var stream = new MemoryStream(bytes, writable: false);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreWhitespace = false,
                IgnoreComments = false,
                CloseInput = false,
            });
            var reparsed = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
            return TreesEquivalent(document.Xml.Root, reparsed.Root);
        }
        catch (XmlException)
        {
            return false;
        }
    }

    private static bool TreesEquivalent(XElement? left, XElement? right)
    {
        if (left is null || right is null)
        {
            return left is null && right is null;
        }

        if (!left.Name.Equals(right.Name) || !AttributesEqual(left, right))
        {
            return false;
        }

        var leftNodes = left.Nodes().Where(IsSignificantNode).ToArray();
        var rightNodes = right.Nodes().Where(IsSignificantNode).ToArray();
        if (leftNodes.Length != rightNodes.Length)
        {
            return false;
        }

        for (var index = 0; index < leftNodes.Length; index++)
        {
            switch (leftNodes[index], rightNodes[index])
            {
                case (XElement leftElement, XElement rightElement):
                    if (!TreesEquivalent(leftElement, rightElement))
                    {
                        return false;
                    }

                    break;
                case (XComment leftComment, XComment rightComment):
                    if (!string.Equals(leftComment.Value, rightComment.Value, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    break;
                case (XText leftText, XText rightText):
                    if (!string.Equals(leftText.Value, rightText.Value, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    private static bool IsSignificantNode(XNode node) =>
        node is not XText text || !string.IsNullOrWhiteSpace(text.Value);

    private static bool IsXmlWhitespace(char character) =>
        character is ' ' or '\t' or '\r' or '\n';

    private static string Escape(string text) =>
        text.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

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

    private abstract record PatchOperation;

    private sealed record ScalarChange(int[] Path, string NewText) : PatchOperation;

    private sealed record ElementDeletion(int[] Path) : PatchOperation;

    private sealed record ElementInsertion(int[] ParentPath, int Index, XElement Element) : PatchOperation;

    private readonly record struct Splice(int Start, int Length, string Text, bool IsInsertion);

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
}
