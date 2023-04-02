using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class AbortMultipartTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task AbortIncompleteUpload(S3Provider _, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(AbortIncompleteUpload);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(200, createResp.StatusCode);
        Assert.Equal(bucket, createResp.BucketName);
        Assert.Equal(objectKey, createResp.ObjectKey);
        Assert.NotNull(createResp.UploadId);

        AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, objectKey, createResp.UploadId).ConfigureAwait(false);
        Assert.Equal(204, abortResp.StatusCode);
    }
}