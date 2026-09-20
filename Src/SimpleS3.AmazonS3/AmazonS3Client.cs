using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.AmazonS3;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public sealed class AmazonS3Client : ClientBase, ISimpleClient
{
    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(string keyId, byte[] accessKey, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AmazonS3Config(new AccessKey(keyId, accessKey), region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(string keyId, string accessKey, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AmazonS3Config(new StringAccessKey(keyId, accessKey), region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="credentials">The credentials to use</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(IAccessKey? credentials, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AmazonS3Config(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(AmazonS3Config config, NetworkConfig? networkConfig = null) : base(new AmazonS3InputValidator(), config, networkConfig) {}

    /// <summary>Provides the <c>AmazonS3Client</c> API.</summary>
    public AmazonS3Client(AmazonS3Config config, INetworkDriver networkDriver) : base(new AmazonS3InputValidator(), config, networkDriver) {}

    internal AmazonS3Client(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedClient signedObjectClient) : base(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient) {}

    /// <summary>Provides the <c>CreateBucketAsync</c> API.</summary>
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => Client.CreateBucketAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeleteBucketAsync</c> API.</summary>
    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketAsync(bucketName, config, token);

    /// <summary>Provides the <c>ListBucketsAsync</c> API.</summary>
    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => Client.ListBucketsAsync(config, token);

    /// <summary>Provides the <c>HeadBucketAsync</c> API.</summary>
    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => Client.HeadBucketAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketLockConfigurationAsync</c> API.</summary>
    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketLockConfigurationAsync(bucketName, enabled, config, token);

    /// <summary>Provides the <c>GetBucketLockConfigurationAsync</c> API.</summary>
    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketLockConfigurationAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketTaggingAsync</c> API.</summary>
    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.GetBucketTaggingAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketAclAsync</c> API.</summary>
    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => Client.GetBucketAclAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketAclAsync</c> API.</summary>
    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => Client.PutBucketAclAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketCorsAsync</c> API.</summary>
    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => Client.GetBucketCorsAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketCorsAsync</c> API.</summary>
    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => Client.PutBucketCorsAsync(bucketName, rules, config, token);

    /// <summary>Provides the <c>DeleteBucketCorsAsync</c> API.</summary>
    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketCorsAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketOwnershipControlsAsync</c> API.</summary>
    public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => Client.GetBucketOwnershipControlsAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketOwnershipControlsAsync</c> API.</summary>
    public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => Client.PutBucketOwnershipControlsAsync(bucketName, rules, config, token);

    /// <summary>Provides the <c>DeleteBucketOwnershipControlsAsync</c> API.</summary>
    public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketOwnershipControlsAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketLocationAsync</c> API.</summary>
    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => Client.GetBucketLocationAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketRequestPaymentAsync</c> API.</summary>
    public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => Client.GetBucketRequestPaymentAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketRequestPaymentAsync</c> API.</summary>
    public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => Client.PutBucketRequestPaymentAsync(bucketName, payer, config, token);

    /// <summary>Provides the <c>GetBucketPolicyStatusAsync</c> API.</summary>
    public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default) => Client.GetBucketPolicyStatusAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketWebsiteAsync</c> API.</summary>
    public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default) => Client.GetBucketWebsiteAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketWebsiteAsync</c> API.</summary>
    public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default) => Client.PutBucketWebsiteAsync(bucketName, websiteConfiguration, config, token);

    /// <summary>Provides the <c>DeleteBucketWebsiteAsync</c> API.</summary>
    public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketWebsiteAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationAsync</c> API.</summary>
    public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketAnalyticsConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>DeleteBucketInventoryConfigurationAsync</c> API.</summary>
    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketInventoryConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>DeleteBucketMetricsConfigurationAsync</c> API.</summary>
    public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketMetricsConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>DeleteBucketIntelligentTieringConfigurationAsync</c> API.</summary>
    public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketIntelligentTieringConfigurationAsync(bucketName, configurationId, config, token);

    /// <summary>Provides the <c>DeleteBucketReplicationAsync</c> API.</summary>
    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketReplicationAsync(bucketName, config, token);

    /// <summary>Provides the <c>UpdateBucketMetadataAnnotationTableConfigurationAsync</c> API.</summary>
    public Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config = null, CancellationToken token = default) => Client.UpdateBucketMetadataAnnotationTableConfigurationAsync(bucketName, configuration, config, token);

    /// <summary>Provides the <c>PutBucketTaggingAsync</c> API.</summary>
    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.PutBucketTaggingAsync(bucketName, tags, config, token);

    /// <summary>Provides the <c>DeleteBucketTaggingAsync</c> API.</summary>
    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketTaggingAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketAccelerateConfigurationAsync</c> API.</summary>
    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketAccelerateConfigurationAsync(bucketName, enabled, config, token);

    /// <summary>Provides the <c>GetBucketAccelerateConfigurationAsync</c> API.</summary>
    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketAccelerateConfigurationAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketLifecycleConfigurationAsync</c> API.</summary>
    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);

    /// <summary>Provides the <c>DeleteBucketLifecycleAsync</c> API.</summary>
    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketLifecycleAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketEncryptionAsync</c> API.</summary>
    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default) => Client.PutBucketEncryptionAsync(bucketName, rules, config, token);

    /// <summary>Provides the <c>GetBucketEncryptionAsync</c> API.</summary>
    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default) => Client.GetBucketEncryptionAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeleteBucketEncryptionAsync</c> API.</summary>
    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketEncryptionAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketVersioningAsync</c> API.</summary>
    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => Client.PutBucketVersioningAsync(bucketName, enabled, config, token);

    /// <summary>Provides the <c>GetBucketVersioningAsync</c> API.</summary>
    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => Client.GetBucketVersioningAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetBucketLifecycleConfigurationAsync</c> API.</summary>
    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketLifecycleConfigurationAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutPublicAccessBlockAsync</c> API.</summary>
    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default) => Client.PutPublicAccessBlockAsync(bucketName, config, token);

    /// <summary>Provides the <c>GetPublicAccessBlockAsync</c> API.</summary>
    public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default) => Client.GetPublicAccessBlockAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeletePublicAccessBlockAsync</c> API.</summary>
    public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default) => Client.DeletePublicAccessBlockAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeleteObjectAsync</c> API.</summary>
    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>DeleteObjectsAsync</c> API.</summary>
    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    /// <summary>Provides the <c>HeadObjectAsync</c> API.</summary>
    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => Client.HeadObjectAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>GetObjectAsync</c> API.</summary>
    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => Client.GetObjectAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>PutObjectAsync</c> API.</summary>
    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => Client.PutObjectAsync(bucketName, objectKey, data, config, token);

    /// <summary>Provides the <c>ListObjectsAsync</c> API.</summary>
    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => Client.ListObjectsAsync(bucketName, config, token);

    /// <summary>Provides the <c>RestoreObjectAsync</c> API.</summary>
    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default) => Client.RestoreObjectAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>CopyObjectAsync</c> API.</summary>
    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => Client.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);

    /// <summary>Provides the <c>PutObjectAclAsync</c> API.</summary>
    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => Client.PutObjectAclAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>GetObjectAclAsync</c> API.</summary>
    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => Client.GetObjectAclAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>GetObjectAttributesAsync</c> API.</summary>
    public Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default) => Client.GetObjectAttributesAsync(bucketName, objectKey, objectAttributes, config, token);

    /// <summary>Provides the <c>GetObjectAnnotationAsync</c> API.</summary>
    public Task<GetObjectAnnotationResponse> GetObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<GetObjectAnnotationRequest>? config = null, CancellationToken token = default) => Client.GetObjectAnnotationAsync(bucketName, objectKey, annotationName, config, token);

    /// <summary>Provides the <c>PutObjectAnnotationAsync</c> API.</summary>
    public Task<PutObjectAnnotationResponse> PutObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Stream content, Action<PutObjectAnnotationRequest>? config = null, CancellationToken token = default) => Client.PutObjectAnnotationAsync(bucketName, objectKey, annotationName, content, config, token);

    /// <summary>Provides the <c>DeleteObjectAnnotationAsync</c> API.</summary>
    public Task<DeleteObjectAnnotationResponse> DeleteObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<DeleteObjectAnnotationRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectAnnotationAsync(bucketName, objectKey, annotationName, config, token);

    /// <summary>Provides the <c>ListObjectAnnotationsAsync</c> API.</summary>
    public Task<ListObjectAnnotationsResponse> ListObjectAnnotationsAsync(string bucketName, string objectKey, Action<ListObjectAnnotationsRequest>? config = null, CancellationToken token = default) => Client.ListObjectAnnotationsAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>GetObjectLegalHoldAsync</c> API.</summary>
    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => Client.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>PutObjectLegalHoldAsync</c> API.</summary>
    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => Client.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);

    /// <summary>Provides the <c>GetObjectTaggingAsync</c> API.</summary>
    public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default) => Client.GetObjectTaggingAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>PutObjectTaggingAsync</c> API.</summary>
    public Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default) => Client.PutObjectTaggingAsync(bucketName, objectKey, tags, config, token);

    /// <summary>Provides the <c>DeleteObjectTaggingAsync</c> API.</summary>
    public Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectTaggingAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>GetObjectRetentionAsync</c> API.</summary>
    public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default) => Client.GetObjectRetentionAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>PutObjectRetentionAsync</c> API.</summary>
    public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default) => Client.PutObjectRetentionAsync(bucketName, objectKey, lockMode, retainUntil, config, token);

    /// <summary>Provides the <c>ListObjectVersionsAsync</c> API.</summary>
    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => Client.ListObjectVersionsAsync(bucketName, config, token);

    /// <summary>Provides the <c>CreateMultipartUploadAsync</c> API.</summary>
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    /// <summary>Provides the <c>UploadPartAsync</c> API.</summary>
    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => Client.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    /// <summary>Provides the <c>UploadPartCopyAsync</c> API.</summary>
    public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => Client.UploadPartCopyAsync(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, partNumber, uploadId, config, token);

    /// <summary>Provides the <c>ListPartsAsync</c> API.</summary>
    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => Client.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    /// <summary>Provides the <c>CompleteMultipartUploadAsync</c> API.</summary>
    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    /// <summary>Provides the <c>AbortMultipartUploadAsync</c> API.</summary>
    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    /// <summary>Provides the <c>ListMultipartUploadsAsync</c> API.</summary>
    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => Client.ListMultipartUploadsAsync(bucketName, config, token);

    /// <summary>Provides the <c>CreateUpload</c> API.</summary>
    public IUpload CreateUpload(string bucket, string objectKey) => Client.CreateUpload(bucket, objectKey);

    /// <summary>Provides the <c>CreateDownload</c> API.</summary>
    public IDownload CreateDownload(string bucket, string objectKey) => Client.CreateDownload(bucket, objectKey);

    /// <summary>Provides the <c>MultipartDownloadAsync</c> API.</summary>
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => Client.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    /// <summary>Provides the <c>MultipartUploadAsync</c> API.</summary>
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => Client.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    /// <summary>Provides the <c>MultipartUploadAsync</c> API.</summary>
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => Client.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);

    /// <summary>Provides the <c>SignRequest&lt;TReq&gt;</c> API.</summary>
    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => Client.SignRequest(request, expiresIn);

    /// <summary>Provides the <c>SendSignedRequestAsync&lt;TResp&gt;</c> API.</summary>
    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => Client.SendSignedRequestAsync<TResp>(url, httpMethod, content, token);

    /// <summary>Provides the <c>GetBucketPolicyAsync</c> API.</summary>
    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default) => Client.GetBucketPolicyAsync(bucketName, config, token);

    /// <summary>Provides the <c>DeleteBucketPolicyAsync</c> API.</summary>
    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketPolicyAsync(bucketName, config, token);

    /// <summary>Provides the <c>PutBucketPolicyAsync</c> API.</summary>
    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default) => Client.PutBucketPolicyAsync(bucketName, policy, config, token);
}