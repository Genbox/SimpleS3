using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Extensions.AmazonS3;

namespace Genbox.SimpleS3.Examples.Advanced;

public static class ClientSetup
{
    /// <summary>Shows how to use a simple client to work with S3</summary>
    public static void Example()
    {
        //Not much to it. Give it credentials, a region and you are ready to use S3.
        using AmazonS3Client client = new AmazonS3Client("YourKeyId", "YourAccessKey", AmazonS3Region.EuNorth1);
    }
}