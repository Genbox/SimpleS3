using System.Net.Http;
using System.Security.Authentication;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IS3ClientBuilder UseHttpClient(this IS3ClientBuilder builder, IOptions<HttpClientConfig> options = null)
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

            builder.Services.AddSingleton<System.Net.Http.HttpClient>();
            builder.Services.AddSingleton<INetworkDriver, HttpClientNetworkDriver>();
            return builder;
        }
    }
}