using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient;

public sealed class HttpClientNetworkDriver : INetworkDriver
{
    private readonly System.Net.Http.HttpClient _client;
    private readonly HttpClientConfig _config;
    private readonly Version _httpVersion1 = new Version("1.1");
    private readonly Version _httpVersion2 = new Version("2.0");
    private readonly Version _httpVersion3 = new Version("3.0");
    private readonly ILogger<HttpClientNetworkDriver> _logger;

    public HttpClientNetworkDriver(IOptions<HttpClientConfig> options, ILogger<HttpClientNetworkDriver> logger, System.Net.Http.HttpClient client)
    {
        _config = options.Value;
        _logger = logger;
        _client = client;
    }

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

            _logger.LogTrace("Sending HTTP request");

            httpResponse = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogDebug("Got an {Status} response with {StatusCode}", httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

        IDictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
            responseHeaders.Add(header.Key, header.Value.First());

        Stream? contentStream = null;

        if (httpResponse.Content != null)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Content.Headers)
                responseHeaders.Add(header.Key, header.Value.First());

            contentStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        HttpResponse returnResp = new HttpResponse();
        returnResp.Content = contentStream;
        returnResp.Headers = responseHeaders;
        returnResp.StatusCode = (int)httpResponse.StatusCode;
        return returnResp;
    }

    private static HttpMethod ConvertToMethod(HttpMethodType method)
    {
        switch (method)
        {
            case HttpMethodType.GET:
                return HttpMethod.Get;
            case HttpMethodType.PUT:
                return HttpMethod.Put;
            case HttpMethodType.HEAD:
                return HttpMethod.Head;
            case HttpMethodType.DELETE:
                return HttpMethod.Delete;
            case HttpMethodType.POST:
                return HttpMethod.Post;
            default:
                throw new ArgumentOutOfRangeException(nameof(method), method, null);
        }
    }
}