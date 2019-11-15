using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Core.Internal.Constants;

namespace Genbox.SimpleS3.Core.Internal.Helpers
{
    internal static class HeaderParserHelper
    {
        private static Regex expirationRegex = new Regex("expiry-date=\"(.+?)\", rule-id=\"(.+?)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
        {
            string _metadataHeader = "x-amz-meta-";

            IDictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> item in headers)
            {
                if (!item.Key.StartsWith(_metadataHeader, StringComparison.OrdinalIgnoreCase))
                    continue;

                //If we crash here, it is because AWS sent us an invalid header.
                metadata[item.Key.Substring(_metadataHeader.Length)] = item.Value;
            }

            return metadata;
        }

        public static bool TryParseExpiration(IDictionary<string, string> headers, out (DateTimeOffset expiresOn, string ruleId) data)
        {
            data = default;

            if (!headers.TryGetValue(AmzHeaders.XAmzExpiration, out string expiration))
                return false;

            Match match = expirationRegex.Match(expiration);

            if (!match.Success)
                return false;

            data = (DateTimeOffset.ParseExact(match.Groups[1].Value, DateTimeFormats.Rfc1123, DateTimeFormatInfo.InvariantInfo), match.Groups[2].Value);
            return true;
        }
    }
}
