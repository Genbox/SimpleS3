using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketAnalyticsTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketAnalyticsConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        const string id = "analytics";
        Assert.Equal(204, (await client.PutBucketAnalyticsConfigurationAsync(bucket, id, new S3AnalyticsConfiguration(id), token: token)).StatusCode);
        GetBucketAnalyticsConfigurationResponse read = await client.GetBucketAnalyticsConfigurationAsync(bucket, id, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(id, read.Configuration.Id);
        Assert.Null(read.Configuration.Filter);
        Assert.Null(read.Configuration.DataExport);
        ListBucketAnalyticsConfigurationsResponse list = await client.ListBucketAnalyticsConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, list.StatusCode);
        Assert.Equal(id, Assert.Single(list.Configurations).Id);
        Assert.False(list.IsTruncated);
        Assert.Equal(204, (await client.DeleteBucketAnalyticsConfigurationAsync(bucket, id, token: token)).StatusCode);
        Assert.Equal(404, (await client.GetBucketAnalyticsConfigurationAsync(bucket, id, token: token)).StatusCode);
        ListBucketAnalyticsConfigurationsResponse empty = await client.ListBucketAnalyticsConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, empty.StatusCode);
        Assert.Empty(empty.Configurations);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PrefixFilter(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3AnalyticsConfiguration configuration = new S3AnalyticsConfiguration("filtered") { Filter = new S3Filter { Prefix = "logs/" } };
        Assert.Equal(204, (await client.PutBucketAnalyticsConfigurationAsync(bucket, configuration.Id, configuration, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketAnalyticsConfigurationResponse read = await client.GetBucketAnalyticsConfigurationAsync(bucket, configuration.Id, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal("logs/", read.Configuration.Filter?.Prefix);
    });
}