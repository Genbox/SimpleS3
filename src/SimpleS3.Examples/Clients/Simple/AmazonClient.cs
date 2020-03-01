using System.Net;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Examples.Clients.Simple
{
    public static class AmazonClient
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region, IWebProxy proxy = null)
        {
            //This is the simplest way of creating S3Client
            return new S3Client(keyId, accessKey, region, proxy);
        }
    }
}