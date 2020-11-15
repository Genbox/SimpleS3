using System;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Aws;

namespace Genbox.SimpleS3.Examples.Clients.WithoutDepInjection
{
    public static class SimpleMinioClient
    {
        public static S3Client Create()
        {
            //Public minio playground credentials
            StringAccessKey key = new StringAccessKey("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");

            //Minio expect the region to always be USEast1
            AwsConfig config = new AwsConfig(key, AwsRegion.UsEast1);

            //We have to set the endpoint to point to the Minio Playground server
            config.Endpoint = new Uri("https://play.min.io:9000/");

            return new S3Client(config);
        }
    }
}