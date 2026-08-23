using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Misc;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ParserHelper
{
    private static readonly Regex _expirationRegex = new Regex("expiry-date=\"(?<date>.+?)\", rule-id=\"(?<id>.+?)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

    public static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
    {
        Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, string> item in headers)
        {
            if (!item.Key.StartsWith(Constants.AmzMetadata, StringComparison.OrdinalIgnoreCase))
                continue;

            //If we crash here, it is because AWS sent us an invalid header.
            metadata[item.Key.Substring(Constants.AmzMetadata.Length)] = item.Value;
        }

        return metadata;
    }

    public static bool TryParseExpiration(IDictionary<string, string> headers, out (DateTimeOffset expiresOn, string ruleId) data)
    {
        data = default;

        if (!headers.TryGetValue(AmzHeaders.XAmzExpiration, out string? expiration))
            return false;

        Match match = _expirationRegex.Match(expiration);

        if (!match.Success)
            return false;

        data = (DateTimeOffset.ParseExact(match.Groups["date"].Value, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo), match.Groups["id"].Value);
        return true;
    }

    public static S3Identity ParseOwner(XmlReader xmlReader, string elementName = "Owner")
    {
        string? id = null;
        string? displayName = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, elementName))
        {
            switch (name)
            {
                case "ID":
                    id = xmlReader.ReadString();
                    break;
                case "DisplayName":
                    displayName = xmlReader.ReadString();

                    // To circumvent providers that return empty. We want to represent missing display names as null.
                    if (displayName == "")
                        displayName = null;
                    break;
            }
        }

        if (id == null)
            throw new InvalidOperationException("Missing required values in Owner section");

        return new S3Identity(id, displayName);
    }

    public static (S3Identity Owner, List<S3Grant> Grants) ParseAccessControlPolicy(XmlReader xmlReader)
    {
        xmlReader.ReadToDescendant("AccessControlPolicy");

        S3Identity? owner = null;
        List<S3Grant> grants = new List<S3Grant>();

        foreach (string name in XmlHelper.ReadElements(xmlReader, "AccessControlPolicy"))
        {
            switch (name)
            {
                case "Owner":
                    owner = ParseOwner(xmlReader);
                    break;
                case "AccessControlList":
                    ParseAcl(grants, xmlReader);
                    break;
            }
        }

        if (owner == null)
            throw new InvalidOperationException("Missing required values in AccessControlPolicy section");

        return (owner, grants);
    }

    private static void ParseAcl(ICollection<S3Grant> grants, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "AccessControlList"))
        {
            if (name == "Grant")
                grants.Add(ParseGrant(xmlReader));
        }
    }

    private static S3Grant ParseGrant(XmlReader xmlReader)
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
            throw new InvalidOperationException("Missing required values in Grant section");

        return new S3Grant(grantee, permission);
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