using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions;

public static class CoreBuilderExtensions
{
    public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder, Action<HttpClientConfig, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(clientBuilder.Name, config);
        return UseHttpClient(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder, Action<HttpClientConfig> config)
    {
        clientBuilder.Services.Configure(clientBuilder.Name, config);
        return UseHttpClient(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder)
    {
        CustomHttpClientBuilder builder = new CustomHttpClientBuilder(clientBuilder.Services, clientBuilder.Name);

        builder.Services.Configure<HttpBuilderActions>(clientBuilder.Name, x =>
        {
            x.HttpHandlerActions.Add((provider, baseHandler) =>
            {
                //When compiled to Blazor, the baseHandler is a BrowserHttpHandler, which does not support setting the settings below
                if (baseHandler is HttpClientHandler handler)
                {
                    IOptionsMonitor<HttpClientConfig> opt = provider.GetRequiredService<IOptionsMonitor<HttpClientConfig>>();
                    HttpClientConfig options = opt.Get(builder.Name);

                    handler.UseCookies = false;
                    handler.MaxAutomaticRedirections = 3;
                    handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                    handler.UseProxy = options.UseProxy;

                    if (options.Proxy != null)
                        handler.Proxy = new WebProxy(options.Proxy);
                }
            });

            x.HttpClientActions.Add((_, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });
        });

        builder.Services.AddKeyedSingleton<INetworkDriver>(builder.Name, (provider, _) => CreateNetworkDriver(provider, builder.Name));

        if (builder.Name == ServiceBuilderBase.DefaultName)
            builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>(provider => CreateNetworkDriver(provider, builder.Name));

        return builder;
    }

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the HttpClientNetworkDriver")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "Disposed by the HttpClientNetworkDriver")]
    private static HttpClientNetworkDriver CreateNetworkDriver(IServiceProvider provider, string name)
    {
        IOptionsMonitor<HttpBuilderActions> opt = provider.GetRequiredService<IOptionsMonitor<HttpBuilderActions>>();
        HttpBuilderActions actions = opt.Get(name);

        HttpClientHandler handler = new HttpClientHandler();

        foreach (Action<IServiceProvider, HttpClientHandler>? action in actions.HttpHandlerActions)
            action(provider, handler);

        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);

        foreach (Action<IServiceProvider, System.Net.Http.HttpClient> action in actions.HttpClientActions)
            action(provider, client);

        return ActivatorUtilities.CreateInstance<HttpClientNetworkDriver>(provider, client, name);
    }
}