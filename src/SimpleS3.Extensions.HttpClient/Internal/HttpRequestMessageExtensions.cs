using Genbox.SimpleS3.Core.Common.Constants;

namespace Genbox.SimpleS3.Extensions.HttpClient.Internal;

internal static class HttpRequestMessageExtensions
{
    private static readonly ISet<string> _contentHeaders = new HashSet<string>
    {
        HttpHeaders.ContentDisposition,
        HttpHeaders.ContentEncoding,
        HttpHeaders.ContentLanguage,
        HttpHeaders.ContentLength,
        HttpHeaders.ContentLocation,
        HttpHeaders.ContentMd5,
        HttpHeaders.ContentRange,
        HttpHeaders.ContentType,
        HttpHeaders.Expires,
        HttpHeaders.LastModified
    };

    public static void AddHeader(this HttpRequestMessage request, string key, string value)
    {
        string keyLowered = key.ToLowerInvariant();

        if (_contentHeaders.Contains(keyLowered))
            request.Content?.Headers.TryAddWithoutValidation(keyLowered, value);
        else
            request.Headers.TryAddWithoutValidation(keyLowered, value);
    }
}