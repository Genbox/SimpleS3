using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;

public static class CoreBuilderExtensions
{
    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder, Action<HttpClientFactoryConfig, IServiceProvider> config)
    {
        clientBuilder.Services.Configure(config);
        return UseHttpClientFactory(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder, Action<HttpClientFactoryConfig> config)
    {
        clientBuilder.Services.Configure(config);
        return UseHttpClientFactory(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder)
    {
        CustomHttpClientFactoryBuilder builder = new CustomHttpClientFactoryBuilder(clientBuilder.Services);

        //Contrary to the naming, this does not add a HttpClient to the services. It is the factories etc. necessary for HttpClientFactory to work.
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<INetworkDriver, HttpClientFactoryNetworkDriver>();

        builder.Services.Configure<HttpBuilderActions>(clientBuilder.Name, x =>
        {
            x.HttpClientActions.Add((_, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });

            x.HttpHandlerActions.Add((provider, handler) =>
            {
                IOptionsMonitor<HttpClientFactoryConfig>? options = provider.GetRequiredService<IOptionsMonitor<HttpClientFactoryConfig>>();
                HttpClientFactoryConfig? config = options.Get(clientBuilder.Name);

                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                handler.UseProxy = config.UseProxy;
                handler.Proxy = config.Proxy;
            });
        });

        builder.Services.Configure<HttpClientFactoryOptions>(clientBuilder.Name, (options, services) =>
        {
            HttpBuilderActions actions = services.GetRequiredService<HttpBuilderActions>();

            foreach (Action<IServiceProvider, HttpClient> httpClientAction in actions.HttpClientActions)
            {
                options.HttpClientActions.Add(client => httpClientAction(services, client));
            }

            options.HttpMessageHandlerBuilderActions.Add(b =>
            {
                HttpClientHandler handler = new HttpClientHandler();

                foreach (Action<IServiceProvider, HttpClientHandler> action in actions.HttpHandlerActions)
                {
                    action(services, handler);
                }

                b.PrimaryHandler = handler;
            });
        });

        return builder;
    }
}