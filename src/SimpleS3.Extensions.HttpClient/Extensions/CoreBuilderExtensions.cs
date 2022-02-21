using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Extensions;
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

        builder.Services.Configure<HttpBuilderActions>(clientBuilder.Name, actions =>
        {
            actions.HttpHandlerActions.Add((x, handler) =>
            {
                IOptions<HttpClientConfig> options = x.GetRequiredService<IOptions<HttpClientConfig>>();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                handler.UseProxy = options.Value.UseProxy;
                handler.Proxy = options.Value.Proxy;
            });

            actions.HttpClientActions.Add((x, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });
        });

        builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>(provider =>
        {
            IOptionsMonitor<HttpBuilderActions> options = provider.GetRequiredService<IOptionsMonitor<HttpBuilderActions>>();
            HttpBuilderActions? actions = options.Get(clientBuilder.Name);

            HttpClientHandler handler = new HttpClientHandler();

            foreach (Action<IServiceProvider, HttpClientHandler>? action in actions.HttpHandlerActions)
            {
                action(provider, handler);
            }

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);

            foreach (Action<IServiceProvider, System.Net.Http.HttpClient> action in actions.HttpClientActions)
            {
                action(provider, client);
            }

            return ActivatorUtilities.CreateInstance<HttpClientNetworkDriver>(provider, client);
        });

        return builder;
    }
}