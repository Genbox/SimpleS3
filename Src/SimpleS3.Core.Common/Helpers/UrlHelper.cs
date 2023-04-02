using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class UrlHelper
{
    //Valid URL characters according to RFC3986: https://tools.ietf.org/html/rfc3986#section-2.3
    private const string _validUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";

    private static readonly HashSet<byte> _validUrlLookup = new HashSet<byte>(BuildLookup(_validUrlCharacters));

    private static IEnumerable<byte> BuildLookup(string charList)
    {
        foreach (char c in charList)
        {
            string escaped = Uri.EscapeUriString(c.ToString(CultureInfo.InvariantCulture));
            if (escaped.Length == 1 && escaped[0] == c)
                yield return (byte)c;
        }
    }

    public static string UrlPathEncode(string input)
    {
        string[] pathSegments = input.Split('/');
        return string.Join("/", pathSegments.Select(UrlEncode));
    }

    public static string UrlEncode(string data)
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent(data.Length * 3);

        foreach (byte symbol in Encoding.UTF8.GetBytes(data))
        {
            if (_validUrlLookup.Contains(symbol))
                sb.Append((char)symbol);
            else
                sb.Append('%').AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", symbol);
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }

    public static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> parameters, bool encode = true, bool outputEqualOnEmpty = false)
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent(512);

        foreach (KeyValuePair<string, string> item in parameters)
        {
            if (sb.Length > 0)
                sb.Append('&');

            string encodedKey = encode ? UrlEncode(item.Key) : item.Key;

            if (string.IsNullOrEmpty(item.Value))
            {
                sb.Append(encodedKey);

                if (outputEqualOnEmpty)
                    sb.Append('=');
            }
            else
                sb.Append(encodedKey).Append('=').Append(encode ? UrlEncode(item.Value) : item.Value);
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }
}