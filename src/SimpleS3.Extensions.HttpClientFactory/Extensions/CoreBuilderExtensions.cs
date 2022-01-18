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

        builder.Services.Configure<HttpClientFactoryOptions>((options, services) =>
        {
            IOptions<HttpClientFactoryConfig> factoryConfig = services.GetRequiredService<IOptions<HttpClientFactoryConfig>>();
            options.HandlerLifetime = factoryConfig.Value.HandlerLifetime;

            options.HttpClientActions.Add(client =>
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;
            });

            options.HttpMessageHandlerBuilderActions.Add(b =>
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
                handler.UseProxy = factoryConfig.Value.UseProxy;
                handler.Proxy = factoryConfig.Value.Proxy;

                b.PrimaryHandler = handler;
            });
        });

        return builder;
    }
}