using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public class HttpClientFactoryNetworkDriver : INetworkDriver
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly HttpClientFactoryConfig _config;
    private readonly Version _httpVersion1 = new Version("1.1");
    private readonly Version _httpVersion2 = new Version("2.0");
    private readonly Version _httpVersion3 = new Version("3.0");
    private readonly ILogger<HttpClientFactoryNetworkDriver> _logger;
    private readonly string _optionsName;

    public HttpClientFactoryNetworkDriver(IOptions<HttpClientFactoryConfig> options, ILogger<HttpClientFactoryNetworkDriver> logger, IHttpClientFactory clientFactory, string optionsName)
    {
        _config = options.Value;
        _logger = logger;
        _clientFactory = clientFactory;
        _optionsName = optionsName;
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
            foreach (KeyValuePair<string, string> item in request.Headers)
                httpRequest.AddHeader(item.Key, item.Value);

            _logger.LogTrace("Sending HTTP request");

            HttpClient client = _clientFactory.CreateClient(_optionsName);
            httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogDebug("Got an {Status} response with {StatusCode}", httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

        Dictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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