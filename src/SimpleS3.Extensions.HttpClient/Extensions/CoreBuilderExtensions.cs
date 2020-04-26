using System;
using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
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
            CustomHttpClientBuilder builder = new CustomHttpClientBuilder(clientBuilder.Services);

            builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>(x =>
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use

                HttpClientConfig conf = x.GetRequiredService<HttpClientConfig>();
                handler.UseProxy = conf.UseProxy;
                handler.Proxy = conf.Proxy;

                ILogger<HttpClientNetworkDriver> logger = x.GetRequiredService<ILogger<HttpClientNetworkDriver>>();
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);
                return new HttpClientNetworkDriver(logger, client);
            });

            return builder;
        }
    }
}