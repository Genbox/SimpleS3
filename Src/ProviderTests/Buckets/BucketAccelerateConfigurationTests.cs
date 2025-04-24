using Genbox.ProviderTests.Code;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketAccelerateConfigurationTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutGetBucketAccelerateConfiguration(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async x =>
        {
            GetBucketAccelerateConfigurationResponse getResp = await client.GetBucketAccelerateConfigurationAsync(x);
            Assert.True(getResp.IsSuccess);
            Assert.False(getResp.AccelerateEnabled);

            PutBucketAccelerateConfigurationResponse putResp = await client.PutBucketAccelerateConfigurationAsync(x, true);
            Assert.True(putResp.IsSuccess);

            getResp = await client.GetBucketAccelerateConfigurationAsync(x);
            Assert.True(getResp.IsSuccess);
            Assert.True(getResp.AccelerateEnabled);
        });
    }
}