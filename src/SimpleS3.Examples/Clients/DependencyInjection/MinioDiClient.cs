using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    public static class MinioDiClient
    {
        public static S3Client Create()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSimpleS3(s3Config =>
            {
                s3Config.Credentials = new StringAccessKey("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");
                s3Config.Region = AwsRegion.UsEast1;
                s3Config.Endpoint = new Uri("https://play.min.io:9000/");
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}