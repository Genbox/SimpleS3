using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class DeleteBucketTests : OnlineTestBase
    {
        public DeleteBucketTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task DeleteBucket()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            DeleteBucketResponse deleteResp1 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(deleteResp1.IsSuccess);
            Assert.Equal(ErrorCode.NoSuchBucket, deleteResp1.Error?.Code);

            await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            DeleteBucketResponse deleteResp2 = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(deleteResp2.IsSuccess);
            Assert.Equal(204, deleteResp2.StatusCode);

            ListObjectsResponse listResp = await ObjectClient.ListObjectsAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(listResp.IsSuccess);
            Assert.Equal(404, listResp.StatusCode);
        }
    }
}