using System;
using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

                IOptions<HttpClientConfig> options = x.GetService<IOptions<HttpClientConfig>>();

                if (options != null)
                {
                    handler.UseProxy = options.Value.UseProxy;
                    handler.Proxy = options.Value.Proxy;
                }

                ILogger<HttpClientNetworkDriver> logger = x.GetRequiredService<ILogger<HttpClientNetworkDriver>>();

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;

                return new HttpClientNetworkDriver(logger, client);
            });

            return builder;
        }
    }
}