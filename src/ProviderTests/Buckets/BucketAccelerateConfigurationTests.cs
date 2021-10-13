using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketAccelerateConfigurationTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutGetBucketAccelerateConfiguration(S3Provider provider, IProfile _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async x =>
            {
                GetBucketAccelerateConfigurationResponse getResp = await client.GetBucketAccelerateConfigurationAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.AccelerateEnabled);

                PutBucketAccelerateConfigurationResponse putResp = await client.PutBucketAccelerateConfigurationAsync(x, true).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                getResp = await client.GetBucketAccelerateConfigurationAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);
                Assert.True(getResp.AccelerateEnabled);
            }).ConfigureAwait(false);
        }
    }
}