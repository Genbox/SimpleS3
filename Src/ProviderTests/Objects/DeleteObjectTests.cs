using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class DeleteObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task DeleteObjectRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(DeleteObjectRoundTrip), null, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        DeleteObjectResponse delREsp = await client.DeleteObjectAsync(bucket, nameof(DeleteObjectRoundTrip), token: TestContext.Current.CancellationToken);
        Assert.Equal(204, delREsp.StatusCode);

        Assert.Equal(404, (await client.HeadObjectAsync(bucket, nameof(DeleteObjectRoundTrip), token: TestContext.Current.CancellationToken)).StatusCode);
    });
}