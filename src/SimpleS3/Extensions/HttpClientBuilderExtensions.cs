using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            return builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {Proxy = proxy});
        }
    }
}