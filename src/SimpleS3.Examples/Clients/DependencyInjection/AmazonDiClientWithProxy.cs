using System;
using System.Net;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    public static class AmazonDiClientWithProxy
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region, string proxyUri)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSimpleS3(s3Config =>
            {
                s3Config.Credentials = new StringAccessKey(keyId, accessKey);
                s3Config.Region = region;
            }, new WebProxy(proxyUri));

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}