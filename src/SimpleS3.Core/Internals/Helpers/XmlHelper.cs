using System;
using System.Xml;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers
{
    internal static class XmlHelper
    {
        public static S3Identity ParseOwner(XmlTextReader xmlReader, string elementName = "Owner")
        {
            string? id = null;
            string? displayName = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == elementName)
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "ID":
                        id = xmlReader.ReadString();
                        break;
                    case "DisplayName":
                        displayName = xmlReader.ReadString();
                        break;
                }
            }

            if (id == null || displayName == null)
                throw new InvalidOperationException("Missing required values in Owner section");

            return new S3Identity(id, displayName);
        }
    }
}