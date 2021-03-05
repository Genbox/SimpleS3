using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Buckets
{
    public class ListBucketsTests : AwsTestBase
    {
        public ListBucketsTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task ListBuckets()
        {
            ListBucketsResponse listResp = await BucketClient.ListBucketsAsync().ConfigureAwait(false);
            Assert.True(listResp.Buckets.Count > 0);
            Assert.Single(listResp.Buckets, bucket => bucket.BucketName == BucketName);
        }
    }
}