using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions;

public static class HttpClientBuilderExtensions
{
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

    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, IWebProxy proxy)
    {
        return UseProxy(builder, _ => proxy);
    }

    public static IHttpClientBuilder UseProxy(this IHttpClientBuilder builder, string proxyUrl) => UseProxy(builder, new WebProxy(proxyUrl));
}