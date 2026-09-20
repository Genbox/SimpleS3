using System.Xml;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class XmlHelper
{
    private static readonly XmlReaderSettings _settings = new XmlReaderSettings
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null
    };

    public static XmlReader CreateReader(Stream stream) => XmlReader.Create(stream, _settings);

    public static bool ReadToDescendant(XmlReader reader, string localName)
    {
        int depth = reader.ReadState == ReadState.Initial ? -1 : reader.Depth;
        if (reader.ReadState != ReadState.Initial && (reader.NodeType != XmlNodeType.Element || reader.IsEmptyElement))
            return false;

        while (reader.Read() && reader.Depth > depth)
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == localName)
                return true;
        }

        return false;
    }

    public static bool ReadToFollowing(XmlReader reader, string localName)
    {
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == localName)
                return true;
        }

        return false;
    }

    // The reader must be positioned on the parent element. Consumers may read child subtrees.
    public static IEnumerable<string> ReadChildren(XmlReader reader)
    {
        if (reader.IsEmptyElement)
            yield break;
        int parentDepth = reader.Depth;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == parentDepth)
                yield break;
            if (reader.NodeType == XmlNodeType.Element && reader.Depth == parentDepth + 1)
                yield return reader.LocalName;
        }
    }

    public static IEnumerable<string> ReadElements(XmlReader xmlReader, string? tagName = null)
    {
        //Support closed tags: <MyTag />
        if (xmlReader.IsEmptyElement && xmlReader.LocalName == tagName)
            yield break;

        while (xmlReader.Read())
        {
            if (tagName != null && xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == tagName)
                break;

            if (xmlReader.NodeType != XmlNodeType.Element)
                continue;

            yield return xmlReader.LocalName;
        }
    }
}