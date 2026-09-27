using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.ProviderBase;

#pragma warning disable CA1033 // Explicit implementations intentionally keep unsupported operations off the concrete provider API.
/// <summary>
/// Supplies throwing fallbacks for unsupported S3 operations without exposing them on a provider's public surface.
/// Provides shared transfer and signing operations through the provider's configured client.
/// </summary>
/// <remarks>
/// Derived providers must declare <see cref="ISimpleClient" /> to reimplement the inherited interface mappings.
/// Their public methods implement supported operations; all remaining operations use these throwing fallbacks.
/// Transfer and signing operations are inherited from this base.
/// </remarks>
public abstract class ProviderClientAdapter : ISimpleClient
{
    private readonly SimpleClient _client;

    /// <summary>Creates an adapter using the provider's configured client.</summary>
    protected ProviderClientAdapter(SimpleClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public IUpload CreateUpload(string bucket, string objectKey) => _client.CreateUpload(bucket, objectKey);

    /// <inheritdoc />
    public IDownload CreateDownload(string bucket, string objectKey) => _client.CreateDownload(bucket, objectKey);

    /// <inheritdoc />
    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => _client.SignRequest(request, expiresIn);

    /// <inheritdoc />
    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => _client.SendSignedRequestAsync<TResp>(url, httpMethod, content, token);

    Task<CreateBucketResponse> IBucketClient.CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketResponse> IBucketClient.DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListBucketsResponse> IBucketClient.ListBucketsAsync(Action<ListBucketsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<HeadBucketResponse> IBucketClient.HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketLockConfigurationResponse> IBucketClient.GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketAclResponse> IBucketClient.GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketAclResponse> IBucketClient.PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketLocationResponse> IBucketClient.GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketVersioningResponse> IBucketClient.GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<DeleteObjectResponse> IObjectClient.DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteObjectsResponse> IObjectClient.DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<HeadObjectResponse> IObjectClient.HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectResponse> IObjectClient.GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectResponse> IObjectClient.PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListObjectsResponse> IObjectClient.ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<CopyObjectResponse> IObjectClient.CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectAclResponse> IObjectClient.PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectAclResponse> IObjectClient.GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectRetentionResponse> IObjectClient.GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectRetentionResponse> IObjectClient.PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<CreateMultipartUploadResponse> IMultipartClient.CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<UploadPartResponse> IMultipartClient.UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListPartsResponse> IMultipartClient.ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<CompleteMultipartUploadResponse> IMultipartClient.CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<AbortMultipartUploadResponse> IMultipartClient.AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListMultipartUploadsResponse> IMultipartClient.ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<CompleteMultipartUploadResponse> IMultipartTransfer.MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize, int numParallelParts, Action<CreateMultipartUploadRequest>? config, Action<UploadPartResponse>? onPartResponse, CancellationToken token) => throw new NotSupportedException();
    Task<CompleteMultipartUploadResponse> IMultipartTransfer.MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize, int numParallelParts, Action<UploadPartResponse>? onPartResponse, CancellationToken token) => throw new NotSupportedException();

    Task<PutBucketLifecycleConfigurationResponse> IBucketClient.PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketLifecycleResponse> IBucketClient.DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketLifecycleConfigurationResponse> IBucketClient.GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketLockConfigurationResponse> IBucketClient.PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<GetBucketTaggingResponse> IBucketClient.GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketCorsResponse> IBucketClient.GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketCorsResponse> IBucketClient.PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketCorsResponse> IBucketClient.DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketOwnershipControlsResponse> IBucketClient.GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketOwnershipControlsResponse> IBucketClient.PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketOwnershipControlsResponse> IBucketClient.DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketRequestPaymentResponse> IBucketClient.GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketRequestPaymentResponse> IBucketClient.PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketPolicyStatusResponse> IBucketClient.GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketWebsiteResponse> IBucketClient.GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketWebsiteResponse> IBucketClient.PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketWebsiteResponse> IBucketClient.DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketAnalyticsConfigurationResponse> IBucketClient.DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketInventoryConfigurationResponse> IBucketClient.DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketMetricsConfigurationResponse> IBucketClient.DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketIntelligentTieringConfigurationResponse> IBucketClient.DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketReplicationResponse> IBucketClient.DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> IBucketClient.UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<CreateBucketMetadataConfigurationResponse> IBucketClient.CreateBucketMetadataConfigurationAsync(string bucketName, S3MetadataConfiguration configuration, Action<CreateBucketMetadataConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketMetadataConfigurationResponse> IBucketClient.DeleteBucketMetadataConfigurationAsync(string bucketName, Action<DeleteBucketMetadataConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketMetadataConfigurationResponse> IBucketClient.GetBucketMetadataConfigurationAsync(string bucketName, Action<GetBucketMetadataConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<UpdateBucketMetadataInventoryTableConfigurationResponse> IBucketClient.UpdateBucketMetadataInventoryTableConfigurationAsync(string bucketName, S3InventoryTableConfiguration configuration, Action<UpdateBucketMetadataInventoryTableConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<UpdateBucketMetadataJournalTableConfigurationResponse> IBucketClient.UpdateBucketMetadataJournalTableConfigurationAsync(string bucketName, S3JournalTableConfiguration configuration, Action<UpdateBucketMetadataJournalTableConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketTaggingResponse> IBucketClient.PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketTaggingResponse> IBucketClient.DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketAccelerateConfigurationResponse> IBucketClient.PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketAccelerateConfigurationResponse> IBucketClient.GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketEncryptionResponse> IBucketClient.PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketEncryptionResponse> IBucketClient.GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketEncryptionResponse> IBucketClient.DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketVersioningResponse> IBucketClient.PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutPublicAccessBlockResponse> IBucketClient.PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetPublicAccessBlockResponse> IBucketClient.GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeletePublicAccessBlockResponse> IBucketClient.DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketPolicyResponse> IBucketClient.GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteBucketPolicyResponse> IBucketClient.DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketPolicyResponse> IBucketClient.PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketAbacResponse> IBucketClient.GetBucketAbacAsync(string bucketName, Action<GetBucketAbacRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketAbacResponse> IBucketClient.PutBucketAbacAsync(string bucketName, bool enabled, Action<PutBucketAbacRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketAnalyticsConfigurationResponse> IBucketClient.GetBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketAnalyticsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketAnalyticsConfigurationResponse> IBucketClient.PutBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, S3AnalyticsConfiguration configuration, Action<PutBucketAnalyticsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListBucketAnalyticsConfigurationsResponse> IBucketClient.ListBucketAnalyticsConfigurationsAsync(string bucketName, Action<ListBucketAnalyticsConfigurationsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketInventoryConfigurationResponse> IBucketClient.GetBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<GetBucketInventoryConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketInventoryConfigurationResponse> IBucketClient.PutBucketInventoryConfigurationAsync(string bucketName, string configurationId, S3InventoryConfiguration configuration, Action<PutBucketInventoryConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListBucketInventoryConfigurationsResponse> IBucketClient.ListBucketInventoryConfigurationsAsync(string bucketName, Action<ListBucketInventoryConfigurationsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketMetricsConfigurationResponse> IBucketClient.GetBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketMetricsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketMetricsConfigurationResponse> IBucketClient.PutBucketMetricsConfigurationAsync(string bucketName, string configurationId, S3MetricsConfiguration configuration, Action<PutBucketMetricsConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListBucketMetricsConfigurationsResponse> IBucketClient.ListBucketMetricsConfigurationsAsync(string bucketName, Action<ListBucketMetricsConfigurationsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetBucketIntelligentTieringConfigurationResponse> IBucketClient.GetBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<GetBucketIntelligentTieringConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketIntelligentTieringConfigurationResponse> IBucketClient.PutBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration, Action<PutBucketIntelligentTieringConfigurationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListBucketIntelligentTieringConfigurationsResponse> IBucketClient.ListBucketIntelligentTieringConfigurationsAsync(string bucketName, Action<ListBucketIntelligentTieringConfigurationsRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<GetBucketLoggingResponse> IBucketClient.GetBucketLoggingAsync(string bucketName, Action<GetBucketLoggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutBucketLoggingResponse> IBucketClient.PutBucketLoggingAsync(string bucketName, S3LoggingConfiguration? loggingConfiguration, Action<PutBucketLoggingRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<ListObjectVersionsResponse> IObjectClient.ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<RestoreObjectResponse> IObjectClient.RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectAttributesResponse> IObjectClient.GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectAnnotationResponse> IObjectClient.GetObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<GetObjectAnnotationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectAnnotationResponse> IObjectClient.PutObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Stream content, Action<PutObjectAnnotationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteObjectAnnotationResponse> IObjectClient.DeleteObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<DeleteObjectAnnotationRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<ListObjectAnnotationsResponse> IObjectClient.ListObjectAnnotationsAsync(string bucketName, string objectKey, Action<ListObjectAnnotationsRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectLegalHoldResponse> IObjectClient.GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectLegalHoldResponse> IObjectClient.PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<GetObjectTaggingResponse> IObjectClient.GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<PutObjectTaggingResponse> IObjectClient.PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<DeleteObjectTaggingResponse> IObjectClient.DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config, CancellationToken token) => throw new NotSupportedException();
    Task<UpdateObjectEncryptionResponse> IObjectClient.UpdateObjectEncryptionAsync(string bucketName, string objectKey, string kmsKeyArn, Action<UpdateObjectEncryptionRequest>? config, CancellationToken token) => throw new NotSupportedException();

    Task<UploadPartCopyResponse> IMultipartClient.UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config, CancellationToken token) => throw new NotSupportedException();
    IAsyncEnumerable<GetObjectResponse> IMultipartTransfer.MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize, int numParallelParts, Action<GetObjectRequest>? config, CancellationToken token) => throw new NotSupportedException();
}
#pragma warning restore CA1033