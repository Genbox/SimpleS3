using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Buckets
{
    public class HeadBucketTests : AwsTestBase
    {
        public HeadBucketTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task HeadBucket()
        {
            HeadBucketResponse headResp = await BucketClient.HeadBucketAsync(BucketName).ConfigureAwait(false);
            Assert.Equal(200, headResp.StatusCode);

            headResp = await BucketClient.HeadBucketAsync(GetTempBucketName()).ConfigureAwait(false);
            Assert.False(headResp.IsSuccess);
        }
    }
}