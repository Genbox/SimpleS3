using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectRetentionTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task PutObjectRetentionRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse uploaded = await client.PutObjectAsync(bucket, "retained", null, token: token);
        Assert.Equal(200, uploaded.StatusCode);
        DateTimeOffset until = DateTimeOffset.UtcNow.AddMinutes(5);
        PutObjectRetentionResponse put = await client.PutObjectRetentionAsync(bucket, "retained", LockMode.Governance, until, request => request.VersionId = uploaded.VersionId, token);
        Assert.Equal(200, put.StatusCode);
        GetObjectRetentionResponse read = await client.GetObjectRetentionAsync(bucket, "retained", request => request.VersionId = uploaded.VersionId, token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(LockMode.Governance, read.LockMode);
        Assert.NotNull(read.LockRetainUntil);
        Assert.Equal(until.UtcDateTime, read.LockRetainUntil.Value.UtcDateTime, TimeSpan.FromSeconds(1));
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task GovernanceRetentionCanBeShortenedWithBypass(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse uploaded = await client.PutObjectAsync(bucket, "retained", null, token: token);
        Assert.Equal(200, uploaded.StatusCode);
        Assert.Equal(200, (await client.PutObjectRetentionAsync(bucket, "retained", LockMode.Governance, DateTimeOffset.UtcNow.AddHours(1), token: token)).StatusCode);
        DateTimeOffset shortened = DateTimeOffset.UtcNow.AddMinutes(5);
        Assert.Equal(200, (await client.PutObjectRetentionAsync(bucket, "retained", LockMode.Governance, shortened, request => request.BypassGovernanceRetention = true, token)).StatusCode);
        GetObjectRetentionResponse read = await client.GetObjectRetentionAsync(bucket, "retained", token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.NotNull(read.LockRetainUntil);
        Assert.Equal(shortened.UtcDateTime, read.LockRetainUntil.Value.UtcDateTime, TimeSpan.FromSeconds(1));
    }, request => request.EnableObjectLocking = true);
}