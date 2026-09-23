using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class RestoreObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task Restore(S3Provider _, string bucket, ISimpleClient client)
    {
        //Upload an object to glacier
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(Restore), null, r => r.StorageClass = StorageClass.Glacier, TestContext.Current.CancellationToken);
        Assert.Equal(StorageClass.Glacier, putResp.StorageClass);

        RestoreObjectResponse restoreResp = await client.RestoreObjectAsync(bucket, nameof(Restore), r => r.Days = 2, TestContext.Current.CancellationToken);

        Assert.Equal(202, restoreResp.StatusCode);
    }
}