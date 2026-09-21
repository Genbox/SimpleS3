using System.Globalization;
using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Misc;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ParserHelper
{
    public static (bool inProgress, DateTimeOffset? expiresOn) ParseRestoreHeader(string? header)
    {
        if (header == null || !TryParseQuotedParameters(header, out Dictionary<string, string> values))
            return default;

        bool inProgress = false;
        DateTimeOffset? expiresOn = null;
        if (values.TryGetValue("ongoing-request", out string? ongoing) && !bool.TryParse(ongoing, out inProgress))
            return default;

        if (values.TryGetValue("expiry-date", out string? date))
        {
            if (!DateTimeOffset.TryParseExact(date, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset parsed))
                return default;
            expiresOn = parsed;
        }

        return (inProgress, expiresOn);
    }

    private static bool TryParseQuotedParameters(string header, out Dictionary<string, string> values)
    {
        values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        int offset = 0;

        while (offset < header.Length)
        {
            int separator = header.IndexOf('=', offset);
            if (separator < 0)
                return false;
            string name = header.Substring(offset, separator - offset).Trim();
            if (name.Length == 0 || name.IndexOfAny([',', '"']) >= 0 || values.ContainsKey(name))
                return false;
            offset = separator + 1;
            while (offset < header.Length && char.IsWhiteSpace(header[offset]))
                offset++;
            if (offset >= header.Length || header[offset] != '"')
                return false;
            int endQuote = header.IndexOf('"', ++offset);
            if (endQuote < 0)
                return false;
            values.Add(name, header.Substring(offset, endQuote - offset));
            offset = endQuote + 1;
            while (offset < header.Length && char.IsWhiteSpace(header[offset]))
                offset++;
            if (offset == header.Length)
                return true;
            if (header[offset++] != ',' || offset == header.Length)
                return false;
        }

        return false;
    }

    public static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
    {
        Dictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, string> item in headers)
        {
            if (item.Key.StartsWith(Constants.AmzMetadata, StringComparison.OrdinalIgnoreCase))
                metadata[item.Key.Substring(Constants.AmzMetadata.Length)] = item.Value;
        }

        return metadata;
    }

    public static bool TryParseExpiration(IDictionary<string, string> headers, out (DateTimeOffset expiresOn, string ruleId) data)
    {
        data = default;

        if (!headers.TryGetValue(AmzHeaders.XAmzExpiration, out string? expiration) ||
            !TryParseQuotedParameters(expiration, out Dictionary<string, string> values) ||
            !values.TryGetValue("expiry-date", out string? date) ||
            !values.TryGetValue("rule-id", out string? ruleId) ||
            !DateTimeOffset.TryParseExact(date, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTimeOffset expiresOn))
            return false;

        data = (expiresOn, WebUtility.UrlDecode(ruleId));
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
                    if (displayName.Length == 0)
                        displayName = null;
                    break;
            }
        }

        if (id == null)
            throw new InvalidOperationException($"Missing required values in {elementName} section");

        return new S3Identity(id, displayName);
    }

    public static (S3Identity? Owner, List<S3Grant> Grants) ParseAccessControlPolicy(XmlReader xmlReader)
    {
        XmlHelper.ReadToDescendant(xmlReader, "AccessControlPolicy");

        S3Identity? owner = null;
        List<S3Grant> grants = [];

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

        return (owner, grants);
    }

    private static void ParseAcl(List<S3Grant> grants, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "AccessControlList"))
        {
            if (name == "Grant")
                grants.Add(ParseGrant(xmlReader));
        }
    }

    private static S3Grant ParseGrant(XmlReader xmlReader)
    {
        S3Grantee? grantee = null;
        S3Permission permission = S3Permission.Unknown;

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

        return new S3Grant(grantee ?? new S3Grantee(GrantType.Unknown, null, null, null), permission);
    }

    private static S3Grantee ParseGrantee(XmlReader xmlReader)
    {
        const string xsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        string? typeValue = xmlReader.GetAttribute("type", xsiNamespace);
        string? id = null;
        string? displayName = null;
        string? uri = null;
        string? emailAddress = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Grantee"))
        {
            switch (name)
            {
                case "Type" when typeValue == null: typeValue = xmlReader.ReadString(); break;
                case "ID": id = xmlReader.ReadString(); break;
                case "DisplayName": displayName = xmlReader.ReadString(); break;
                case "URI": uri = xmlReader.ReadString(); break;
                case "EmailAddress": emailAddress = xmlReader.ReadString(); break;
            }
        }

        GrantType type = string.IsNullOrWhiteSpace(typeValue) ? GrantType.Unknown : Core.Enums.Enums.GrantType.Parse(typeValue);
        return new S3Grantee(type, id, displayName, uri, emailAddress);
    }
}