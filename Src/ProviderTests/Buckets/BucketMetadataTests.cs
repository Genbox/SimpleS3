using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketMetadataTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task CreateBucketMetadataConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CheckMetadataConfigurationAsync(provider, client, "round-trip");

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, "inventory", "journal-expiration")]
    public async Task MetadataConfigurationUpdate(S3Provider provider, string _, ISimpleClient client, string feature) => await CheckMetadataConfigurationAsync(provider, client, feature);

    private static async Task CheckMetadataConfigurationAsync(S3Provider provider, ISimpleClient client, string feature) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        S3MetadataConfiguration configuration = new S3MetadataConfiguration(new S3JournalTableConfiguration(new S3RecordExpiration(MetadataTableConfigurationState.Disabled)))
        {
            InventoryTableConfiguration = new S3InventoryTableConfiguration(MetadataTableConfigurationState.Disabled),
            AnnotationTableConfiguration = new S3AnnotationTableConfiguration(AnnotationTableConfigurationState.Disabled)
        };

        bool created = false;

        try
        {
            CreateBucketMetadataConfigurationResponse createResp = await client.CreateBucketMetadataConfigurationAsync(tempBucket, configuration, token: TestContext.Current.CancellationToken);
            Assert.True(createResp.IsSuccess, createResp.Error?.Message);
            created = true;

            GetBucketMetadataConfigurationResponse getResp = await client.GetBucketMetadataConfigurationAsync(tempBucket, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, getResp.StatusCode);
            Assert.NotNull(getResp.Configuration.Destination);
            Assert.Equal("DISABLED", getResp.Configuration.JournalTable?.Expiration);
            Assert.Null(getResp.Configuration.JournalTable?.ExpirationDays);
            Assert.Equal("DISABLED", getResp.Configuration.InventoryTable?.ConfigurationState);
            Assert.Equal("DISABLED", getResp.Configuration.AnnotationTable?.ConfigurationState);

            if (feature == "round-trip")
                return;

            // Configuration creation is asynchronous; updates require an ACTIVE journal table.
            await ReadExpectedAsync(t => client.GetBucketMetadataConfigurationAsync(tempBucket, token: t),
                response => response.IsSuccess && response.Configuration.JournalTable?.TableStatus == "ACTIVE",
                "an active metadata journal table", TestContext.Current.CancellationToken, 36, TimeSpan.FromSeconds(5));

            if (feature == "inventory")
            {
                UpdateBucketMetadataInventoryTableConfigurationResponse response = await client.UpdateBucketMetadataInventoryTableConfigurationAsync(tempBucket, new S3InventoryTableConfiguration(MetadataTableConfigurationState.Enabled), token: TestContext.Current.CancellationToken);
                Assert.True(response.IsSuccess, response.Error?.Message);
            }

            if (feature == "journal-expiration")
                Assert.True((await client.UpdateBucketMetadataJournalTableConfigurationAsync(tempBucket, new S3JournalTableConfiguration(new S3RecordExpiration(MetadataTableConfigurationState.Enabled) { Days = 30 }), token: TestContext.Current.CancellationToken)).IsSuccess);
            GetBucketMetadataConfigurationResponse updated = await ReadExpectedAsync(t => client.GetBucketMetadataConfigurationAsync(tempBucket, token: t),
                response => response.IsSuccess && feature switch
                {
                    "inventory" => response.Configuration.InventoryTable?.ConfigurationState == "ENABLED",
                    _ => response.Configuration.JournalTable?.Expiration == "ENABLED" && response.Configuration.JournalTable.ExpirationDays == 30
                }, $"the {feature} update to persist", TestContext.Current.CancellationToken, 36, TimeSpan.FromSeconds(5));

            if (feature == "journal-expiration")
            {
                Assert.Equal("ENABLED", updated.Configuration.JournalTable?.Expiration);
                Assert.Equal(30, updated.Configuration.JournalTable?.ExpirationDays);
            }

            if (feature == "inventory")
            {
                Assert.Equal("ENABLED", updated.Configuration.InventoryTable?.ConfigurationState);
                Assert.Null(updated.Configuration.InventoryTable?.Error);
            }
        }
        finally
        {
            if (created)
            {
                using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                DeleteBucketMetadataConfigurationResponse deleteResp = await client.DeleteBucketMetadataConfigurationAsync(tempBucket, token: cleanup.Token);
                Assert.True(deleteResp.IsSuccess, deleteResp.Error?.Message);
                GetBucketMetadataConfigurationResponse deleted = await client.GetBucketMetadataConfigurationAsync(tempBucket, token: cleanup.Token);
                Assert.Equal(404, deleted.StatusCode);
            }
        }
    });
}