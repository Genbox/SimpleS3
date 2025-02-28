﻿using System.Net;
using System.Runtime.InteropServices;
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
        clientBuilder.Services.Configure(config);
        return UseHttpClient(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder, Action<HttpClientConfig> config)
    {
        clientBuilder.Services.Configure(config);
        return UseHttpClient(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder)
    {
        CustomHttpClientBuilder builder = new CustomHttpClientBuilder(clientBuilder.Services, clientBuilder.Name);

        builder.Services.Configure<HttpBuilderActions>(clientBuilder.Name, x =>
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")))
            {
                x.HttpHandlerActions.Add((provider, handler) =>
                {
                    IOptions<HttpClientConfig> opt = provider.GetRequiredService<IOptions<HttpClientConfig>>();
                    HttpClientConfig options = opt.Value;

                    handler.UseCookies = false;
                    handler.MaxAutomaticRedirections = 3;
                    handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                    handler.UseProxy = options.UseProxy;

                    if (options.Proxy != null)
                        handler.Proxy = new WebProxy(options.Proxy);
                });
            }

            x.HttpClientActions.Add((_, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });
        });

        builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>(provider =>
        {
            IOptions<HttpBuilderActions> opt = provider.GetRequiredService<IOptions<HttpBuilderActions>>();
            HttpBuilderActions actions = opt.Value;

            HttpClientHandler handler = new HttpClientHandler();

            foreach (Action<IServiceProvider, HttpClientHandler>? action in actions.HttpHandlerActions)
                action(provider, handler);

 #pragma warning disable IDISP001 - Disposed by the HttpClientN
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);
 #pragma warning restore IDISP001

            foreach (Action<IServiceProvider, System.Net.Http.HttpClient> action in actions.HttpClientActions)
                action(provider, client);

            return ActivatorUtilities.CreateInstance<HttpClientNetworkDriver>(provider, client);
        });

        return builder;
    }
}