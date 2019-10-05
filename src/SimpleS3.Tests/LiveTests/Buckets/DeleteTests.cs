using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class DeleteTests : LiveTestBase
    {
        public DeleteTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task DeleteBucket()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            DeleteBucketResponse delete1 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(delete1.IsSuccess);
            Assert.Equal(ErrorCode.NoSuchBucket, delete1.Error.Code);

            await BucketClient.PutBucketAsync(tempBucketName, request => request.Region = Config.Region).ConfigureAwait(false);

            DeleteBucketResponse delete2 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(delete2.IsSuccess);
            Assert.Equal(204, delete2.StatusCode);

            GetBucketResponse resp = await BucketClient.GetBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(resp.IsSuccess);
            Assert.Equal(404, resp.StatusCode);
        }
    }
}