using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory
{
    public class HttpClientFactoryNetworkDriver : INetworkDriver
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<HttpClientFactoryConfig> _options;
        private readonly ILogger<HttpClientFactoryNetworkDriver> _logger;
        private readonly Version _httpVersion1 = new Version("1.1");
        private readonly Version _httpVersion2 = new Version("2.0");
        private readonly Version _httpVersion3 = new Version("3.0");

        public HttpClientFactoryNetworkDriver(IOptions<HttpClientFactoryConfig> options, ILogger<HttpClientFactoryNetworkDriver> logger, IHttpClientFactory clientFactory)
        {
            _options = options;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethodType method, string url, IReadOnlyDictionary<string, string>? headers = null, Stream? dataStream = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse;
            using (HttpRequestMessage httpRequest = new HttpRequestMessage(ConvertToMethod(method), url))
            {
                if (_options.Value.HttpVersion == HttpVersion.Http1)
                    httpRequest.Version = _httpVersion1;
                else if (_options.Value.HttpVersion == HttpVersion.Http2)
                    httpRequest.Version = _httpVersion2;
                else if (_options.Value.HttpVersion == HttpVersion.Http3)
                    httpRequest.Version = _httpVersion3;
                else if (_options.Value.HttpVersion == HttpVersion.Unknown)
                {
                    //Do nothing. Use default.
                }
                else
                    throw new ArgumentOutOfRangeException();

                if (dataStream != null)
                    httpRequest.Content = new StreamContent(dataStream);

                if (headers != null)
                {
                    //Map all the headers to the HTTP request headers. We have to do this after setting the content as some headers are related to content
                    foreach (KeyValuePair<string, string> item in headers)
                    {
                        httpRequest.AddHeader(item.Key, item.Value);
                    }
                }

                _logger.LogTrace("Sending HTTP request");

                HttpClient client = _clientFactory.CreateClient(_options.Value.HttpClientName);
                httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            }

            _logger.LogDebug("Got an {status} response with {Code}", httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

            IDictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
            {
                responseHeaders.Add(header.Key, header.Value.First());
            }

            Stream? contentStream = null;

            if (httpResponse.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Content.Headers)
                {
                    responseHeaders.Add(header.Key, header.Value.First());
                }

                contentStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            return ((int)httpResponse.StatusCode, responseHeaders, contentStream);
        }

        private static System.Net.Http.HttpMethod ConvertToMethod(HttpMethodType method)
        {
            switch (method)
            {
                case HttpMethodType.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethodType.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethodType.HEAD:
                    return System.Net.Http.HttpMethod.Head;
                case HttpMethodType.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethodType.POST:
                    return System.Net.Http.HttpMethod.Post;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }
    }
}