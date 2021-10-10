using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketTaggingTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetPutDeleteBucketTagging(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                IDictionary<string, string> tags = new Dictionary<string, string>();
                tags.Add("MyKey", "MyValue");
                tags.Add("MyKey2", "MyValue2");

                PutBucketTaggingResponse putResp = await client.PutBucketTaggingAsync(tempBucket, tags).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                Assert.Equal(tags, getResp.Tags);

                DeleteBucketTaggingResponse deleteResp = await client.DeleteBucketTaggingAsync(tempBucket).ConfigureAwait(false);
                Assert.True(deleteResp.IsSuccess);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetEmptyBucketTagging(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                GetBucketTaggingResponse getResp = await client.GetBucketTaggingAsync(tempBucket).ConfigureAwait(false);
                Assert.Equal(404, getResp.StatusCode);
            }).ConfigureAwait(false);
        }
    }
}