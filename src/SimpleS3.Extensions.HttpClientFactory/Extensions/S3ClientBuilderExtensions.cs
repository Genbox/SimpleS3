using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Misc;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IHttpClientBuilder UseHttpClientFactory(this IS3ClientBuilder builder)
        {
            IHttpClientBuilder httpBuilder = builder.Services.AddHttpClient<INetworkDriver, HttpClientFactoryNetworkDriver>((provider, client) =>
                client.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.DefaultUserAgent));

            return httpBuilder;
        }
    }
}