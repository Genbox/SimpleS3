using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketRequestPaymentTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketRequestPaymentRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutBucketRequestPaymentAsync(bucket, RequestPaymentPayer.Requester, token: token)).StatusCode);
        GetBucketRequestPaymentResponse read = await client.GetBucketRequestPaymentAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(RequestPaymentPayer.Requester, read.Payer);
        Assert.Equal(200, (await client.PutBucketRequestPaymentAsync(bucket, RequestPaymentPayer.BucketOwner, token: token)).StatusCode);
        GetBucketRequestPaymentResponse reset = await client.GetBucketRequestPaymentAsync(bucket, token: token);
        Assert.Equal(200, reset.StatusCode);
        Assert.Equal(RequestPaymentPayer.BucketOwner, reset.Payer);
    });
}