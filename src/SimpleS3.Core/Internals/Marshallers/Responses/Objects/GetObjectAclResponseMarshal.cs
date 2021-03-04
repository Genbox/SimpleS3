using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectAclResponseMarshal : IResponseMarshal<GetObjectAclResponse>
    {
        public void MarshalResponse(Config config, GetObjectAclResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("AccessControlPolicy");

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
                    {
                        case "Owner":
                            response.Owner = XmlHelper.ParseOwner(xmlReader);
                            break;
                        case "AccessControlList":
                            ParseAcl(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private void ParseAcl(GetObjectAclResponse response, XmlTextReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "AccessControlList")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                if (xmlReader.Name == "Grant")
                    ParseGrant(response, xmlReader);
            }
        }

        private void ParseGrant(GetObjectAclResponse response, XmlTextReader xmlReader)
        {
            S3Permission permission = S3Permission.Unknown;
            S3Grantee? grantee = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Grant")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "Grantee":
                        grantee = ParseGrantee(xmlReader);
                        break;
                    case "Permission":
                        permission = ValueHelper.ParseEnum<S3Permission>(xmlReader.ReadString());
                        break;
                }
            }

            if (grantee == null)
                throw new InvalidOperationException("Missing required values");

            response.Grants.Add(new S3Grant(grantee, permission));
        }

        private S3Grantee ParseGrantee(XmlTextReader xmlReader)
        {
            xmlReader.MoveToAttribute("xsi:type");
            xmlReader.ReadAttributeValue();

            string grantType = xmlReader.Value;

            string? id = null;
            string? displayName = null;
            string? uri = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Grantee")
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
                    case "URI":
                        uri = xmlReader.ReadString();
                        break;
                }
            }

            return new S3Grantee(ValueHelper.ParseEnum<GrantType>(grantType), id, displayName, uri);
        }
    }
}