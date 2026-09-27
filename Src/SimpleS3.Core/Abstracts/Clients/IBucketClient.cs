using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Abstracts.Clients;

/// <summary>Provides the <c>IBucketClient</c> API.</summary>
public interface IBucketClient
{
    /// <summary>Creates a bucket</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default);

    /// <summary>Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can be deleted.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default);

    /// <summary>List all buckets you own</summary>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default);

    /// <summary>Use this to determine if a bucket exists and you have permission to access it</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default);

    /// <summary>
    /// Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in the
    /// specified bucket.
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketAclAsync</c> API.</summary>
    Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketAclAsync</c> API.</summary>
    Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketLocationAsync</c> API.</summary>
    Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default);

    /// <summary>Get the versioning configuration on an existing bucket.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="rules">A list of rules you wish to use</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketLifecycleAsync</c> API.</summary>
    Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default);

    /// <summary>Returns the lifecycle configuration information set on the bucket.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>
    /// Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in
    /// the specified bucket.
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="enabled">A boolean indicating if locking is enabled or not</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets the logging configuration for a bucket.</summary>
    Task<GetBucketLoggingResponse> GetBucketLoggingAsync(string bucketName, Action<GetBucketLoggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Sets or disables logging for a bucket.</summary>
    Task<PutBucketLoggingResponse> PutBucketLoggingAsync(string bucketName, S3LoggingConfiguration? loggingConfiguration, Action<PutBucketLoggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Get the tags associated with the bucket.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketCorsAsync</c> API.</summary>
    Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketCorsAsync</c> API.</summary>
    Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketCorsAsync</c> API.</summary>
    Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketOwnershipControlsAsync</c> API.</summary>
    Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketOwnershipControlsAsync</c> API.</summary>
    Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketOwnershipControlsAsync</c> API.</summary>
    Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketRequestPaymentAsync</c> API.</summary>
    Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketRequestPaymentAsync</c> API.</summary>
    Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketPolicyStatusAsync</c> API.</summary>
    Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketWebsiteAsync</c> API.</summary>
    Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketWebsiteAsync</c> API.</summary>
    Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketWebsiteAsync</c> API.</summary>
    Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationAsync</c> API.</summary>
    Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketInventoryConfigurationAsync</c> API.</summary>
    Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketMetricsConfigurationAsync</c> API.</summary>
    Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketIntelligentTieringConfigurationAsync</c> API.</summary>
    Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketReplicationAsync</c> API.</summary>
    Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>UpdateBucketMetadataAnnotationTableConfigurationAsync</c> API.</summary>
    Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates an S3 Metadata V2 configuration.</summary>
    Task<CreateBucketMetadataConfigurationResponse> CreateBucketMetadataConfigurationAsync(string bucketName, S3MetadataConfiguration configuration, Action<CreateBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Deletes an S3 Metadata V2 configuration.</summary>
    Task<DeleteBucketMetadataConfigurationResponse> DeleteBucketMetadataConfigurationAsync(string bucketName, Action<DeleteBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets an S3 Metadata V2 configuration.</summary>
    Task<GetBucketMetadataConfigurationResponse> GetBucketMetadataConfigurationAsync(string bucketName, Action<GetBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Updates the S3 Metadata live inventory table.</summary>
    Task<UpdateBucketMetadataInventoryTableConfigurationResponse> UpdateBucketMetadataInventoryTableConfigurationAsync(string bucketName, S3InventoryTableConfiguration configuration, Action<UpdateBucketMetadataInventoryTableConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Updates S3 Metadata journal record expiration.</summary>
    Task<UpdateBucketMetadataJournalTableConfigurationResponse> UpdateBucketMetadataJournalTableConfigurationAsync(string bucketName, S3JournalTableConfiguration configuration, Action<UpdateBucketMetadataJournalTableConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Set tags associated with the bucket. This can be used to track bucket usage on your invoices.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="tags">The tags</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Delete tags associated with a bucket</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default);

    /// <summary>
    /// Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to perform faster data transfers to
    /// Amazon S3.
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="enabled">Set to true to enable acceleration. Set to false to disable it.</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>
    /// Returns the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to
    /// perform faster data transfers to and from Amazon S3.
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Configures default encryption and blocked encryption types for an existing bucket.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="rules">A list of encryption rules you wish to use</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketEncryptionAsync</c> API.</summary>
    Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketEncryptionAsync</c> API.</summary>
    Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default);

    /// <summary>Enable or disable versioning on an existing bucket.</summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="enabled">Set to true if you want to enable versioning. Set it to false to disable versioning.</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token </param>
    Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutPublicAccessBlockAsync</c> API.</summary>
    Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetPublicAccessBlockAsync</c> API.</summary>
    Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeletePublicAccessBlockAsync</c> API.</summary>
    Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketPolicyAsync</c> API.</summary>
    Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketPolicyAsync</c> API.</summary>
    Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketPolicyAsync</c> API.</summary>
    Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets a bucket's ABAC status.</summary>
    Task<GetBucketAbacResponse> GetBucketAbacAsync(string bucketName, Action<GetBucketAbacRequest>? config = null, CancellationToken token = default);

    /// <summary>Sets a bucket's ABAC status.</summary>
    Task<PutBucketAbacResponse> PutBucketAbacAsync(string bucketName, bool enabled, Action<PutBucketAbacRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets an analytics configuration.</summary>
    Task<GetBucketAnalyticsConfigurationResponse> GetBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates or replaces an analytics configuration.</summary>
    Task<PutBucketAnalyticsConfigurationResponse> PutBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, S3AnalyticsConfiguration configuration, Action<PutBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Lists analytics configurations.</summary>
    Task<ListBucketAnalyticsConfigurationsResponse> ListBucketAnalyticsConfigurationsAsync(string bucketName, Action<ListBucketAnalyticsConfigurationsRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets an inventory configuration.</summary>
    Task<GetBucketInventoryConfigurationResponse> GetBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<GetBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates or replaces an inventory configuration.</summary>
    Task<PutBucketInventoryConfigurationResponse> PutBucketInventoryConfigurationAsync(string bucketName, string configurationId, S3InventoryConfiguration configuration, Action<PutBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Lists inventory configurations.</summary>
    Task<ListBucketInventoryConfigurationsResponse> ListBucketInventoryConfigurationsAsync(string bucketName, Action<ListBucketInventoryConfigurationsRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets a metrics configuration.</summary>
    Task<GetBucketMetricsConfigurationResponse> GetBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates or replaces a metrics configuration.</summary>
    Task<PutBucketMetricsConfigurationResponse> PutBucketMetricsConfigurationAsync(string bucketName, string configurationId, S3MetricsConfiguration configuration, Action<PutBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Lists metrics configurations.</summary>
    Task<ListBucketMetricsConfigurationsResponse> ListBucketMetricsConfigurationsAsync(string bucketName, Action<ListBucketMetricsConfigurationsRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets an Intelligent-Tiering configuration.</summary>
    Task<GetBucketIntelligentTieringConfigurationResponse> GetBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<GetBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Creates or replaces an Intelligent-Tiering configuration.</summary>
    Task<PutBucketIntelligentTieringConfigurationResponse> PutBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration, Action<PutBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default);

    /// <summary>Lists Intelligent-Tiering configurations.</summary>
    Task<ListBucketIntelligentTieringConfigurationsResponse> ListBucketIntelligentTieringConfigurationsAsync(string bucketName, Action<ListBucketIntelligentTieringConfigurationsRequest>? config = null, CancellationToken token = default);
}