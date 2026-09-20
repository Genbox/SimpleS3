using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions;

/// <summary>Provides the <c>HttpClientBuilderExtensions</c> API.</summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>Provides the <c>UseProxy</c> API.</summary>
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, Func<IServiceProvider, IWebProxy> proxyFactory)
    {
        builder.Services.Configure<HttpBuilderActions>(builder.Name, actions =>
        {
            actions.HttpHandlerActions.Add((provider, baseHandler) =>
            {
                //When compiled to Blazor, the baseHandler is a BrowserHttpHandler, which does not support setting the settings below
                if (baseHandler is HttpClientHandler handler)
                {
                    handler.Proxy = proxyFactory(provider);
                    handler.UseProxy = true;
                }
            });
        });

        return builder;
    }

    /// <summary>Provides the <c>UseProxy</c> API.</summary>
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy) => builder.UseProxy(_ => proxy);

    /// <summary>Provides the <c>UseProxy</c> API.</summary>
    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, string proxyUrl) => builder.UseProxy(new WebProxy(proxyUrl));
}