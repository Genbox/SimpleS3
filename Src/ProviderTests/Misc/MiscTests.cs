using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Misc;

public class MiscTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task IsEncodingCorrect(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "equals-" + Guid.NewGuid().ToString("N") + "/a=a";
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse uploaded = await client.PutObjectStringAsync(bucket, key, "hello", token: token);
        Assert.Equal(200, uploaded.StatusCode);

        try
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: token);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("hello", await response.Content.AsStringAsync(token: token));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            Assert.Equal(204, (await client.DeleteObjectAsync(bucket, key, r => r.VersionId = uploaded.VersionId, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
        }
    }

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task StreamReturnsLength(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "stream-length-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse uploaded = await client.PutObjectStringAsync(bucket, key, "hello world", token: token);
        Assert.Equal(200, uploaded.StatusCode);

        try
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: token);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(11, response.ContentLength);
            Assert.True(response.Content.HasLength);
            Assert.Equal(11, response.Content.Length);
            Assert.Equal("hello world", await response.Content.AsStringAsync(token: token));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            Assert.Equal(204, (await client.DeleteObjectAsync(bucket, key, r => r.VersionId = uploaded.VersionId, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
        }
    }
}