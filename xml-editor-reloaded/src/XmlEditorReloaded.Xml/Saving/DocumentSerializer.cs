using System.Xml;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Saving;

/// <summary>
/// Serializes a <see cref="TableDocument"/> using its original encoding, BOM preference, and newline style.
/// </summary>
public sealed class DocumentSerializer
{
    /// <summary>
    /// Serializes the document to bytes without re-indenting it or normalizing its values.
    /// </summary>
    /// <param name="document">The loaded document to serialize.</param>
    /// <returns>The serialized XML bytes, including a BOM when the source file had one.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the document is not saveable.</exception>
    public byte[] Serialize(TableDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (document.Status != Core.Abstractions.TableDocumentStatus.Loaded)
        {
            throw new InvalidOperationException("Only documents with status Loaded can be serialized.");
        }

        var settings = new XmlWriterSettings
        {
            Encoding = document.Format.CreateEncoding(),
            Indent = false,
            OmitXmlDeclaration = document.Xml.Declaration is null,
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars = document.Format.NewLine,
            CloseOutput = false,
            CheckCharacters = true,
            ConformanceLevel = ConformanceLevel.Document,
        };

        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, settings))
        {
            document.Xml.Save(writer);
        }

        return stream.ToArray();
    }
}
