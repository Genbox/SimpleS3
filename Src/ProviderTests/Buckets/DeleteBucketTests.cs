using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class DeleteBucketTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task DeleteBucketRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucketName =>
    {
        DeleteBucketResponse deleted = await client.DeleteBucketAsync(tempBucketName, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, deleted.StatusCode);

        await ReadExpectedAsync(token => ReadBucketNamesAsync(client, token), names => !names.Contains(tempBucketName),
            $"'{tempBucketName}' to disappear from the complete bucket inventory", TestContext.Current.CancellationToken);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task MissingBucket(S3Provider _1, string _2, ISimpleClient client)
    {
        DeleteBucketResponse response = await client.DeleteBucketAsync(GetTemporaryBucket(), token: TestContext.Current.CancellationToken);
        Assert.Equal(404, response.StatusCode);
        Assert.Equal(ErrorCode.NoSuchBucket, response.Error?.Code);
    }

    private static async Task<HashSet<string>> ReadBucketNamesAsync(ISimpleClient client, CancellationToken token)
    {
        HashSet<string> names = new HashSet<string>(StringComparer.Ordinal);
        HashSet<string> tokens = new HashSet<string>(StringComparer.Ordinal);
        string? continuation = null;

        do
        {
            ListBucketsResponse page = await client.ListBucketsAsync(request => request.ContinuationToken = continuation, token);
            Assert.Equal(200, page.StatusCode);
            foreach (S3Bucket bucket in page.Buckets)
                names.Add(bucket.BucketName);

            continuation = page.ContinuationToken;
            if (!string.IsNullOrEmpty(continuation))
                Assert.True(tokens.Add(continuation), "Bucket inventory repeated a continuation token.");
        } while (!string.IsNullOrEmpty(continuation));

        return names;
    }
}