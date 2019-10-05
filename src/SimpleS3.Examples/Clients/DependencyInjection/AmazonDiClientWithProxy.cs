using System;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    public static class AmazonDiClientWithProxy
    {
        public static S3Client Create(string keyId, string accessKey, string proxyUri)
        {
            ServiceCollection services = new ServiceCollection();
            IS3ClientBuilder clientBuilder = services.AddSimpleS3Core(s3Config =>
            {
                s3Config.Credentials = new StringAccessKey(keyId, accessKey);
                s3Config.Region = AwsRegion.EUWest1;
            });

            IHttpClientBuilder httpBuilder = clientBuilder.UseHttpClientFactory();
            httpBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {Proxy = new WebProxy(proxyUri)});

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}