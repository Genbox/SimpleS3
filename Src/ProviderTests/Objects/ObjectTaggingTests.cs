using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectTaggingTests
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task PutObjectTaggingRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "object-tags-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        Dictionary<string, string> tags = new Dictionary<string, string>(StringComparer.Ordinal) { ["purpose"] = "test" };
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: token), async _ =>
        {
            Assert.Equal(200, (await client.PutObjectTaggingAsync(bucket, key, tags, token: token)).StatusCode);
            GetObjectTaggingResponse read = await client.GetObjectTaggingAsync(bucket, key, token: token);
            Assert.Equal(200, read.StatusCode);
            Assert.Equal(tags, read.Tags);
            Assert.Equal(204, (await client.DeleteObjectTaggingAsync(bucket, key, token: token)).StatusCode);
            GetObjectTaggingResponse deleted = await client.GetObjectTaggingAsync(bucket, key, token: token);
            Assert.Equal(200, deleted.StatusCode);
            Assert.Empty(deleted.Tags);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task ReplacingTagsRemovesOldKeys(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "replace-tags-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        Dictionary<string, string> initial = new Dictionary<string, string>(StringComparer.Ordinal) { ["old"] = "value" };
        Dictionary<string, string> replacement = new Dictionary<string, string>(StringComparer.Ordinal) { ["new"] = "replacement" };
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: token), async _ =>
        {
            Assert.Equal(200, (await client.PutObjectTaggingAsync(bucket, key, initial, token: token)).StatusCode);
            Assert.Equal(200, (await client.PutObjectTaggingAsync(bucket, key, replacement, token: token)).StatusCode);
            GetObjectTaggingResponse read = await client.GetObjectTaggingAsync(bucket, key, token: token);
            Assert.Equal(200, read.StatusCode);
            Assert.Equal(replacement, read.Tags);
        });
    }
}