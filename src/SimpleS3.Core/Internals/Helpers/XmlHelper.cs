using System.Collections.Generic;
using System.Xml;

namespace Genbox.SimpleS3.Core.Internals.Helpers
{
    internal static class XmlHelper
    {
        public static IEnumerable<string> ReadElements(XmlReader xmlReader, string? tagName = null)
        {
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
}