using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class ListBucketsTests : OnlineTestBase
    {
        public ListBucketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task ListBuckets()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();
            await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            ListBucketsResponse listResp = await BucketClient.ListBucketsAsync().ConfigureAwait(false);
            Assert.True(listResp.Buckets.Count > 0);

            S3Bucket? bucketObj = Assert.Single(listResp.Buckets, bucket => bucket.Name == tempBucketName);
            Assert.Equal(bucketObj.CreatedOn.UtcDateTime, DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}