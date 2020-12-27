using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            builder.Services.Configure<HttpClientConfig>(config =>
            {
                config.UseProxy = true;
                config.Proxy = proxy;
            });

            return builder;
        }

        public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, string proxyUrl)
        {
            builder.Services.Configure<HttpClientConfig>(config =>
            {
                config.UseProxy = true;
                config.Proxy = new WebProxy(proxyUrl);
            });

            return builder;
        }
    }
}