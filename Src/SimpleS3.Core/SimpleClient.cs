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

namespace Genbox.SimpleS3.Core;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public class SimpleClient : ISimpleClient
{
    private readonly IBucketClient _bucketClient;
    private readonly IMultipartClient _multipartClient;
    private readonly IMultipartTransfer _multipartTransfer;
    private readonly IObjectClient _objectClient;
    private readonly ISignedClient _signedClient;
    private readonly ITransfer _transfer;

    public SimpleClient(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedClient signedObject)
    {
        _objectClient = objectClient;
        _bucketClient = bucketClient;
        _multipartClient = multipartClient;
        _multipartTransfer = multipartTransfer;
        _transfer = transfer;
        _signedClient = signedObject;
    }

    protected SimpleClient() {}

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => _objectClient.ListObjectsAsync(bucketName, config, token);

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default) => _objectClient.RestoreObjectAsync(bucketName, objectKey, config, token);

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => _objectClient.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => _objectClient.PutObjectAclAsync(bucketName, objectKey, config, token);

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectAclAsync(bucketName, objectKey, config, token);

    public Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectAttributesAsync(bucketName, objectKey, objectAttributes, config, token);

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _objectClient.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);

    public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectTaggingAsync(bucketName, objectKey, config, token);

    public Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default) => _objectClient.PutObjectTaggingAsync(bucketName, objectKey, tags, config, token);

    public Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default) => _objectClient.DeleteObjectTaggingAsync(bucketName, objectKey, config, token);

    public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectRetentionAsync(bucketName, objectKey, config, token);

    public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default) => _objectClient.PutObjectRetentionAsync(bucketName, objectKey, lockMode, retainUntil, config, token);

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => _objectClient.ListObjectVersionsAsync(bucketName, config, token);

    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => _bucketClient.CreateBucketAsync(bucketName, config, token);

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketAsync(bucketName, config, token);

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => _multipartClient.ListMultipartUploadsAsync(bucketName, config, token);

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => _bucketClient.ListBucketsAsync(config, token);

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => _bucketClient.HeadBucketAsync(bucketName, config, token);

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketLockConfigurationAsync(bucketName, enabled, config, token);

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketLockConfigurationAsync(bucketName, config, token);

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketTaggingAsync(bucketName, config, token);

    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketAclAsync(bucketName, config, token);

    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketAclAsync(bucketName, config, token);

    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketCorsAsync(bucketName, config, token);

    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketCorsAsync(bucketName, rules, config, token);

    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketCorsAsync(bucketName, config, token);

    public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketOwnershipControlsAsync(bucketName, config, token);

    public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketOwnershipControlsAsync(bucketName, rules, config, token);

    public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketOwnershipControlsAsync(bucketName, config, token);

    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketLocationAsync(bucketName, config, token);

    public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketRequestPaymentAsync(bucketName, config, token);

    public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketRequestPaymentAsync(bucketName, payer, config, token);

    public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketPolicyStatusAsync(bucketName, config, token);

    public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketWebsiteAsync(bucketName, config, token);

    public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketWebsiteAsync(bucketName, websiteConfiguration, config, token);

    public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketWebsiteAsync(bucketName, config, token);

    public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketAnalyticsConfigurationAsync(bucketName, configurationId, config, token);

    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketInventoryConfigurationAsync(bucketName, configurationId, config, token);

    public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketMetricsConfigurationAsync(bucketName, configurationId, config, token);

    public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketIntelligentTieringConfigurationAsync(bucketName, configurationId, config, token);

    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketReplicationAsync(bucketName, config, token);

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketTaggingAsync(bucketName, tags, config, token);

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketTaggingAsync(bucketName, config, token);

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketAccelerateConfigurationAsync(bucketName, enabled, config, token);

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketAccelerateConfigurationAsync(bucketName, config, token);

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);

    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketLifecycleAsync(bucketName, config, token);

    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketEncryptionAsync(bucketName, rules, config, token);

    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketEncryptionAsync(bucketName, config, token);

    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketEncryptionAsync(bucketName, config, token);

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketVersioningAsync(bucketName, enabled, config, token);

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketVersioningAsync(bucketName, config, token);

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketLifecycleConfigurationAsync(bucketName, config, token);

    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default) => _bucketClient.PutPublicAccessBlockAsync(bucketName, config, token);

    public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default) => _bucketClient.GetPublicAccessBlockAsync(bucketName, config, token);

    public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default) => _bucketClient.DeletePublicAccessBlockAsync(bucketName, config, token);

    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default) => _bucketClient.GetBucketPolicyAsync(bucketName, config, token);

    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default) => _bucketClient.DeleteBucketPolicyAsync(bucketName, config, token);

    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default) => _bucketClient.PutBucketPolicyAsync(bucketName, policy, config, token);

    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => _objectClient.DeleteObjectAsync(bucketName, objectKey, config, token);

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => _objectClient.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => _objectClient.HeadObjectAsync(bucketName, objectKey, config, token);

    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => _multipartClient.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => _multipartClient.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => _multipartClient.UploadPartCopyAsync(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, partNumber, uploadId, config, token);

    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => _multipartClient.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => _multipartClient.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => _multipartClient.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _objectClient.GetObjectAsync(bucketName, objectKey, config, token);

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => _objectClient.PutObjectAsync(bucketName, objectKey, data, config, token);

    public IUpload CreateUpload(string bucket, string objectKey) => _transfer.CreateUpload(bucket, objectKey);

    public IDownload CreateDownload(string bucket, string objectKey) => _transfer.CreateDownload(bucket, objectKey);

    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _multipartTransfer.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _multipartTransfer.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _multipartTransfer.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);

    public string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest => _signedClient.SignRequest(request, expiresIn);

    public Task<TResp> SendSignedRequestAsync<TResp>(string url, HttpMethodType httpMethod, Stream? content = null, CancellationToken token = default) where TResp : IResponse, new() => _signedClient.SendSignedRequestAsync<TResp>(url, httpMethod, content, token);
}