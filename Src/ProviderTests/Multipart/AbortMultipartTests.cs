using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class AbortMultipartTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task AbortMultipartUploadRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(AbortMultipartUploadRoundTrip);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, createResp.StatusCode);
        Assert.Equal(bucket, createResp.BucketName);
        Assert.Equal(objectKey, createResp.ObjectKey);
        Assert.NotNull(createResp.UploadId);

        AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, objectKey, createResp.UploadId, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, abortResp.StatusCode);
        Assert.Equal(404, (await client.ListPartsAsync(bucket, objectKey, createResp.UploadId, token: TestContext.Current.CancellationToken)).StatusCode);
    });
}