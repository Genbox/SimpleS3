using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Tests
{
    public static class StaticClientHelper
    {
        public static (FakeNetworkDriver driver, S3Client client) CreateFakeClient()
        {
            AwsConfig config = new AwsConfig(new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY"), AwsRegion.UsEast1);

            FakeNetworkDriver fakeNetworkDriver = new FakeNetworkDriver();

            S3Client client = new S3Client(Options.Create(config), fakeNetworkDriver, NullLoggerFactory.Instance);
            return (fakeNetworkDriver, client);
        }
    }
}