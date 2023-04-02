using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, Func<IServiceProvider, IWebProxy> proxyFactory)
    {
        builder.Services.Configure<HttpBuilderActions>(builder.Name, actions =>
        {
            actions.HttpHandlerActions.Add((provider, handler) =>
            {
                handler.Proxy = proxyFactory(provider);
                handler.UseProxy = true;
            });
        });

        return builder;
    }

    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
    {
        return UseProxy(builder, _ => proxy);
    }

    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, string proxyUrl) => UseProxy(builder, new WebProxy(proxyUrl));
}