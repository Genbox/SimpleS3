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
        const string objectKey = nameof(AbortIncompleteUpload);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey);
        Assert.Equal(200, createResp.StatusCode);
        Assert.Equal(bucket, createResp.BucketName);
        Assert.Equal(objectKey, createResp.ObjectKey);
        Assert.NotNull(createResp.UploadId);

        AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, objectKey, createResp.UploadId);
        Assert.Equal(204, abortResp.StatusCode);
    }
}