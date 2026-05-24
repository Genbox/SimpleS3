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

    public static IEnumerable<string> ReadElements(XmlReader xmlReader, string? tagName = null)
    {
        //Support closed tags: <MyTag />
        if (xmlReader.IsEmptyElement && xmlReader.Name == tagName)
            yield break;

        while (xmlReader.Read())
        {
            if (tagName != null && xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tagName)
                break;

            if (xmlReader.NodeType != XmlNodeType.Element)
                continue;

            yield return xmlReader.Name;
        }
    }
}