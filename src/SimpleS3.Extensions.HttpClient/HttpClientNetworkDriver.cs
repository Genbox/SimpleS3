using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.Logging;
using HttpMethod = Genbox.SimpleS3.Core.Abstracts.Enums.HttpMethod;

namespace Genbox.SimpleS3.Extensions.HttpClient
{
    public class HttpClientNetworkDriver : INetworkDriver, IDisposable
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly ILogger<HttpClientNetworkDriver> _logger;

        public HttpClientNetworkDriver(ILogger<HttpClientNetworkDriver> logger, System.Net.Http.HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<(int statusCode, IDictionary<string, string> headers, Stream? responseStream)> SendRequestAsync(HttpMethod method, string url, IReadOnlyDictionary<string, string> headers, Stream? dataStream, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponse;
            using (HttpRequestMessage httpRequest = new HttpRequestMessage(ConvertToMethod(method), url))
            {
                if (dataStream != null)
                    httpRequest.Content = new StreamContent(dataStream);

                //Map all the headers to the HTTP request headers. We have to do this after setting the content as some headers are related to content
                foreach (KeyValuePair<string, string> item in headers)
                    httpRequest.AddHeader(item.Key, item.Value);

                _logger.LogTrace("Sending HTTP request");

                httpResponse = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            }

            _logger.LogDebug("Got an {status} response with {Code}", httpResponse.IsSuccessStatusCode ? "successful" : "unsuccessful", httpResponse.StatusCode);

            IDictionary<string, string> responseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Headers)
                responseHeaders.Add(header.Key, header.Value.First());

            Stream? responseStream = null;

            if (httpResponse.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponse.Content.Headers)
                    responseHeaders.Add(header.Key, header.Value.First());

                responseStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            return ((int)httpResponse.StatusCode, responseHeaders, responseStream);
        }

        private static System.Net.Http.HttpMethod ConvertToMethod(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.HEAD:
                    return System.Net.Http.HttpMethod.Head;
                case HttpMethod.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}