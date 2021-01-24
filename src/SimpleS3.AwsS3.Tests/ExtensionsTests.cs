using System.Threading.Tasks;
using Xunit;

namespace Genbox.SimpleS3.AwsS3.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public async Task TryExtensions()
        {
            (FakeNetworkDriver driver, S3Client client) = StaticClientHelper.CreateFakeClient();

            Assert.True((await client.GetObjectAsync("testbucket", "GetDataAsync").ConfigureAwait(false)).IsSuccess);
            Assert.Equal("https://testbucket.s3.us-east-1.amazonaws.com/GetDataAsync", driver.SendResource);
        }
    }
}