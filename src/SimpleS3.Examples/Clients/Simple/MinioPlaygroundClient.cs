using System;
using System.Net;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;

namespace Genbox.SimpleS3.Examples.Clients.Simple
{
    public static class MinioPlaygroundClient
    {
        public static S3Client Create(IWebProxy proxy = null)
        {
            //Public minio playground credentials
            StringAccessKey key = new StringAccessKey("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");

            //Minio expect the region to always be USEast1
            S3Config config = new S3Config(key, AwsRegion.UsEast1);

            //We have to set the endpoint to point to the Minio Playground server
            config.Endpoint = new Uri("https://play.min.io:9000/");

            return new S3Client(config, proxy);
        }
    }
}