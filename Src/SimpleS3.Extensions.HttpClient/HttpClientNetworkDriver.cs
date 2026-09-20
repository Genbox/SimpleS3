using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient;

/// <summary>Provides the <c>HttpClientNetworkDriver</c> API.</summary>
public sealed partial class HttpClientNetworkDriver(IOptionsMonitor<HttpClientConfig> options, ILogger<HttpClientNetworkDriver> logger, System.Net.Http.HttpClient client, string optionsName) : INetworkDriver
{
    private readonly HttpClientConfig _config = options.Get(optionsName);
    private readonly Version _httpVersion1 = new Version("1.1");
    private readonly Version _httpVersion2 = new Version("2.0");
    private readonly Version _httpVersion3 = new Version("3.0");

    /// <summary>Provides the <c>SendRequestAsync&lt;T&gt;</c> API.</summary>
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "Transport response ownership is transferred to the returned response content.")]
    public async Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse
    {
        HttpResponseMessage httpResponse;

        // S3-010: Construct the transport URI without removing escaped dot-only key segments.
        using (HttpRequestMessage httpRequest = new HttpRequestMessage(ConvertToMethod(request.Method), UrlHelper.CreateUriWithoutPathNormalization(url)))
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

            SendingRequest();

            httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, token);
        }

        ReceivedResponse(httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

        Dictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
            responseHeaders.Add(header.Key, header.Value.First());

        ContentStream? contentStream = null;

        if (httpResponse.Content != null)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Content.Headers)
                responseHeaders.Add(header.Key, header.Value.First());

            Stream innerStream = await httpResponse.Content.ReadAsStreamAsync();
            contentStream = new ContentStream(innerStream, httpResponse.Content.Headers.ContentLength, httpResponse);
        }
        else
            httpResponse.Dispose();

        return new HttpResponse(contentStream, responseHeaders, (int)httpResponse.StatusCode);
    }

    private static HttpMethod ConvertToMethod(HttpMethodType method) => method switch
    {
        HttpMethodType.GET => HttpMethod.Get,
        HttpMethodType.PUT => HttpMethod.Put,
        HttpMethodType.HEAD => HttpMethod.Head,
        HttpMethodType.DELETE => HttpMethod.Delete,
        HttpMethodType.POST => HttpMethod.Post,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, "Unsupported HTTP method")
    };
}