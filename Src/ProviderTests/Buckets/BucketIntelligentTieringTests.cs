using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketIntelligentTieringTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketIntelligentTieringConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        S3IntelligentTieringConfiguration configuration = new S3IntelligentTieringConfiguration("tiering", true);
        configuration.Tierings.Add(new S3IntelligentTieringTransition(90, "ARCHIVE_ACCESS"));
        Assert.Equal(204, (await client.PutBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, configuration, token: token)).StatusCode);
        GetBucketIntelligentTieringConfigurationResponse read = await client.GetBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(configuration.Id, read.Configuration.Id);
        Assert.True(read.Configuration.Enabled);
        S3IntelligentTieringTransition tier = Assert.Single(read.Configuration.Tierings);
        Assert.Equal(90, tier.Days);
        Assert.Equal("ARCHIVE_ACCESS", tier.AccessTier);
        ListBucketIntelligentTieringConfigurationsResponse list = await client.ListBucketIntelligentTieringConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, list.StatusCode);
        Assert.Equal(configuration.Id, Assert.Single(list.Configurations).Id);
        Assert.False(list.IsTruncated);
        Assert.Equal(204, (await client.DeleteBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, token: token)).StatusCode);
        Assert.Equal(404, (await client.GetBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, token: token)).StatusCode);
        ListBucketIntelligentTieringConfigurationsResponse empty = await client.ListBucketIntelligentTieringConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, empty.StatusCode);
        Assert.Empty(empty.Configurations);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PrefixFilter(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3IntelligentTieringConfiguration configuration = new S3IntelligentTieringConfiguration("filtered", true) { Filter = new S3Filter { Prefix = "archive/" } };
        configuration.Tierings.Add(new S3IntelligentTieringTransition(90, "ARCHIVE_ACCESS"));
        Assert.Equal(204, (await client.PutBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, configuration, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketIntelligentTieringConfigurationResponse read = await client.GetBucketIntelligentTieringConfigurationAsync(bucket, configuration.Id, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal("archive/", read.Configuration.Filter?.Prefix);
    });
}