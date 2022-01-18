using System.Xml;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class XmlHelper
{
    public static IEnumerable<string> ReadElements(XmlReader xmlReader, string? tagName = null)
    {
        //Support closed tags: <MyTag />
        if (xmlReader.IsEmptyElement && xmlReader.Name == tagName)
            yield break;

        while (xmlReader.Read())
        {
            if (tagName != null)
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == tagName)
                    break;
            }

            if (xmlReader.NodeType != XmlNodeType.Element)
                continue;

            yield return xmlReader.Name;
        }
    }
}