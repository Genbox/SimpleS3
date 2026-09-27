using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class DeleteObjectsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task MixedSuccessAndMissingVersion(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(200, (await client.PutBucketVersioningAsync(bucket, true, token: TestContext.Current.CancellationToken)).StatusCode);
        S3DeleteInfo[] resources = new S3DeleteInfo[2];
        resources[0] = new S3DeleteInfo("first");
        resources[1] = new S3DeleteInfo("second", "versionnotfound");

        PutObjectResponse putResp1 = await client.PutObjectAsync(bucket, resources[0].ObjectKey, null, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp1.StatusCode);

        PutObjectResponse putResp2 = await client.PutObjectAsync(bucket, resources[1].ObjectKey, null, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp2.StatusCode);

        DeleteObjectsResponse delResp = await client.DeleteObjectsAsync(bucket, resources, r => r.Quiet = false, TestContext.Current.CancellationToken);
        Assert.Equal(200, delResp.StatusCode);

        S3DeletedObject delObj = Assert.Single(delResp.Deleted);
        Assert.Equal(resources[0].ObjectKey, delObj.ObjectKey);
        Assert.True(delObj.IsDeleteMarker);
        Assert.NotNull(delObj.DeleteMarkerVersionId);
        Assert.NotEmpty(delObj.DeleteMarkerVersionId);

        S3DeleteError errorObj = Assert.Single(delResp.Errors);

        Assert.Equal(resources[1].ObjectKey, errorObj.ObjectKey);
        Assert.Equal(resources[1].VersionId, errorObj.VersionId);
        Assert.Equal(ErrorCode.NoSuchVersion, errorObj.Code);
        Assert.NotEmpty(errorObj.Message);
        Assert.Equal(404, (await client.HeadObjectAsync(bucket, "first", token: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal(200, (await client.HeadObjectAsync(bucket, "second", token: TestContext.Current.CancellationToken)).StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task DeleteObjectsRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        List<S3DeleteInfo> objects = [];

        foreach (string key in new[] { "first", "second" })
        {
            PutObjectResponse put = await client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, put.StatusCode);
            objects.Add(new S3DeleteInfo(key, put.VersionId));
        }

        DeleteObjectsResponse response = await client.DeleteObjectsAsync(bucket, objects, request => request.Quiet = false, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Empty(response.Errors);
        Assert.Equal(2, response.Deleted.Count);
        foreach (S3DeleteInfo item in objects)
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, item.ObjectKey, token: TestContext.Current.CancellationToken)).StatusCode);
    });
}