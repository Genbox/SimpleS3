using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class RestoreObjectTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task RestoreObjectRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        const string key = "archived-content";
        byte[] data = "restored content"u8.ToArray();
        PutObjectResponse put = await client.PutObjectDataAsync(bucket, key, data, request => request.StorageClass = StorageClass.Glacier, token);
        Assert.Equal(200, put.StatusCode);
        RestoreObjectResponse restore = await client.RestoreObjectAsync(bucket, key, request =>
        {
            request.Days = 1;
            request.GlacierTier = RetrievalTier.Expedited;
        }, token);
        Assert.Equal(202, restore.StatusCode);
        await ReadExpectedAsync(t => client.HeadObjectAsync(bucket, key, token: t),
            response => response.IsSuccess && !response.RestoreInProgress && response.RestoreExpiresOn.HasValue,
            "the expedited Glacier restore to complete", token, 91, TimeSpan.FromSeconds(10));
        await AssertObjectContentAsync(client, bucket, key, data);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task RestoreRequestAccepted(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        //Upload an object to glacier
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(RestoreRequestAccepted), null, r => r.StorageClass = StorageClass.Glacier, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);
        Assert.Equal(StorageClass.Glacier, putResp.StorageClass);

        RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucket, nameof(RestoreRequestAccepted), r => r.Days = 2, TestContext.Current.CancellationToken);

        Assert.Equal(202, restoreResp.StatusCode);
    });
}