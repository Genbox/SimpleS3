using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects;

public class ObjectLegalHoldTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task PutGetObjectLegalHold(S3Provider _, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(PutGetObjectLegalHold);

        //Create an object
        await client.PutObjectAsync(bucket, objectKey, null).ConfigureAwait(false);

        //Check that there is no lock
        GetObjectLegalHoldResponse getLegalResp = await client.GetObjectLegalHoldAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(404, getLegalResp.StatusCode);
        Assert.False(getLegalResp.LegalHold);

        //Set a lock
        PutObjectLegalHoldResponse putLegalResp = await client.PutObjectLegalHoldAsync(bucket, objectKey, true).ConfigureAwait(false);
        Assert.Equal(200, putLegalResp.StatusCode);

        //There should be a lock now
        GetObjectLegalHoldResponse getLegalResp2 = await client.GetObjectLegalHoldAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(200, getLegalResp2.StatusCode);
        Assert.True(getLegalResp2.LegalHold);
    }
}