using System.Security.Cryptography;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects;

public class GetObjectTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetObjectData(S3Provider _, string bucket, ISimpleClient client)
    {
        // Object content should:
        // - Support any combination of bytes
        // - Preserve length

        byte[] binaryData = new byte[10];
        RandomNumberGenerator.Fill(binaryData);

        PutObjectResponse putResp = await client.PutObjectDataAsync(bucket, nameof(GetObjectData), binaryData).ConfigureAwait(false);
        Assert.Equal(200, putResp.StatusCode);

        GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(GetObjectData)).ConfigureAwait(false);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(binaryData, await getResp.Content.AsDataAsync());
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetObjectString(S3Provider _, string bucket, ISimpleClient client)
    {
        // For strings it should:
        // - Preserve casing
        // - Preserve encoding

        string stringData = "Hello 你好 ਸਤ ਸ੍ਰੀ ਅਕਾਲ Привет";

        PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, nameof(GetObjectString), stringData).ConfigureAwait(false);
        Assert.Equal(200, putResp.StatusCode);

        GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(GetObjectString)).ConfigureAwait(false);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(stringData, await getResp.Content.AsStringAsync());
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task GetObjectContentRange(S3Provider _, string bucket, ISimpleClient client)
    {
        await client.PutObjectStringAsync(bucket, nameof(GetObjectContentRange), "test").ConfigureAwait(false);
        GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(GetObjectContentRange), r => r.Range.Add(0, 2)).ConfigureAwait(false);

        Assert.Equal(206, getResp.StatusCode);
        Assert.Equal(3, getResp.ContentLength);
        Assert.Equal("bytes", getResp.AcceptRanges);
        Assert.Equal("bytes 0-2/4", getResp.ContentRange);
        Assert.Equal("tes", await getResp.Content.AsStringAsync().ConfigureAwait(false));
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
    {
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(GetObjectLifecycle), null).ConfigureAwait(false);

        //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
        Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
        Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);

        GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(GetObjectLifecycle)).ConfigureAwait(false);
        Assert.Equal(200, getResp.StatusCode);

        //Test lifecycle expiration
        Assert.Equal(DateTime.UtcNow.AddDays(2).Date, getResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
        Assert.Equal("ExpireAll", getResp.LifeCycleRuleId);
    }
}