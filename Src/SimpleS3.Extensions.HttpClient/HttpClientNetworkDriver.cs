using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient;

public sealed class HttpClientNetworkDriver(IOptions<HttpClientConfig> options, ILogger<HttpClientNetworkDriver> logger, System.Net.Http.HttpClient client) : INetworkDriver
{
    private readonly HttpClientConfig _config = options.Value;
    private readonly Version _httpVersion1 = new Version("1.1");
    private readonly Version _httpVersion2 = new Version("2.0");
    private readonly Version _httpVersion3 = new Version("3.0");

    public async Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse
    {
        HttpResponseMessage httpResponse;
        using (HttpRequestMessage httpRequest = new HttpRequestMessage(ConvertToMethod(request.Method), url))
        {
            if (_config.HttpVersion == HttpVersion.Http1)
                httpRequest.Version = _httpVersion1;
            else if (_config.HttpVersion == HttpVersion.Http2)
                httpRequest.Version = _httpVersion2;
            else if (_config.HttpVersion == HttpVersion.Http3)
                httpRequest.Version = _httpVersion3;
            else if (_config.HttpVersion == HttpVersion.Unknown)
            {
                //Do nothing. Use default.
            }
            else
                throw new ArgumentOutOfRangeException(_config.HttpVersion + " is not a supported HTTP version");

            if (requestStream != null)
                httpRequest.Content = new StreamContent(requestStream);

            //Map all the headers to the HTTP request headers. We have to do this after setting the content as some headers are related to content
            foreach (KeyValuePair<string, string> pair in request.Headers)
                httpRequest.AddHeader(pair.Key, pair.Value);

            logger.LogTrace("Sending HTTP request");

 #pragma warning disable IDISP001 - We cannot dispose the response as the stream it delivers is reused by our response object
            httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
 #pragma warning restore IDISP001
        }
        logger.LogDebug("Got an {Status} response with {StatusCode}", httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

        Dictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
            responseHeaders.Add(header.Key, header.Value.First());

        Stream? contentStream = null;

        if (httpResponse.Content != null)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Content.Headers)
                responseHeaders.Add(header.Key, header.Value.First());

 #pragma warning disable IDISP001 - We cannot dispose as we need the response stream
            contentStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
 #pragma warning restore IDISP001
        }

        return new HttpResponse(contentStream, responseHeaders, (int)httpResponse.StatusCode);
    }

    private static HttpMethod ConvertToMethod(HttpMethodType method)
    {
        return method switch
        {
            HttpMethodType.GET => HttpMethod.Get,
            HttpMethodType.PUT => HttpMethod.Put,
            HttpMethodType.HEAD => HttpMethod.Head,
            HttpMethodType.DELETE => HttpMethod.Delete,
            HttpMethodType.POST => HttpMethod.Post,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, "Unsupported HTTP method")
        };
    }
}