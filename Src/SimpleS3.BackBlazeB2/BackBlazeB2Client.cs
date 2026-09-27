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
using Genbox.SimpleS3.Extensions.BackBlazeB2;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.BackBlazeB2;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
/// <remarks>
/// Presigned copies support metadata copying/replacement and all four source ETag/date conditions. Presigned multipart copies support byte ranges.
/// Copy header values use signed transport aliases to avoid Backblaze query/header conflicts.
/// Expired presigned GET and HEAD requests return HTTP 401 in the verified provider scenario, rather than AWS's HTTP 403.
/// </remarks>
public sealed class BackBlazeB2Client : ProviderClientAdapter, ISimpleClient, IDisposable
{
    private readonly SimpleClient _client;
    private readonly ProviderClientOwner? _owner;

    /// <summary>Creates a new instance of <see cref="BackBlazeB2Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public BackBlazeB2Client(string keyId, byte[] accessKey, BackBlazeB2Region region, NetworkConfig? networkConfig = null) : this(new AccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="BackBlazeB2Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public BackBlazeB2Client(string keyId, string accessKey, BackBlazeB2Region region, NetworkConfig? networkConfig = null) : this(new StringAccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="BackBlazeB2Client" /></summary>
    /// <param name="credentials">The credentials to use</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public BackBlazeB2Client(IAccessKey? credentials, BackBlazeB2Region region, NetworkConfig? networkConfig = null) : this(new BackBlazeB2Config(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="BackBlazeB2Client" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public BackBlazeB2Client(BackBlazeB2Config config, NetworkConfig? networkConfig = null) : this(new ProviderClientOwner(new BackblazeB2InputValidator(), config, networkConfig)) {}

    /// <summary>Provides the <c>BackBlazeB2Client</c> API.</summary>
    public BackBlazeB2Client(BackBlazeB2Config config, INetworkDriver networkDriver) : this(new ProviderClientOwner(new BackblazeB2InputValidator(), config, networkDriver: networkDriver)) {}

    private BackBlazeB2Client(ProviderClientOwner owner) : this(owner.Client)
    {
        _owner = owner;
    }

    internal BackBlazeB2Client(SimpleClient client) : base(client)
    {
        _client = client;
    }

    /// <summary>Releases the connection owned by this client.</summary>
    public void Dispose() => _owner?.Dispose();

    /// <inheritdoc cref="SimpleClient.GetBucketLifecycleConfigurationAsync" />
    /// <remarks>Backblaze maps its hide, delete and unfinished-upload rules to S3 lifecycle rules. Expiration rules are returned with a paired orphan-delete-marker rule.</remarks>
    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetBucketLifecycleConfigurationAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketLifecycleConfigurationAsync" />
    /// <remarks>Use enabled, prefix-filtered rules. Expiration in days requires a matching ExpiredObjectDeleteMarker rule. Transitions, date-based expiration, disabled rules and non-prefix filters are unsupported.</remarks>
    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => _client.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteBucketLifecycleAsync" />
    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketLifecycleAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketLoggingAsync" />
    public Task<GetBucketLoggingResponse> GetBucketLoggingAsync(string bucketName, Action<GetBucketLoggingRequest>? config = null, CancellationToken token = default) => _client.GetBucketLoggingAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketLoggingAsync" />
    public Task<PutBucketLoggingResponse> PutBucketLoggingAsync(string bucketName, S3LoggingConfiguration? loggingConfiguration, Action<PutBucketLoggingRequest>? config = null, CancellationToken token = default) => _client.PutBucketLoggingAsync(bucketName, loggingConfiguration, config, token);

    /// <inheritdoc cref="SimpleClient.PutObjectLockConfigurationAsync" />
    public Task<PutObjectLockConfigurationResponse> PutObjectLockConfigurationAsync(string bucketName, bool enabled, Action<PutObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => _client.PutObjectLockConfigurationAsync(bucketName, enabled, config, token);

    /// <inheritdoc cref="SimpleClient.MultipartDownloadAsync" />
    /// <remarks>Falls back to an ordinary GET when the provider does not expose multipart metadata.</remarks>
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    /// <inheritdoc cref="SimpleClient.ListObjectVersionsAsync" />
    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => _client.ListObjectVersionsAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.ListObjectsAsync" />
    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => _client.ListObjectsAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.CopyObjectAsync" />
    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => _client.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);

    /// <inheritdoc cref="SimpleClient.PutObjectAclAsync" />
    /// <remarks>Backblaze accepts only an ACL matching the bucket ACL; objects cannot have independent ACLs.</remarks>
    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => _client.PutObjectAclAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.GetObjectAclAsync" />
    /// <remarks>Returns the bucket ACL for the object, rather than an independently configurable object ACL.</remarks>
    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => _client.GetObjectAclAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.GetObjectRetentionAsync" />
    public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default) => _client.GetObjectRetentionAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.PutObjectRetentionAsync" />
    public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default) => _client.PutObjectRetentionAsync(bucketName, objectKey, lockMode, retainUntil, config, token);

    /// <inheritdoc cref="SimpleClient.CreateBucketAsync" />
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => _client.CreateBucketAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteBucketAsync" />
    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.ListMultipartUploadsAsync" />
    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => _client.ListMultipartUploadsAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.ListBucketsAsync" />
    /// <remarks>The tested service ignores the bucket-name Prefix filter and returns the unfiltered bucket list.</remarks>
    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => _client.ListBucketsAsync(config, token);

    /// <inheritdoc cref="SimpleClient.HeadBucketAsync" />
    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => _client.HeadBucketAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.GetObjectLockConfigurationAsync" />
    public Task<GetObjectLockConfigurationResponse> GetObjectLockConfigurationAsync(string bucketName, Action<GetObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetObjectLockConfigurationAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketAclAsync" />
    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => _client.GetBucketAclAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketAclAsync" />
    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => _client.PutBucketAclAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketLocationAsync" />
    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => _client.GetBucketLocationAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketVersioningAsync" />
    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.GetBucketVersioningAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteObjectAsync" />
    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteObjectsAsync" />
    /// <remarks>Per-object ETag, size and timestamp conditions are ignored by the provider. Any batch containing these conditions is rejected before transport.</remarks>
    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    /// <inheritdoc cref="SimpleClient.HeadObjectAsync" />
    /// <remarks>Only a single strong ETag is supported for read conditions. Comma-separated ETag lists, wildcard conditions and weak If-None-Match tags do not use AWS matching behavior. Explicit delete-marker version reads return HTTP 404 instead of AWS HTTP 405, including presigned reads.</remarks>
    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => _client.HeadObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.CreateMultipartUploadAsync" />
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.UploadPartAsync" />
    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => _client.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    /// <inheritdoc cref="SimpleClient.ListPartsAsync" />
    /// <remarks>Tested SHA256 part digests are returned, but upload checksum algorithm and type metadata are omitted.</remarks>
    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => _client.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc cref="SimpleClient.CompleteMultipartUploadAsync" />
    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    /// <inheritdoc cref="SimpleClient.AbortMultipartUploadAsync" />
    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc cref="SimpleClient.GetObjectAsync" />
    /// <remarks>Only a single strong ETag is supported for read conditions. Comma-separated ETag lists, wildcard conditions and weak If-None-Match tags do not use AWS matching behavior. Explicit delete-marker version reads return HTTP 404 instead of AWS HTTP 405, including presigned reads.</remarks>
    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.GetObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.PutObjectAsync" />
    /// <remarks>Additional-checksum uploads using SHA512, MD5 or xxHash return NotImplemented. Append offsets are rejected before transport because the provider replaces the object instead of appending.</remarks>
    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => _client.PutObjectAsync(bucketName, objectKey, data, config, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);

    /// <inheritdoc cref="SimpleClient.GetObjectLegalHoldAsync" />
    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _client.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.PutObjectLegalHoldAsync" />
    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => _client.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);

    /// <summary>Returns the empty compatibility tag set; Backblaze does not store S3 object tags.</summary>
    public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default) => _client.GetObjectTaggingAsync(bucketName, objectKey, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketCorsAsync" />
    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => _client.GetBucketCorsAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketCorsAsync" />
    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => _client.PutBucketCorsAsync(bucketName, rules, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteBucketCorsAsync" />
    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketCorsAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketEncryptionAsync" />
    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default) => _client.PutBucketEncryptionAsync(bucketName, rules, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketEncryptionAsync" />
    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default) => _client.GetBucketEncryptionAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.DeleteBucketEncryptionAsync" />
    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketEncryptionAsync(bucketName, config, token);

    /// <summary>Accepts enabling versioning. Backblaze always retains versions and rejects suspension.</summary>
    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.PutBucketVersioningAsync(bucketName, enabled, config, token);

    /// <inheritdoc cref="SimpleClient.UploadPartCopyAsync" />
    public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => _client.UploadPartCopyAsync(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, partNumber, uploadId, config, token);
}