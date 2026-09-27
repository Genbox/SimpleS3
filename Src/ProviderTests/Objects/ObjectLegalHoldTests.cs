using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectLegalHoldTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task PutObjectLegalHoldRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(PutObjectLegalHoldRoundTrip);

        //Create an object
        Assert.Equal(200, (await client.PutObjectAsync(bucket, objectKey, null, token: TestContext.Current.CancellationToken)).StatusCode);

        //Check that there is no lock
        GetObjectLegalHoldResponse getLegalResp = await client.GetObjectLegalHoldAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(404, getLegalResp.StatusCode);
        Assert.False(getLegalResp.LegalHold);

        //Set a lock
        PutObjectLegalHoldResponse putLegalResp = await client.PutObjectLegalHoldAsync(bucket, objectKey, true, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, putLegalResp.StatusCode);

        //There should be a lock now
        GetObjectLegalHoldResponse getLegalResp2 = await client.GetObjectLegalHoldAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getLegalResp2.StatusCode);
        Assert.True(getLegalResp2.LegalHold);
    }, request => request.EnableObjectLocking = true);
}