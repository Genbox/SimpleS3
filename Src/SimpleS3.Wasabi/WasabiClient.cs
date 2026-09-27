using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.Wasabi;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.Wasabi;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public sealed class WasabiClient : ProviderClientAdapter, ISimpleClient, IDisposable
{
    private readonly SimpleClient _client;
    private readonly ProviderClientOwner? _owner;

    /// <summary>Creates a new instance of <see cref="WasabiClient" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public WasabiClient(string keyId, byte[] accessKey, WasabiRegion region, NetworkConfig? networkConfig = null) : this(new AccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="WasabiClient" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public WasabiClient(string keyId, string accessKey, WasabiRegion region, NetworkConfig? networkConfig = null) : this(new StringAccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="WasabiClient" /></summary>
    /// <param name="credentials">The credentials to use</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public WasabiClient(IAccessKey? credentials, WasabiRegion region, NetworkConfig? networkConfig = null) : this(new WasabiConfig(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="WasabiClient" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public WasabiClient(WasabiConfig config, NetworkConfig? networkConfig = null) : this(new ProviderClientOwner(new WasabiInputValidator(), config, networkConfig)) {}

    /// <summary>Provides the <c>WasabiClient</c> API.</summary>
    public WasabiClient(WasabiConfig config, INetworkDriver networkDriver) : this(new ProviderClientOwner(new WasabiInputValidator(), config, networkDriver: networkDriver)) {}

    private WasabiClient(ProviderClientOwner owner) : this(owner.Client)
    {
        _owner = owner;
    }

    internal WasabiClient(SimpleClient client) : base(client)
    {
        _client = client;
    }

    /// <summary>Releases the connection owned by this client.</summary>
    public void Dispose() => _owner?.Dispose();

    /// <inheritdoc cref="SimpleClient.DeleteBucketReplicationAsync" />
    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketReplicationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _client.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketLifecycleAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetBucketLifecycleConfigurationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketLoggingResponse> GetBucketLoggingAsync(string bucketName, Action<GetBucketLoggingRequest>? config = null, CancellationToken token = default) => _client.GetBucketLoggingAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketLoggingResponse> PutBucketLoggingAsync(string bucketName, S3LoggingConfiguration? loggingConfiguration, Action<PutBucketLoggingRequest>? config = null, CancellationToken token = default) => _client.PutBucketLoggingAsync(bucketName, loggingConfiguration, config, token);

    /// <inheritdoc />
    public Task<PutObjectLockConfigurationResponse> PutObjectLockConfigurationAsync(string bucketName, bool enabled, Action<PutObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => _client.PutObjectLockConfigurationAsync(bucketName, enabled, config, token);

    /// <inheritdoc />
    /// <remarks>Uses byte ranges for parts because Wasabi GET ignores partNumber. Output response lengths are checked before writing.</remarks>
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    /// <inheritdoc />
    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => _client.ListObjectVersionsAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => _client.ListObjectsAsync(bucketName, config, token);

    /// <inheritdoc />
    /// <remarks>Source ETag and date conditions are rejected before sending because Wasabi does not enforce them.</remarks>
    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => _client.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);

    /// <inheritdoc />
    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => _client.PutObjectAclAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => _client.GetObjectAclAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default) => _client.GetObjectRetentionAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default) => _client.PutObjectRetentionAsync(bucketName, objectKey, lockMode, retainUntil, config, token);

    /// <inheritdoc />
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => _client.CreateBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => _client.ListMultipartUploadsAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => _client.ListBucketsAsync(config, token);

    /// <inheritdoc />
    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => _client.HeadBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetObjectLockConfigurationResponse> GetObjectLockConfigurationAsync(string bucketName, Action<GetObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetObjectLockConfigurationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => _client.GetBucketAclAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => _client.PutBucketAclAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => _client.GetBucketLocationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.GetBucketVersioningAsync(bucketName, config, token);

    /// <inheritdoc />
    /// <remarks>Wasabi ignores S3 ETag, size and timestamp conditions on deletes. ETag, size and timestamp conditions are rejected before transport.</remarks>
    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    /// <remarks>Per-object ETag, size and timestamp conditions are ignored by the provider. Any batch containing these conditions is rejected before transport.</remarks>
    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    /// <inheritdoc />
    /// <remarks>
    /// When IfETagNotMatch is set, SimpleS3 omits IfModifiedSince to preserve AWS ETag precedence. Only a single strong ETag is supported for read conditions. Comma-separated ETag lists, wildcard conditions and weak If-None-Match tags do not use AWS matching behavior. A byte range starting at the
    /// object length is ignored and returns the full object with HTTP 200 instead of 416, including through presigned requests. Explicit delete-marker version reads return HTTP 404 instead of AWS HTTP 405, including presigned reads.
    /// </remarks>
    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => _client.HeadObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => _client.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    /// <inheritdoc />
    /// <remarks>Tested SHA256 part digests and algorithm are returned, but upload checksum type metadata is omitted.</remarks>
    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => _client.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc />
    /// <remarks>Conditional multipart completion is unsupported. IfMatch and IfNoneMatch are rejected before transport.</remarks>
    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    /// <inheritdoc />
    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc />
    /// <remarks>
    /// Wasabi ignores PartNumber on GET and returns the full object. Use Range for a partial read or MultipartDownloadAsync to assemble parts. When IfETagNotMatch is set, SimpleS3 omits IfModifiedSince to preserve AWS ETag precedence. Only a single strong ETag is supported for read conditions.
    /// Comma-separated ETag lists, wildcard conditions and weak If-None-Match tags do not use AWS matching behavior. A byte range starting at the object length is ignored and returns the full object with HTTP 200 instead of 416, including through presigned requests. Explicit delete-marker version reads
    /// return HTTP 404 instead of AWS HTTP 405, including presigned reads.
    /// </remarks>
    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.GetObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    /// <remarks>
    /// Wasabi rejects the tested Unicode upload-tag key/value pair with InvalidTag; empty tag values are supported. Wasabi ignores upload conditions. Requests with IfMatch or IfNoneMatch are rejected before transport. Additional-checksum uploads using SHA512, MD5 or xxHash return InvalidRequest.
    /// Append offsets are rejected before transport because the provider replaces the object instead of appending.
    /// </remarks>
    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => _client.PutObjectAsync(bucketName, objectKey, data, config, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);

    /// <inheritdoc />
    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _client.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _client.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);

    /// <inheritdoc />
    public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default) => _client.GetObjectTaggingAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    /// <remarks>The tested Unicode tag keys and values are rejected with InvalidTag; existing tags remain intact. Empty tag values are supported.</remarks>
    public Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default) => _client.PutObjectTaggingAsync(bucketName, objectKey, tags, config, token);

    /// <inheritdoc />
    public Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectTaggingAsync(bucketName, objectKey, config, token);

    /// <summary>Gets object attributes, verified against Wasabi despite the documented exclusion.</summary>
    /// <remarks>Wasabi returns invalid pagination marker values. When IsTruncated is true, use the last returned part's PartNumber as the next request's PartNumberMarker.</remarks>
    public Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default) => _client.GetObjectAttributesAsync(bucketName, objectKey, objectAttributes, config, token);

    /// <summary>Gets CORS rules. Wasabi does not return rule IDs in the tested responses.</summary>
    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => _client.GetBucketCorsAsync(bucketName, config, token);

    /// <summary>Sets CORS rules. Rule IDs are not preserved in the tested Wasabi responses.</summary>
    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => _client.PutBucketCorsAsync(bucketName, rules, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteBucketCorsAsync" />
    /// <remarks>Deleting custom rules restores Wasabi's permissive default CORS configuration; it does not disable cross-origin requests. Object access policies still apply.</remarks>
    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketCorsAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => _client.GetBucketTaggingAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketInventoryConfigurationAsync(bucketName, configurationId, config, token);

    /// <inheritdoc />
    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => _client.PutBucketTaggingAsync(bucketName, tags, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketTaggingAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.PutBucketVersioningAsync(bucketName, enabled, config, token);

    /// <inheritdoc />
    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default) => _client.GetBucketPolicyAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketPolicyAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default) => _client.PutBucketPolicyAsync(bucketName, policy, config, token);

    /// <inheritdoc />
    public Task<GetBucketInventoryConfigurationResponse> GetBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<GetBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetBucketInventoryConfigurationAsync(bucketName, configurationId, config, token);

    /// <inheritdoc />
    public Task<PutBucketInventoryConfigurationResponse> PutBucketInventoryConfigurationAsync(string bucketName, string configurationId, S3InventoryConfiguration configuration, Action<PutBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => _client.PutBucketInventoryConfigurationAsync(bucketName, configurationId, configuration, config, token);

    /// <inheritdoc />
    public Task<ListBucketInventoryConfigurationsResponse> ListBucketInventoryConfigurationsAsync(string bucketName, Action<ListBucketInventoryConfigurationsRequest>? config = null, CancellationToken token = default) => _client.ListBucketInventoryConfigurationsAsync(bucketName, config, token);

    /// <inheritdoc />
    /// <remarks>Source ETag and date conditions are rejected before sending because Wasabi does not enforce them.</remarks>
    public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => _client.UploadPartCopyAsync(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, partNumber, uploadId, config, token);
}