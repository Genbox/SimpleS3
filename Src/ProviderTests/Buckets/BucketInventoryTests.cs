using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketInventoryTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketInventoryConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        S3InventoryConfiguration configuration = CreateConfiguration(bucket);
        Assert.Equal(204, (await client.PutBucketInventoryConfigurationAsync(bucket, configuration.Id, configuration, token: token)).StatusCode);
        GetBucketInventoryConfigurationResponse read = await client.GetBucketInventoryConfigurationAsync(bucket, configuration.Id, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(configuration.Id, read.Configuration.Id);
        Assert.True(read.Configuration.Enabled);
        Assert.Equal("All", read.Configuration.IncludedObjectVersions);
        Assert.Equal("Daily", read.Configuration.ScheduleFrequency);
        Assert.Equal(configuration.Destination.BucketArn, read.Configuration.Destination.BucketArn);
        Assert.Equal("CSV", read.Configuration.Destination.Format);
        ListBucketInventoryConfigurationsResponse list = await client.ListBucketInventoryConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, list.StatusCode);
        Assert.Equal(configuration.Id, Assert.Single(list.Configurations).Id);
        Assert.False(list.IsTruncated);
        Assert.Equal(204, (await client.DeleteBucketInventoryConfigurationAsync(bucket, configuration.Id, token: token)).StatusCode);
        Assert.Equal(404, (await client.GetBucketInventoryConfigurationAsync(bucket, configuration.Id, token: token)).StatusCode);
        ListBucketInventoryConfigurationsResponse empty = await client.ListBucketInventoryConfigurationsAsync(bucket, token: token);
        Assert.Equal(200, empty.StatusCode);
        Assert.Empty(empty.Configurations);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task OptionalFields(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3InventoryConfiguration configuration = CreateConfiguration(bucket);
        configuration.OptionalFields.Add("Size");
        configuration.OptionalFields.Add("LastModifiedDate");
        Assert.Equal(204, (await client.PutBucketInventoryConfigurationAsync(bucket, configuration.Id, configuration, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketInventoryConfigurationResponse read = await client.GetBucketInventoryConfigurationAsync(bucket, configuration.Id, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(2, read.Configuration.OptionalFields.Count);
        Assert.Contains("Size", read.Configuration.OptionalFields, StringComparer.Ordinal);
        Assert.Contains("LastModifiedDate", read.Configuration.OptionalFields, StringComparer.Ordinal);
    });

    private static S3InventoryConfiguration CreateConfiguration(string bucket) => new S3InventoryConfiguration("inventory")
    {
        Enabled = true,
        Destination = new S3InventoryDestination { BucketArn = "arn:aws:s3:::" + bucket }
    };
}