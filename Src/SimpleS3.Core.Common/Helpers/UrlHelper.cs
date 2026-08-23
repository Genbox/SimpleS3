using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class UrlHelper
{
    //Valid URL characters according to RFC3986: https://tools.ietf.org/html/rfc3986#section-2.3
    private const string _validUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";
    private static readonly HashSet<byte> _validUrlLookup = [..BuildLookup(_validUrlCharacters)];

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
        return string.Join("/", pathSegments.Select(segment =>
        {
            // S3-010: Dot-only key segments must remain escaped so transport cannot retarget the request.
            if (segment.Length > 0 && segment.All(c => c == '.'))
                return string.Concat(segment.Select(_ => "%2E"));

            return UrlEncode(segment);
        }));
    }

    public static Uri CreateUriWithoutPathNormalization(string url)
    {
        // S3-010: Resolve the runtime-specific option from the same core library as Uri.
        Type? optionsType = typeof(Uri).Assembly.GetType("System.UriCreationOptions");
        System.Reflection.MethodInfo? method = typeof(Uri).GetMethods().FirstOrDefault(x =>
        {
            System.Reflection.ParameterInfo[] parameters = x.GetParameters();
            return x.Name == nameof(Uri.TryCreate) && parameters.Length == 3 && parameters[1].ParameterType.GetElementType() == optionsType;
        });

        if (optionsType == null || method == null)
        {
            // S3-010: Older Uri implementations cannot represent these paths exactly; never retarget silently.
            if (ContainsEscapedDotSegment(url))
                throw new PlatformNotSupportedException("This runtime cannot send an S3 key containing a dot-only path segment without URI normalization.");

            return new Uri(url, UriKind.Absolute);
        }

        object options = Activator.CreateInstance(optionsType)!;
        optionsType.GetProperty("DangerousDisablePathAndQueryCanonicalization")!.SetValue(options, true);
        object?[] args = [url, options, null];

        if (!(bool)method.Invoke(null, args)!)
            throw new UriFormatException("Invalid request URI: " + url);

        return (Uri)args[2]!;
    }

    private static bool ContainsEscapedDotSegment(string url)
    {
        int authority = url.IndexOf("://", StringComparison.Ordinal);
        int pathStart = authority < 0 ? 0 : url.IndexOf('/', authority + 3);

        if (pathStart < 0)
            return false;

        int pathEnd = url.IndexOfAny(['?', '#'], pathStart);
        string path = pathEnd < 0 ? url.Substring(pathStart) : url.Substring(pathStart, pathEnd - pathStart);

        foreach (string segment in path.Split('/'))
        {
            if (segment.Length == 0 || segment.Length % 3 != 0)
                continue;

            bool dotOnly = true;

            for (int i = 0; i < segment.Length; i += 3)
            {
                if (!segment.Substring(i, 3).Equals("%2E", StringComparison.OrdinalIgnoreCase))
                {
                    dotOnly = false;
                    break;
                }
            }

            if (dotOnly)
                return true;
        }

        return false;
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