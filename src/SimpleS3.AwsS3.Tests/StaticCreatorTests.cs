using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.AwsS3;
using Xunit;

namespace Genbox.SimpleS3.AwsS3.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            FakeNetworkDriver driver = new FakeNetworkDriver();

            AwsConfig config = new AwsConfig();
            config.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
            config.Region = AwsRegion.UsEast1;

            S3Client client = new S3Client(config, driver);
            await client.PutObjectStringAsync("testbucket", "PutAsync", "data").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/PutAsync", driver.SendResource);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/GetObjectAsync", driver.SendResource);
        }
    }
}