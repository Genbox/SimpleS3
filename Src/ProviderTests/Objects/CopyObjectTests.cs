using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class CopyObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task CopyObjectRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        //Upload an object to copy
        const string sourceKey = nameof(CopyObjectRoundTrip);
        const string destinationKey = sourceKey + "2";

        Assert.Equal(200, (await client.PutObjectStringAsync(bucket, sourceKey, "test", token: TestContext.Current.CancellationToken)).StatusCode);

        CopyObjectResponse copyResp = await client.CopyObjectAsync(bucket, sourceKey, bucket, destinationKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, copyResp.StatusCode);

        await AssertObjectContentAsync(client, bucket, destinationKey, "test"u8.ToArray());
    });
}