using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketListingTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, "prefix", "region")]
    public async Task BucketListingFilter(S3Provider provider, string _, ISimpleClient client, string filter) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        string region = ProviderSetup.Instance.GetProfile(provider).RegionCode;
        ListBucketsResponse response = await client.ListBucketsAsync(request =>
        {
            if (filter == "prefix") request.Prefix = bucket;
            if (filter == "region") request.BucketRegion = region;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        S3Bucket result = Assert.Single(response.Buckets, item => item.BucketName == bucket);
        Assert.Equal(region, result.BucketRegion);

        if (filter == "prefix")
        {
            Assert.Single(response.Buckets);
            Assert.Equal(bucket, response.Prefix);
        }
        else
            Assert.All(response.Buckets, item => Assert.Equal(region, item.BucketRegion));

        Assert.Null(response.ContinuationToken);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task BucketListingPaginationPreservesAccountInventory(S3Provider provider, string bucket, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async temporaryBucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
        HashSet<string> tokens = new HashSet<string>(StringComparer.Ordinal);
        string? continuation = null;
        int pages = 0;

        do
        {
            Assert.True(++pages <= 1000, "Bucket pagination did not terminate.");
            ListBucketsResponse response = await client.ListBucketsAsync(request =>
            {
                request.MaxBuckets = 1;
                request.ContinuationToken = continuation;
            }, token);
            Assert.Equal(200, response.StatusCode);
            Assert.InRange(response.Buckets.Count, 0, 1);
            foreach (S3Bucket item in response.Buckets)
                Assert.True(seen.Add(item.BucketName), "Bucket pagination repeated an entry.");
            continuation = response.ContinuationToken;
            if (response.Buckets.Count == 0)
                Assert.True(string.IsNullOrEmpty(continuation), "Empty bucket page unexpectedly requires further traversal.");
            if (!string.IsNullOrEmpty(continuation))
                Assert.True(tokens.Add(continuation), "Bucket pagination repeated a continuation token.");
        } while (!string.IsNullOrEmpty(continuation));

        Assert.True(pages >= 2);
        Assert.Contains(bucket, seen);
        Assert.Contains(temporaryBucket, seen);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task BucketListingUnmatchedPrefixBehavior(S3Provider provider, string bucket, ISimpleClient client)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string prefix = "audit-no-match-" + Guid.NewGuid().ToString("N");
        ListBucketsResponse filtered = await client.ListBucketsAsync(request => request.Prefix = prefix, token);
        ListBucketsResponse control = await client.ListBucketsAsync(token: token);
        Assert.Equal(200, control.StatusCode);
        Assert.Contains(control.Buckets, item => item.BucketName == bucket);

        if (provider == S3Provider.GoogleCloudStorage)
            Assert.Equal(400, filtered.StatusCode);
        else if (provider == S3Provider.BackBlazeB2)
        {
            Assert.Equal(200, filtered.StatusCode);
            Assert.True(filtered.Buckets.Select(item => item.BucketName).ToHashSet(StringComparer.Ordinal)
                                .SetEquals(control.Buckets.Select(item => item.BucketName)), "Backblaze unmatched-prefix response differed from its unfiltered listing.");
        }
        else
        {
            Assert.True(filtered.IsSuccess);
            Assert.Empty(filtered.Buckets);
            ListBucketsResponse matching = await client.ListBucketsAsync(request => request.Prefix = bucket, token);
            Assert.Equal(200, matching.StatusCode);
            Assert.Contains(matching.Buckets, item => item.BucketName == bucket);
            Assert.All(matching.Buckets, item => Assert.StartsWith(bucket, item.BucketName, StringComparison.Ordinal));
        }
    }

    [Theory]
    [MultipleProviders(S3Provider.BackBlazeB2 | S3Provider.GoogleCloudStorage)]
    public async Task UnsupportedBucketListingPageSize(S3Provider provider, string bucket, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async temporaryBucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        ListBucketsResponse limited = await client.ListBucketsAsync(request => request.MaxBuckets = 1, token);
        ListBucketsResponse control = await client.ListBucketsAsync(token: token);
        Assert.Equal(200, control.StatusCode);
        Assert.Contains(control.Buckets, item => item.BucketName == bucket);
        Assert.Contains(control.Buckets, item => item.BucketName == temporaryBucket);

        if (provider == S3Provider.GoogleCloudStorage)
        {
            Assert.Equal(400, limited.StatusCode);
            return;
        }

        Assert.Equal(200, limited.StatusCode);
        Assert.True(limited.Buckets.Count >= 2);
        Assert.True(limited.Buckets.Select(item => item.BucketName).ToHashSet(StringComparer.Ordinal)
                           .SetEquals(control.Buckets.Select(item => item.BucketName)), "Backblaze MaxBuckets response differed from its unbounded listing.");
        Assert.Null(limited.ContinuationToken);
    });
}