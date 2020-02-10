using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IClientBuilder UseHttpClient(this IClientBuilder builder, IOptions<HttpClientConfig> options = null)
        {
            builder.Services.AddSingleton(x =>
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None;
                handler.UseProxy = options.Value.UseProxy;
                handler.Proxy = options.Value.Proxy;
                return handler;
            });

            builder.Services.AddSingleton(serviceProvider =>
            {
                System.Net.Http.HttpClient client = ActivatorUtilities.CreateInstance<System.Net.Http.HttpClient>(serviceProvider);
                client.DefaultRequestHeaders.TransferEncodingChunked = false;

                return client;
            });

            builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>();
            return builder;
        }
    }
}