using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectContentTests
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task ObjectKeyContainingEquals(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "equals-" + Guid.NewGuid().ToString("N") + "/a=a";
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "hello", token: token), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: token);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("hello", await response.Content.AsStringAsync(token: token));
        });
    }

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task StreamReturnsLength(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "stream-length-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "hello world", token: token), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: token);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(11, response.ContentLength);
            Assert.True(response.Content.HasLength);
            Assert.Equal(11, response.Content.Length);
            Assert.Equal("hello world", await response.Content.AsStringAsync(token: token));
        });
    }
}