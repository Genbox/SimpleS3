using System.Threading.Tasks;
using Xunit;

namespace Genbox.SimpleS3.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public async Task TryExtensions()
        {
            (FakeHttpHandler handler, S3Client client) = StaticClientHelper.CreateFakeClient();

            Assert.True((await client.GetObjectAsync("testbucket", "GetDataAsync").ConfigureAwait(false)).IsSuccess);
            Assert.Equal("testbucket/GetDataAsync", handler.SendResource);
        }
    }
}