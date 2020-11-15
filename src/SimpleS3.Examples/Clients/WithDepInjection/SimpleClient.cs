﻿using System;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Aws;
using Genbox.SimpleS3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.WithDepInjection
{
    public static class SimpleClient
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSimpleS3(s3Config =>
            {
                s3Config.Credentials = new StringAccessKey(keyId, accessKey);
                s3Config.Region = region;
            });

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}