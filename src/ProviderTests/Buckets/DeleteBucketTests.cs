using System.Threading.Tasks;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class DeleteBucketTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task DeleteBucket(S3Provider provider, string _, ISimpleClient client)
        {
            string tempBucketName = GetTemporaryBucket();

            DeleteBucketResponse deleteResp1 = await client.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(deleteResp1.IsSuccess);
            Assert.Equal(ErrorCode.NoSuchBucket, deleteResp1.Error?.Code);

            await client.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            DeleteBucketResponse deleteResp2 = await client.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(deleteResp2.IsSuccess);
            Assert.Equal(204, deleteResp2.StatusCode);

            ListObjectsResponse listResp = await client.ListObjectsAsync(tempBucketName).ConfigureAwait(false);
            Assert.False(listResp.IsSuccess);
            Assert.Equal(404, listResp.StatusCode);
        }
    }
}