using System.Net;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Examples.Clients.Simple
{
    public static class AmazonClientWithProxy
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region, string proxyUri)
        {
            //You have to have a proxy listening on 'proxyUri' for this to work
            return new S3Client(keyId, accessKey, region, new WebProxy(proxyUri));
        }
    }
}