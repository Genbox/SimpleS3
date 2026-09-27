using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

/// <summary>Provides the <c>IBucketOperations</c> API.</summary>
[PublicAPI]
public interface IBucketOperations
{
    /// <summary>Creates a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CreateBucket.html for details</summary>
    Task<CreateBucketResponse> CreateBucketAsync(CreateBucketRequest request, CancellationToken token = default);

    /// <summary>Delete a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteBucket.html for details</summary>
    Task<DeleteBucketResponse> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken token = default);

    /// <summary>List all buckets owned by the authenticated sender of the request See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListBuckets.html for details</summary>
    Task<ListBucketsResponse> ListBucketsAsync(ListBucketsRequest request, CancellationToken token = default);

    /// <summary>Check to see if a bucket exists. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_HeadBucket.html for details</summary>
    Task<HeadBucketResponse> HeadBucketAsync(HeadBucketRequest request, CancellationToken token = default);

    /// <summary>
    /// Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in
    /// the specified bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObjectLockConfiguration.html for details
    /// </summary>
    Task<PutObjectLockConfigurationResponse> PutObjectLockConfigurationAsync(PutObjectLockConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in the
    /// specified bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObjectLockConfiguration.html for details
    /// </summary>
    Task<GetObjectLockConfigurationResponse> GetObjectLockConfigurationAsync(GetObjectLockConfigurationRequest request, CancellationToken token = default);

    /// <summary>Gets the bucket logging configuration.</summary>
    Task<GetBucketLoggingResponse> GetBucketLoggingAsync(GetBucketLoggingRequest request, CancellationToken token = default);

    /// <summary>Sets the bucket logging configuration.</summary>
    Task<PutBucketLoggingResponse> PutBucketLoggingAsync(PutBucketLoggingRequest request, CancellationToken token = default);

    /// <summary>Returns the tag set associated with the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketTagging.html for details</summary>
    Task<GetBucketTaggingResponse> GetBucketTaggingAsync(GetBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketAclAsync</c> API.</summary>
    Task<GetBucketAclResponse> GetBucketAclAsync(GetBucketAclRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketAclAsync</c> API.</summary>
    Task<PutBucketAclResponse> PutBucketAclAsync(PutBucketAclRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketCorsAsync</c> API.</summary>
    Task<GetBucketCorsResponse> GetBucketCorsAsync(GetBucketCorsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketCorsAsync</c> API.</summary>
    Task<PutBucketCorsResponse> PutBucketCorsAsync(PutBucketCorsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketCorsAsync</c> API.</summary>
    Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(DeleteBucketCorsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketOwnershipControlsAsync</c> API.</summary>
    Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(GetBucketOwnershipControlsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketOwnershipControlsAsync</c> API.</summary>
    Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(PutBucketOwnershipControlsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketOwnershipControlsAsync</c> API.</summary>
    Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(DeleteBucketOwnershipControlsRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketLocationAsync</c> API.</summary>
    Task<GetBucketLocationResponse> GetBucketLocationAsync(GetBucketLocationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketRequestPaymentAsync</c> API.</summary>
    Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(GetBucketRequestPaymentRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketRequestPaymentAsync</c> API.</summary>
    Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(PutBucketRequestPaymentRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketPolicyStatusAsync</c> API.</summary>
    Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(GetBucketPolicyStatusRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketWebsiteAsync</c> API.</summary>
    Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(GetBucketWebsiteRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketWebsiteAsync</c> API.</summary>
    Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(PutBucketWebsiteRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketWebsiteAsync</c> API.</summary>
    Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(DeleteBucketWebsiteRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationAsync</c> API.</summary>
    Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(DeleteBucketAnalyticsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketInventoryConfigurationAsync</c> API.</summary>
    Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketMetricsConfigurationAsync</c> API.</summary>
    Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(DeleteBucketMetricsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketIntelligentTieringConfigurationAsync</c> API.</summary>
    Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(DeleteBucketIntelligentTieringConfigurationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketReplicationAsync</c> API.</summary>
    Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(DeleteBucketReplicationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>UpdateBucketMetadataAnnotationTableConfigurationAsync</c> API.</summary>
    Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(UpdateBucketMetadataAnnotationTableConfigurationRequest request, CancellationToken token = default);

    /// <summary>Creates an S3 Metadata V2 configuration.</summary>
    Task<CreateBucketMetadataConfigurationResponse> CreateBucketMetadataConfigurationAsync(CreateBucketMetadataConfigurationRequest request, CancellationToken token = default);

    /// <summary>Deletes an S3 Metadata V2 configuration.</summary>
    Task<DeleteBucketMetadataConfigurationResponse> DeleteBucketMetadataConfigurationAsync(DeleteBucketMetadataConfigurationRequest request, CancellationToken token = default);

    /// <summary>Gets an S3 Metadata V2 configuration.</summary>
    Task<GetBucketMetadataConfigurationResponse> GetBucketMetadataConfigurationAsync(GetBucketMetadataConfigurationRequest request, CancellationToken token = default);

    /// <summary>Updates the S3 Metadata inventory table configuration.</summary>
    Task<UpdateBucketMetadataInventoryTableConfigurationResponse> UpdateBucketMetadataInventoryTableConfigurationAsync(UpdateBucketMetadataInventoryTableConfigurationRequest request, CancellationToken token = default);

    /// <summary>Updates the S3 Metadata journal table configuration.</summary>
    Task<UpdateBucketMetadataJournalTableConfigurationResponse> UpdateBucketMetadataJournalTableConfigurationAsync(UpdateBucketMetadataJournalTableConfigurationRequest request, CancellationToken token = default);

    /// <summary>Sets the tags for a bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketTagging.html for details.</summary>
    Task<PutBucketTaggingResponse> PutBucketTaggingAsync(PutBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>Deletes the tags from the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_DeleteBucketTagging.html for details</summary>
    Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>
    /// Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to perform faster data transfers to
    /// Amazon S3.
    /// </summary>
    Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Returns the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to
    /// perform faster data transfers to and from Amazon S3.
    /// </summary>
    Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketLifecycleConfiguration.html for details
    /// </summary>
    Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(PutBucketLifecycleConfigurationRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketLifecycleAsync</c> API.</summary>
    Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(DeleteBucketLifecycleRequest request, CancellationToken token = default);

    /// <summary>
    /// Configures default encryption and blocked encryption types for an existing bucket. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketEncryption.html for details.
    /// </summary>
    Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(PutBucketEncryptionRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketEncryptionAsync</c> API.</summary>
    Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(GetBucketEncryptionRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketEncryptionAsync</c> API.</summary>
    Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(DeleteBucketEncryptionRequest request, CancellationToken token = default);

    /// <summary>
    /// Sets the versioning state of an existing bucket. To set the versioning state, you must be the bucket owner. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketVersioning.html for details.
    /// </summary>
    Task<PutBucketVersioningResponse> PutBucketVersioningAsync(PutBucketVersioningRequest request, CancellationToken token = default);

    /// <summary>Returns the versioning state of a bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketVersioning.html for details.</summary>
    Task<GetBucketVersioningResponse> GetBucketVersioningAsync(GetBucketVersioningRequest request, CancellationToken token = default);

    /// <summary>
    /// Returns the lifecycle configuration information set on the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketLifecycleConfiguration.html for
    /// details.
    /// </summary>
    Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(GetBucketLifecycleConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Creates or modifies the PublicAccessBlock configuration for an Amazon S3 bucket. To use this operation, you must have the s3:PutBucketPublicAccessBlock permission.
    /// </summary>
    Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(PutPublicAccessBlockRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetPublicAccessBlockAsync</c> API.</summary>
    Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(GetPublicAccessBlockRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeletePublicAccessBlockAsync</c> API.</summary>
    Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(DeletePublicAccessBlockRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>GetBucketPolicyAsync</c> API.</summary>
    Task<GetBucketPolicyResponse> GetBucketPolicyAsync(GetBucketPolicyRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteBucketPolicyAsync</c> API.</summary>
    Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(DeleteBucketPolicyRequest request, CancellationToken token = default);

    /// <summary>Provides the <c>PutBucketPolicyAsync</c> API.</summary>
    Task<PutBucketPolicyResponse> PutBucketPolicyAsync(PutBucketPolicyRequest request, CancellationToken token = default);

    /// <summary>Gets a bucket's ABAC status.</summary>
    Task<GetBucketAbacResponse> GetBucketAbacAsync(GetBucketAbacRequest request, CancellationToken token = default);

    /// <summary>Sets a bucket's ABAC status.</summary>
    Task<PutBucketAbacResponse> PutBucketAbacAsync(PutBucketAbacRequest request, CancellationToken token = default);

    /// <summary>Gets an analytics configuration.</summary>
    Task<GetBucketAnalyticsConfigurationResponse> GetBucketAnalyticsConfigurationAsync(GetBucketAnalyticsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Creates or replaces an analytics configuration.</summary>
    Task<PutBucketAnalyticsConfigurationResponse> PutBucketAnalyticsConfigurationAsync(PutBucketAnalyticsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Lists analytics configurations.</summary>
    Task<ListBucketAnalyticsConfigurationsResponse> ListBucketAnalyticsConfigurationsAsync(ListBucketAnalyticsConfigurationsRequest request, CancellationToken token = default);

    /// <summary>Gets an inventory configuration.</summary>
    Task<GetBucketInventoryConfigurationResponse> GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, CancellationToken token = default);

    /// <summary>Creates or replaces an inventory configuration.</summary>
    Task<PutBucketInventoryConfigurationResponse> PutBucketInventoryConfigurationAsync(PutBucketInventoryConfigurationRequest request, CancellationToken token = default);

    /// <summary>Lists inventory configurations.</summary>
    Task<ListBucketInventoryConfigurationsResponse> ListBucketInventoryConfigurationsAsync(ListBucketInventoryConfigurationsRequest request, CancellationToken token = default);

    /// <summary>Gets a metrics configuration.</summary>
    Task<GetBucketMetricsConfigurationResponse> GetBucketMetricsConfigurationAsync(GetBucketMetricsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Creates or replaces a metrics configuration.</summary>
    Task<PutBucketMetricsConfigurationResponse> PutBucketMetricsConfigurationAsync(PutBucketMetricsConfigurationRequest request, CancellationToken token = default);

    /// <summary>Lists metrics configurations.</summary>
    Task<ListBucketMetricsConfigurationsResponse> ListBucketMetricsConfigurationsAsync(ListBucketMetricsConfigurationsRequest request, CancellationToken token = default);

    /// <summary>Gets an Intelligent-Tiering configuration.</summary>
    Task<GetBucketIntelligentTieringConfigurationResponse> GetBucketIntelligentTieringConfigurationAsync(GetBucketIntelligentTieringConfigurationRequest request, CancellationToken token = default);

    /// <summary>Creates or replaces an Intelligent-Tiering configuration.</summary>
    Task<PutBucketIntelligentTieringConfigurationResponse> PutBucketIntelligentTieringConfigurationAsync(PutBucketIntelligentTieringConfigurationRequest request, CancellationToken token = default);

    /// <summary>Lists Intelligent-Tiering configurations.</summary>
    Task<ListBucketIntelligentTieringConfigurationsResponse> ListBucketIntelligentTieringConfigurationsAsync(ListBucketIntelligentTieringConfigurationsRequest request, CancellationToken token = default);
}