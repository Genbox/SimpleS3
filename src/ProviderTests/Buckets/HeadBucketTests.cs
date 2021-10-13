using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class HeadBucketTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadBucket(S3Provider _, string bucket, ISimpleClient client)
        {
            HeadBucketResponse headResp = await client.HeadBucketAsync(bucket).ConfigureAwait(false);
            Assert.Equal(200, headResp.StatusCode);

            headResp = await client.HeadBucketAsync(GetTemporaryBucket()).ConfigureAwait(false);
            Assert.False(headResp.IsSuccess);
        }
    }
}