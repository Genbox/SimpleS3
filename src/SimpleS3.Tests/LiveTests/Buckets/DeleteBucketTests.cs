using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class DeleteBucketTests : LiveTestBase
    {
        public DeleteBucketTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task DeleteBucket()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            DeleteBucketResponse delete1 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(delete1.IsSuccess);
            Assert.Equal(ErrorCode.NoSuchBucket, delete1.Error.Code);

            await BucketClient.CreateBucketAsync(tempBucketName, Config.Region).ConfigureAwait(false);

            DeleteBucketResponse delete2 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(delete2.IsSuccess);
            Assert.Equal(204, delete2.StatusCode);

            ListObjectsResponse resp = await BucketClient.ListObjectsAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(resp.IsSuccess);
            Assert.Equal(404, resp.StatusCode);
        }
    }
}