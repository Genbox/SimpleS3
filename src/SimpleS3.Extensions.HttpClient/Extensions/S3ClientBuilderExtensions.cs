using System.Net.Http;
using Genbox.SimpleS3.Abstracts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IS3ClientBuilder UseHttpClient(this IS3ClientBuilder builder)
        {
            builder.Services.TryAddSingleton(x =>
            {
                SocketsHttpHandler handler = new SocketsHttpHandler();
                return new System.Net.Http.HttpClient(handler);
            });
            builder.Services.TryAddSingleton<INetworkDriver, HttpClientNetworkDriver>();
            return builder;
        }
    }
}