using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketVersioningTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutBucketVersioningRequest(S3Provider provider, IProfile  _, ISimpleClient client)
        {
            await CreateTempBucketAsync(client, async tempBucket =>
            {
                //Check if versioning is enabled (it shouldn't be)
                GetBucketVersioningResponse getResp = await client.GetBucketVersioningAsync(tempBucket);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.Status);
                Assert.False(getResp.MfaDelete);

                //Enable versioning
                PutBucketVersioningResponse putResp = await client.PutBucketVersioningAsync(tempBucket, true);
                Assert.True(putResp.IsSuccess);

                //Check if versioning is enabled (it should be)
                getResp = await client.GetBucketVersioningAsync(tempBucket);
                Assert.True(getResp.IsSuccess);
                Assert.True(getResp.Status);

                //Disable versioning
                putResp = await client.PutBucketVersioningAsync(tempBucket, false);
                Assert.True(putResp.IsSuccess);

                //Check if versioning is enabled (it shouldn't be)
                getResp = await client.GetBucketVersioningAsync(tempBucket);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.Status);
            }).ConfigureAwait(false);
        }
    }
}