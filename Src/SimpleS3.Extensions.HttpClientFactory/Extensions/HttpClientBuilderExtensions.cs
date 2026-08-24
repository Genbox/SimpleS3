using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;

/// <summary>Provides the <c>HttpClientBuilderExtensions</c> API.</summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>Provides the <c>UseProxy</c> API.</summary>
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

    /// <summary>Provides the <c>UseProxy</c> API.</summary>
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
    {
        return UseProxy(builder, _ => proxy);
    }

    /// <summary>Provides the <c>UseProxy</c> API.</summary>
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, string proxyUrl) => UseProxy(builder, new WebProxy(proxyUrl));
}