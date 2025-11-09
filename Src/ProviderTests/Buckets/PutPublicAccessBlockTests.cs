using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class PutPublicAccessBlockTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutPublicAccessBlockRequest(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            PutPublicAccessBlockResponse resp = await client.PutPublicAccessBlockAsync(tempBucket);
            Assert.Equal(200, resp.StatusCode);
        });
    }
}