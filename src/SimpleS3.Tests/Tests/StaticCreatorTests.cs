using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Tests.Code.Helpers;
using Genbox.SimpleS3.Tests.Code.Other;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class StaticCreatorTests
    {
        [Fact]
        public async Task StaticClient()
        {
            (FakeHttpHandler handler, S3Client client) = StaticClientHelper.CreateFakeClient();

            await client.PutObjectStringAsync("testbucket", "PutAsync", "data").ConfigureAwait(false);
            Assert.Equal("testbucket/PutAsync", handler.SendResource);

            await client.GetObjectAsync("testbucket", "GetObjectAsync").ConfigureAwait(false);
            Assert.Equal("testbucket/GetObjectAsync", handler.SendResource);
        }
    }
}