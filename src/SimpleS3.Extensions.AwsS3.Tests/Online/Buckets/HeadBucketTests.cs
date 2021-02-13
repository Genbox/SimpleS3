using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Buckets
{
    public class HeadBucketTests : AwsTestBase
    {
        public HeadBucketTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task ListBuckets()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();
            await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            HeadBucketResponse headResp = await BucketClient.HeadBucketAsync(tempBucketName).ConfigureAwait(false);

            Assert.Equal(200, headResp.StatusCode);
        }
    }
}