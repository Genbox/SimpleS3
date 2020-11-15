using Genbox.SimpleS3.Core.Aws;

namespace Genbox.SimpleS3.Examples.Clients.WithoutDepInjection
{
    public static class SimpleClient
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region)
        {
            //This is the simplest way of creating S3Client
            return new S3Client(keyId, accessKey, region);
        }
    }
}