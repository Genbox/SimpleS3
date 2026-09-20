using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Provides the <c>UrlHelper</c> API.</summary>
[SuppressMessage("Design", "CA1062", Justification = "String encoding helpers preserve null-input failures from the underlying string and encoding APIs.")]
[SuppressMessage("Design", "CA1055", Justification = "These encoders return escaped URL components, not complete URIs.")]
public static class UrlHelper
{
    //Valid URL characters according to RFC3986: https://tools.ietf.org/html/rfc3986#section-2.3
    private const string _validUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";
    private static readonly Func<string, Uri> _uriFactory = CreateUriFactory();
    private static readonly HashSet<byte> _validUrlLookup = [.. BuildLookup(_validUrlCharacters)];

    private static IEnumerable<byte> BuildLookup(string charList)
    {
        foreach (char c in charList)
        {
            string escaped = Uri.EscapeUriString(c.ToString(CultureInfo.InvariantCulture));
            if (escaped.Length == 1 && escaped[0] == c)
                yield return (byte)c;
        }
    }

    /// <summary>Provides the <c>UrlPathEncode</c> API.</summary>
    public static string UrlPathEncode(string input)
    {
        string[] pathSegments = input.Split('/');
        return string.Join("/", pathSegments.Select(segment =>
        {
            // S3-010: Dot-only key segments must remain escaped so transport cannot retarget the request.
            if (segment is "." or "..")
                return string.Concat(segment.Select(_ => "%2E"));

            return UrlEncode(segment);
        }));
    }

    /// <summary>Provides the <c>CreateUriWithoutPathNormalization</c> API.</summary>
    [SuppressMessage("Design", "CA1054", Justification = "This factory accepts the original URL text specifically to avoid normalization by an already-constructed Uri.")]
    public static Uri CreateUriWithoutPathNormalization(string url) => _uriFactory(url);

    [SuppressMessage("Major Code Smell", "S3011", Justification = "The reflected method is private code in this type and only closes over the runtime UriCreationOptions value type.")]
    private static Func<string, Uri> CreateUriFactory()
    {
        // S3-010: Resolve the runtime-specific option from the same core library as Uri.
        Type? optionsType = typeof(Uri).Assembly.GetType("System.UriCreationOptions");
        MethodInfo? method = typeof(Uri).GetMethods().FirstOrDefault(x =>
        {
            ParameterInfo[] parameters = x.GetParameters();
            return x.Name == nameof(Uri.TryCreate) && parameters.Length == 3 && parameters[1].ParameterType.GetElementType() == optionsType;
        });
        PropertyInfo? disableCanonicalization = optionsType?.GetProperty("DangerousDisablePathAndQueryCanonicalization");

        if (optionsType == null || method == null || disableCanonicalization == null || !optionsType.IsValueType)
            return CreateNormalizedUriWithSafetyCheck;

        try
        {
            MethodInfo factory = typeof(UrlHelper).GetMethod(nameof(CreateUriFactoryCore), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)!.MakeGenericMethod(optionsType);
            return (Func<string, Uri>)factory.Invoke(null, [method, disableCanonicalization])!;
        }
        catch (Exception)
        {
            return CreateNormalizedUriWithSafetyCheck;
        }
    }

    private static Func<string, Uri> CreateUriFactoryCore<TOptions>(MethodInfo method, PropertyInfo disableCanonicalization) where TOptions : struct
    {
        object boxedOptions = default(TOptions);
        disableCanonicalization.SetValue(boxedOptions, true);
        TOptions options = (TOptions)boxedOptions;
        TryCreateUri<TOptions> tryCreate = (TryCreateUri<TOptions>)method.CreateDelegate(typeof(TryCreateUri<TOptions>));

        return url =>
        {
            TOptions invocationOptions = options;

            if (!tryCreate(url, in invocationOptions, out Uri result))
                throw new UriFormatException("Invalid request URI: " + url);

            return result;
        };
    }

    private static Uri CreateNormalizedUriWithSafetyCheck(string url)
    {
        // S3-010: Older Uri implementations cannot represent these paths exactly; never retarget silently.
        if (ContainsEscapedDotSegment(url))
            throw new PlatformNotSupportedException("This runtime cannot send an S3 key containing a dot-only path segment without URI normalization.");

        return new Uri(url, UriKind.Absolute);
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
            if (segment.Equals("%2E", StringComparison.OrdinalIgnoreCase) || segment.Equals("%2E%2E", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>Provides the <c>UrlEncode</c> API.</summary>
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

    /// <summary>Provides the <c>CreateQueryString</c> API.</summary>
    public static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> parameters, bool encode = true, bool outputEqualOnEmpty = false)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

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

    private delegate bool TryCreateUri<TOptions>(string url, in TOptions options, out Uri result) where TOptions : struct;
}