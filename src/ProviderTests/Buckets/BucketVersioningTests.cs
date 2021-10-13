using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketVersioningTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutBucketVersioningRequest(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                //Check if versioning is enabled (it shouldn't be)
                GetBucketVersioningResponse getResp = await client.GetBucketVersioningAsync(tempBucket);
                Assert.Equal(200, getResp.StatusCode);
                Assert.False(getResp.MfaDelete);

                if (provider != S3Provider.BackBlazeB2) //backblaze always return true for versioning
                    Assert.False(getResp.Status);

                //Enable versioning
                PutBucketVersioningResponse putResp = await client.PutBucketVersioningAsync(tempBucket, true);
                Assert.Equal(200, putResp.StatusCode);

                //Check if versioning is enabled (it should be)
                GetBucketVersioningResponse getResp2 = await client.GetBucketVersioningAsync(tempBucket);
                Assert.Equal(200, getResp2.StatusCode);
                Assert.True(getResp2.Status);

                //Disable versioning
                PutBucketVersioningResponse putResp2 = await client.PutBucketVersioningAsync(tempBucket, false);
                Assert.Equal(200, putResp2.StatusCode);

                //Check if versioning is enabled (it shouldn't be)
                GetBucketVersioningResponse getResp3 = await client.GetBucketVersioningAsync(tempBucket);
                Assert.Equal(200, putResp2.StatusCode);
                Assert.False(getResp3.Status);
            }).ConfigureAwait(false);
        }
    }
}