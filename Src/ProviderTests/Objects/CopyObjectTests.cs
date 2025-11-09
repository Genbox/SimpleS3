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
    public async Task CopyObject(S3Provider _, string bucket, ISimpleClient client)
    {
        //Upload an object to copy
        const string sourceKey = nameof(CopyObject);
        const string destinationKey = sourceKey + "2";

        await client.PutObjectStringAsync(bucket, sourceKey, "test");

        CopyObjectResponse copyResp = await client.CopyObjectAsync(bucket, sourceKey, bucket, destinationKey);
        Assert.Equal(200, copyResp.StatusCode);

        GetObjectResponse getResp = await client.GetObjectAsync(bucket, destinationKey);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal("test", await getResp.Content.AsStringAsync());
    }
}