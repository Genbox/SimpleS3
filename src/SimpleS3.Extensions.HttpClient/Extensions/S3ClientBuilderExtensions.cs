using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.HttpClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IHttpClientBuilder UseHttpClient(this ICoreBuilder clientBuilder, IOptions<HttpClientConfig> options = null)
        {
            CustomHttpClientBuilder builder = new CustomHttpClientBuilder(clientBuilder.Services);

            builder.Services.AddSingleton(x =>
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = false;
                handler.MaxAutomaticRedirections = 3;
                handler.SslProtocols = SslProtocols.None; //Let the OS handle the protocol to use
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