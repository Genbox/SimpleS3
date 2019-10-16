using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions.HttpClient
{
    public static class S3ClientBuilderExtensions
    {
        public static IS3ClientBuilder UseHttpClient(this IS3ClientBuilder builder)
        {
            builder.Services.TryAddSingleton<System.Net.Http.HttpClient>();
            builder.Services.TryAddSingleton<INetworkDriver, HttpClientNetworkDriver>();
            return builder;
        }

        public static IHttpClientBuilder UseHttpClientFactory(this IS3ClientBuilder builder)
        {
            IHttpClientBuilder httpBuilder = builder.Services.AddHttpClient<INetworkDriver, HttpClientNetworkDriver>((provider, client) =>
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent));

            return httpBuilder;
        }
    }
}