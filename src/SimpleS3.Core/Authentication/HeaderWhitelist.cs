using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Authentication;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Authentication;

[PublicAPI]
public static class HeaderWhitelist
{
    //These headers are always signed
    public static ISet<string> Whitelist { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        HttpHeaders.Host,
        HttpHeaders.ContentType,
        HttpHeaders.ContentMd5
    };

    public static Func<string, bool> ShouldSignHeader { get; set; } = DefaultHeaderCheck;

    private static bool DefaultHeaderCheck(string header)
    {
        //Only amz headers: https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        if (header.StartsWith(SigningConstants.AmazonHeaderPrefix, StringComparison.Ordinal))
            return true;

        return Whitelist.Contains(header);
    }

    internal static IEnumerable<KeyValuePair<string, string>> FilterHeaders(IEnumerable<KeyValuePair<string, string>> headers)
    {
        foreach (KeyValuePair<string, string> item in headers.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            string loweredKey = item.Key.ToLowerInvariant();

            if (ShouldSignHeader(loweredKey))
                yield return new KeyValuePair<string, string>(loweredKey, item.Value);
        }
    }
}