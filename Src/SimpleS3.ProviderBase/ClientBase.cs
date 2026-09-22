using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.ProviderBase;

/// <summary>Provides the <c>ClientBase</c> API.</summary>
public abstract class ClientBase : IDisposable
{
    private ServiceProvider? _serviceProvider;

    /// <summary>Provides the <c>ClientBase</c> API.</summary>
    protected ClientBase(IInputValidator inputValidator, SimpleS3Config config, NetworkConfig? networkConfig = null)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton(inputValidator);
        services.AddSingleton(Options.Create(config));
        services.AddLogging();

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

        networkConfig ??= new NetworkConfig();

        if (networkConfig.Proxy != null)
            httpBuilder.UseProxy(networkConfig.Proxy);

        httpBuilder.UseRetryAndTimeout(x =>
        {
            x.Retries = networkConfig.Retries;
            x.RetryMode = networkConfig.RetryMode;
            x.Timeout = networkConfig.Timeout;
            x.MaxRandomDelay = networkConfig.MaxRandomDelay;
        });

        Client = Build(services);
    }

    /// <summary>Provides the <c>ClientBase</c> API.</summary>
    protected ClientBase(IInputValidator inputValidator, SimpleS3Config config, INetworkDriver networkDriver)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton(inputValidator);
        services.AddLogging();

        SimpleS3CoreServices.AddSimpleS3Core(services);

        services.AddSingleton(networkDriver);
        services.AddSingleton(Options.Create(config));

        Client = Build(services);
    }

    /// <summary>Provides the <c>ClientBase</c> API.</summary>
    protected ClientBase(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedClient signedObjectClient)
    {
        Client = new SimpleClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
    }

    /// <summary>Provides the <c>Client</c> API.</summary>
    protected SimpleClient Client { get; }

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Provides the <c>CreateBucketMetadataConfigurationAsync</c> API.</summary>
    public virtual Task<CreateBucketMetadataConfigurationResponse> CreateBucketMetadataConfigurationAsync(string bucketName, S3MetadataConfiguration configuration, Action<CreateBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => Client.CreateBucketMetadataConfigurationAsync(bucketName, configuration, config, token);

    /// <summary>Provides the <c>DeleteBucketMetadataConfigurationAsync</c> API.</summary>
    public virtual Task<DeleteBucketMetadataConfigurationResponse> DeleteBucketMetadataConfigurationAsync(string bucketName, Action<DeleteBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketMetadataConfigurationAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketMetadataConfigurationAsync</c> API.</summary>
    public virtual Task<GetBucketMetadataConfigurationResponse> GetBucketMetadataConfigurationAsync(string bucketName, Action<GetBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketMetadataConfigurationAsync(bucketName, config, token);

    /// <summary>Provides the <c>UpdateBucketMetadataInventoryTableConfigurationAsync</c> API.</summary>
    public virtual Task<UpdateBucketMetadataInventoryTableConfigurationResponse> UpdateBucketMetadataInventoryTableConfigurationAsync(string bucketName, S3InventoryTableConfiguration configuration, Action<UpdateBucketMetadataInventoryTableConfigurationRequest>? config = null, CancellationToken token = default) => Client.UpdateBucketMetadataInventoryTableConfigurationAsync(bucketName, configuration, config, token);

    /// <summary>Provides the <c>UpdateBucketMetadataJournalTableConfigurationAsync</c> API.</summary>
    public virtual Task<UpdateBucketMetadataJournalTableConfigurationResponse> UpdateBucketMetadataJournalTableConfigurationAsync(string bucketName, S3JournalTableConfiguration configuration, Action<UpdateBucketMetadataJournalTableConfigurationRequest>? config = null, CancellationToken token = default) => Client.UpdateBucketMetadataJournalTableConfigurationAsync(bucketName, configuration, config, token);

    /// <summary>Provides the <c>GetBucketAnalyticsConfigurationAsync</c> API.</summary>
    public virtual Task<GetBucketAnalyticsConfigurationResponse> GetBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketAnalyticsConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>PutBucketAnalyticsConfigurationAsync</c> API.</summary>
    public virtual Task<PutBucketAnalyticsConfigurationResponse> PutBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, S3AnalyticsConfiguration configuration, Action<PutBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketAnalyticsConfigurationAsync(bucketName, configurationId, configuration, config, token);

    /// <summary>Provides the <c>ListBucketAnalyticsConfigurationsAsync</c> API.</summary>
    public virtual Task<ListBucketAnalyticsConfigurationsResponse> ListBucketAnalyticsConfigurationsAsync(string bucketName, Action<ListBucketAnalyticsConfigurationsRequest>? config = null, CancellationToken token = default) => Client.ListBucketAnalyticsConfigurationsAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketInventoryConfigurationAsync</c> API.</summary>
    public virtual Task<GetBucketInventoryConfigurationResponse> GetBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<GetBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketInventoryConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>PutBucketInventoryConfigurationAsync</c> API.</summary>
    public virtual Task<PutBucketInventoryConfigurationResponse> PutBucketInventoryConfigurationAsync(string bucketName, string configurationId, S3InventoryConfiguration configuration, Action<PutBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketInventoryConfigurationAsync(bucketName, configurationId, configuration, config, token);

    /// <summary>Provides the <c>ListBucketInventoryConfigurationsAsync</c> API.</summary>
    public virtual Task<ListBucketInventoryConfigurationsResponse> ListBucketInventoryConfigurationsAsync(string bucketName, Action<ListBucketInventoryConfigurationsRequest>? config = null, CancellationToken token = default) => Client.ListBucketInventoryConfigurationsAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketMetricsConfigurationAsync</c> API.</summary>
    public virtual Task<GetBucketMetricsConfigurationResponse> GetBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketMetricsConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>PutBucketMetricsConfigurationAsync</c> API.</summary>
    public virtual Task<PutBucketMetricsConfigurationResponse> PutBucketMetricsConfigurationAsync(string bucketName, string configurationId, S3MetricsConfiguration configuration, Action<PutBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketMetricsConfigurationAsync(bucketName, configurationId, configuration, config, token);

    /// <summary>Provides the <c>ListBucketMetricsConfigurationsAsync</c> API.</summary>
    public virtual Task<ListBucketMetricsConfigurationsResponse> ListBucketMetricsConfigurationsAsync(string bucketName, Action<ListBucketMetricsConfigurationsRequest>? config = null, CancellationToken token = default) => Client.ListBucketMetricsConfigurationsAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketIntelligentTieringConfigurationAsync</c> API.</summary>
    public virtual Task<GetBucketIntelligentTieringConfigurationResponse> GetBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<GetBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketIntelligentTieringConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>PutBucketIntelligentTieringConfigurationAsync</c> API.</summary>
    public virtual Task<PutBucketIntelligentTieringConfigurationResponse> PutBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration, Action<PutBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketIntelligentTieringConfigurationAsync(bucketName, configurationId, configuration, config, token);

    /// <summary>Provides the <c>ListBucketIntelligentTieringConfigurationsAsync</c> API.</summary>
    public virtual Task<ListBucketIntelligentTieringConfigurationsResponse> ListBucketIntelligentTieringConfigurationsAsync(string bucketName, Action<ListBucketIntelligentTieringConfigurationsRequest>? config = null, CancellationToken token = default) => Client.ListBucketIntelligentTieringConfigurationsAsync(bucketName, config, token);

    private SimpleClient Build(IServiceCollection services)
    {
        Validator.RequireThat(_serviceProvider == null, "You should only call Build once.");

 #pragma warning disable IDISP003
        _serviceProvider = services.BuildServiceProvider();
 #pragma warning restore IDISP003

        IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
        IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
        IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
        IMultipartTransfer multipartTransfer = _serviceProvider.GetRequiredService<IMultipartTransfer>();
        ITransfer transfer = _serviceProvider.GetRequiredService<ITransfer>();
        ISignedClient signedObjectClient = _serviceProvider.GetRequiredService<ISignedClient>();

        return new SimpleClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
    }

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _serviceProvider?.Dispose();
    }
}