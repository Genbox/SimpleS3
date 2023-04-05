using System.Net;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;

public static class CoreBuilderExtensions
{
    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder, Action<HttpClientFactoryConfig, IServiceProvider> config, string? name = null)
    {
        clientBuilder.Services.Configure(config);
        return UseHttpClientFactory(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder, Action<HttpClientFactoryConfig> config, string? name = null)
    {
        clientBuilder.Services.Configure(config);
        return UseHttpClientFactory(clientBuilder);
    }

    public static IHttpClientBuilder UseHttpClientFactory(this ICoreBuilder clientBuilder, string? name = null)
    {
        CustomHttpClientFactoryBuilder builder = new CustomHttpClientFactoryBuilder(clientBuilder.Services, name);

        //Contrary to the naming, this does not add a HttpClient to the services. It is the factories etc. necessary for HttpClientFactory to work.
        builder.Services.AddHttpClient();

        //We register the driver this way in order to support named configs in case the user want config isolation
        builder.Services.AddSingleton<INetworkDriver, HttpClientFactoryNetworkDriver>(x => ActivatorUtilities.CreateInstance<HttpClientFactoryNetworkDriver>(x, builder.Name));

        builder.Services.Configure<HttpBuilderActions>(x =>
        {
            x.HttpClientActions.Add((_, client) =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });

            x.HttpHandlerActions.Add((provider, handler) =>
            {
                IOptions<HttpClientFactoryConfig> opt = provider.GetRequiredService<IOptions<HttpClientFactoryConfig>>();
                HttpClientFactoryConfig config = opt.Value;

                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                handler.UseProxy = config.UseProxy;

                if (config.Proxy != null)
                    handler.Proxy = new WebProxy(config.Proxy);
            });
        });

        builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, (options, services) =>
        {
            IOptions<HttpBuilderActions> opt = services.GetRequiredService<IOptions<HttpBuilderActions>>();
            HttpBuilderActions actions = opt.Value;

            foreach (Action<IServiceProvider, HttpClient> httpClientAction in actions.HttpClientActions)
                options.HttpClientActions.Add(client => httpClientAction(services, client));

            options.HttpMessageHandlerBuilderActions.Add(b =>
            {
                HttpClientHandler handler = new HttpClientHandler();

                foreach (Action<IServiceProvider, HttpClientHandler> action in actions.HttpHandlerActions)
                    action(services, handler);

                b.PrimaryHandler = handler;
            });
        });

        return builder;
    }
}