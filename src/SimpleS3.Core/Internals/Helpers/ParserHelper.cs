using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Misc;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ParserHelper
{
    private static readonly Regex _expirationRegex = new Regex("expiry-date=\"(.+?)\", rule-id=\"(.+?)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
    {
        IDictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

        data = (DateTimeOffset.ParseExact(match.Groups[1].Value, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo), match.Groups[2].Value);
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
                    break;
            }
        }

        if (id == null)
            throw new InvalidOperationException("Missing required values in Owner section");

        return new S3Identity(id, displayName);
    }
}