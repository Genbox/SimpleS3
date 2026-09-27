using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class GetObjectTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.All, "match", "mismatch", "not-match", "not-match-rejected")]
    public async Task GetObjectETagCondition(S3Provider _, string bucket, ISimpleClient client, string condition)
    {
        string key = nameof(GetObjectETagCondition) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async put =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, request =>
            {
                if (condition == "match") request.IfETagMatch.Set(put.ETag);
                if (condition == "mismatch") request.IfETagMatch.Set("different");
                if (condition == "not-match") request.IfETagNotMatch.Set("different");
                if (condition == "not-match-rejected") request.IfETagNotMatch.Set(put.ETag);
            }, TestContext.Current.CancellationToken);
            int expected = condition switch { "mismatch" => 412, "not-match-rejected" => 304, _ => 200 };
            Assert.Equal(expected, response.StatusCode);
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2, "cache-control", "disposition", "encoding", "language", "type", "expires")]
    public async Task ResponseHeaderOverride(S3Provider _, string bucket, ISimpleClient client, string header)
    {
        string key = nameof(ResponseHeaderOverride) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            DateTimeOffset expires = DateTimeOffset.UtcNow.AddHours(1);
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, request => ConfigureResponseHeaderOverride(request, header, expires), TestContext.Current.CancellationToken);
            AssertResponseHeaderOverride(response, header, expires);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetObjectRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        // Object content should:
        // - Support any combination of bytes
        // - Preserve length

        byte[] binaryData = new byte[10];
        RandomNumberGenerator.Fill(binaryData);

        string key = nameof(GetObjectRoundTrip) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectDataAsync(bucket, key, binaryData, token: TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse getResp = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(binaryData, await getResp.Content.AsDataAsync(TestContext.Current.CancellationToken));
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetObjectUnicodeContent(S3Provider _, string bucket, ISimpleClient client)
    {
        // For strings it should:
        // - Preserve casing
        // - Preserve encoding

        const string stringData = "Hello 你好 ਸਤ ਸ੍ਰੀ ਅਕਾਲ Привет";

        string key = nameof(GetObjectUnicodeContent) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, stringData, token: TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse getResp = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(stringData, await getResp.Content.AsStringAsync(token: TestContext.Current.CancellationToken));
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(GetObjectLifecycle) + Guid.NewGuid().ToString("N");
        DateTime expected = DateTime.UtcNow.AddDays(2).Date;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse getResp = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, getResp.StatusCode);

            //Test lifecycle expiration
            Assert.Equal(expected, getResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", getResp.LifeCycleRuleId);
        });
    }
}