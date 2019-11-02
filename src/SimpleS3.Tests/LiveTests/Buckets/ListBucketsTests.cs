using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class ListBucketsTests : LiveTestBase
    {
        public ListBucketsTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task ListBucketsTest()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();
            await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            List<S3Bucket> list = await BucketClient.ListAllBucketsAsync().ToListAsync().ConfigureAwait(false);
            Assert.True(list.Count > 0);

            S3Bucket bucketObj = Assert.Single(list, bucket => bucket.Name == tempBucketName);
            Assert.Equal(bucketObj.CreatedOn.UtcDateTime, DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}