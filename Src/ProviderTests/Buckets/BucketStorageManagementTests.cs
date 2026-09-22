using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketStorageManagementTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketMetricsConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        const string id = "provider-tests";
        S3MetricsConfiguration configuration = new S3MetricsConfiguration(id);

        PutBucketMetricsConfigurationResponse putResp = await client.PutBucketMetricsConfigurationAsync(tempBucket, id, configuration, token: TestContext.Current.CancellationToken);
        Assert.True(putResp.IsSuccess);

        GetBucketMetricsConfigurationResponse getResp = await client.GetBucketMetricsConfigurationAsync(tempBucket, id, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(id, getResp.Configuration.Id);
        Assert.Null(getResp.Configuration.Filter);

        ListBucketMetricsConfigurationsResponse listResp = await client.ListBucketMetricsConfigurationsAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, listResp.StatusCode);
        Assert.Contains(listResp.Configurations, item => item.Id == id);

        DeleteBucketMetricsConfigurationResponse deleteResp = await client.DeleteBucketMetricsConfigurationAsync(tempBucket, id, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, deleteResp.StatusCode);
        Assert.Equal(404, (await client.GetBucketMetricsConfigurationAsync(tempBucket, id, token: TestContext.Current.CancellationToken)).StatusCode);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task MetricsPrefixFilter(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string id = "filtered";
        S3MetricsConfiguration configuration = new S3MetricsConfiguration(id) { Filter = new S3Filter { Prefix = "metrics/" } };
        Assert.Equal(204, (await client.PutBucketMetricsConfigurationAsync(bucket, id, configuration, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketMetricsConfigurationResponse response = await client.GetBucketMetricsConfigurationAsync(bucket, id, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("metrics/", response.Configuration.Filter?.Prefix);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task MetricsConfigurationPagination(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        List<string> ids = Enumerable.Range(0, 101).Select(static value => $"pagination-{value:D3}").ToList();
        List<string> createdIds = [];

        try
        {
            foreach (string id in ids)
            {
                PutBucketMetricsConfigurationResponse put = await client.PutBucketMetricsConfigurationAsync(tempBucket, id, new S3MetricsConfiguration(id), token: token);
                Assert.Equal(204, put.StatusCode);
                createdIds.Add(id);
            }

            ListBucketMetricsConfigurationsResponse first = await client.ListBucketMetricsConfigurationsAsync(tempBucket, token: token);
            Assert.Equal(200, first.StatusCode);
            Assert.True(first.IsTruncated);
            Assert.Null(first.ContinuationToken);
            Assert.False(string.IsNullOrEmpty(first.NextContinuationToken));
            Assert.Equal(100, first.Configurations.Count);

            string nextToken = Assert.IsType<string>(first.NextContinuationToken);
            ListBucketMetricsConfigurationsResponse second = await client.ListBucketMetricsConfigurationsAsync(tempBucket, request => request.ContinuationToken = nextToken, token);
            Assert.Equal(200, second.StatusCode);
            Assert.False(second.IsTruncated);
            Assert.Equal(nextToken, second.ContinuationToken);
            Assert.Null(second.NextContinuationToken);
            Assert.Single(second.Configurations);

            string[] listed = first.Configurations.Concat(second.Configurations).Select(static configuration => configuration.Id).Order(StringComparer.Ordinal).ToArray();
            Assert.Equal(ids, listed);
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromMinutes(5));

            foreach (string id in createdIds)
            {
                DeleteBucketMetricsConfigurationResponse deleted = await client.DeleteBucketMetricsConfigurationAsync(tempBucket, id, token: cleanup.Token);
                Assert.Equal(204, deleted.StatusCode);
            }

            ListBucketMetricsConfigurationsResponse empty = await client.ListBucketMetricsConfigurationsAsync(tempBucket, token: cleanup.Token);
            Assert.Equal(200, empty.StatusCode);
            Assert.Empty(empty.Configurations);
            Assert.False(empty.IsTruncated);
            Assert.Null(empty.ContinuationToken);
            Assert.Null(empty.NextContinuationToken);
        }
    });
}