using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class GetObjectAclResponseMarshal : IResponseMarshal<GetObjectAclResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectAclResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlTextReader xmlReader = new XmlTextReader(responseStream);
        xmlReader.ReadToDescendant("AccessControlPolicy");

        foreach (string name in XmlHelper.ReadElements(xmlReader, "AccessControlPolicy"))
        {
            switch (name)
            {
                case "Owner":
                    response.Owner = ParserHelper.ParseOwner(xmlReader);
                    break;
                case "AccessControlList":
                    ParseAcl(response, xmlReader);
                    break;
            }
        }
    }

    private static void ParseAcl(GetObjectAclResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "AccessControlList"))
        {
            if (name == "Grant")
                ParseGrant(response, xmlReader);
        }
    }

    private static void ParseGrant(GetObjectAclResponse response, XmlReader xmlReader)
    {
        S3Permission permission = S3Permission.Unknown;
        S3Grantee? grantee = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Grant"))
        {
            switch (name)
            {
                case "Grantee":
                    grantee = ParseGrantee(xmlReader);
                    break;
                case "Permission":
                    permission = Core.Enums.Enums.S3Permission.Parse(xmlReader.ReadString(), S3PermissionFormat.DisplayName);
                    break;
            }
        }

        if (grantee == null)
            throw new InvalidOperationException("Missing required values");

        response.Grants.Add(new S3Grant(grantee, permission));
    }

    private static S3Grantee ParseGrantee(XmlReader xmlReader)
    {
        xmlReader.MoveToAttribute("xsi:type");
        xmlReader.ReadAttributeValue();

        string grantType = xmlReader.Value;

        string? id = null;
        string? displayName = null;
        string? uri = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Grantee"))
        {
            switch (name)
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

        return new S3Grantee(Core.Enums.Enums.GrantType.Parse(grantType), id, displayName, uri);
    }
}