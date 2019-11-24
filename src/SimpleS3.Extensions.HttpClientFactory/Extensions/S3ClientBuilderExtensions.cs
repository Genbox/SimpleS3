using System;
using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IHttpClientBuilder UseHttpClientFactory(this IS3ClientBuilder clientBuilder, Action<HttpClientFactoryConfig> config = null)
        {
            clientBuilder.Services.AddHttpClient();

            CustomHttpClientBuilder builder = new CustomHttpClientBuilder(clientBuilder.Services);

            builder.Services.AddSingleton<IConfigureOptions<HttpClientFactoryOptions>>(x =>
            {
                return new ConfigureNamedOptions<HttpClientFactoryOptions>(builder.Name, options =>
                {
                    IOptions<HttpClientFactoryConfig> o = x.GetService<IOptions<HttpClientFactoryConfig>>();
                    options.HandlerLifetime = o.Value.HandlerLifetime;

                    options.HttpClientActions.Add(client => client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent));
                });
            });

            if (config != null)
                builder.Services.Configure(config);

            builder.Services.AddSingleton(x =>
            {
                IHttpClientFactory httpClientFactory = x.GetRequiredService<IHttpClientFactory>();
                return httpClientFactory.CreateClient("SimpleS3");
            });

            builder.Services.AddSingleton<INetworkDriver, HttpClientFactoryNetworkDriver>();

            builder.Services.AddSingleton(x =>
            {
                IOptions<HttpClientFactoryConfig> options = x.GetService<IOptions<HttpClientFactoryConfig>>();

                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None;

                if (options != null)
                {
                    handler.UseProxy = options.Value.UseProxy;
                    handler.Proxy = options.Value.Proxy;
                }

                return handler;
            });

            return builder;
        }
    }
}