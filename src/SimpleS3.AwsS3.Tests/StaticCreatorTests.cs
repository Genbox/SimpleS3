using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Xunit;

namespace Genbox.SimpleS3.AwsS3.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            (FakeNetworkDriver driver, S3Client client) = StaticClientHelper.CreateFakeClient();

            await client.PutObjectStringAsync("testbucket", "PutAsync", "data").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/PutAsync", driver.SendResource);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/GetObjectAsync", driver.SendResource);
        }
    }
}