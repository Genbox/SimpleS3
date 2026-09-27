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
using Genbox.SimpleS3.Extensions.GoogleCloudStorage;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.GoogleCloudStorage;

/// <summary>Provides the Google Cloud Storage client and its supported S3-compatible operations.</summary>
public sealed class GoogleCloudStorageClient : ProviderClientAdapter, ISimpleClient, IDisposable
{
    private readonly SimpleClient _client;
    private readonly ProviderClientOwner? _owner;

    /// <summary>Creates a new instance of <see cref="GoogleCloudStorageClient" /></summary>
    public GoogleCloudStorageClient(string keyId, byte[] accessKey, GoogleCloudStorageRegion region, NetworkConfig? networkConfig = null) : this(new AccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GoogleCloudStorageClient" /></summary>
    public GoogleCloudStorageClient(string keyId, string accessKey, GoogleCloudStorageRegion region, NetworkConfig? networkConfig = null) : this(new StringAccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GoogleCloudStorageClient" /></summary>
    public GoogleCloudStorageClient(IAccessKey? credentials, GoogleCloudStorageRegion region, NetworkConfig? networkConfig = null) : this(new GoogleCloudStorageConfig(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GoogleCloudStorageClient" /></summary>
    public GoogleCloudStorageClient(GoogleCloudStorageConfig config, NetworkConfig? networkConfig = null) : this(new ProviderClientOwner(new GoogleCloudStorageInputValidator(), config, networkConfig)) {}

    /// <summary>Creates a new instance of <see cref="GoogleCloudStorageClient" /></summary>
    public GoogleCloudStorageClient(GoogleCloudStorageConfig config, INetworkDriver networkDriver) : this(new ProviderClientOwner(new GoogleCloudStorageInputValidator(), config, networkDriver: networkDriver)) {}

    private GoogleCloudStorageClient(ProviderClientOwner owner) : this(owner.Client)
    {
        _owner = owner;
    }

    internal GoogleCloudStorageClient(SimpleClient client) : base(client)
    {
        _client = client;
    }

    /// <summary>Releases the services owned by this client.</summary>
    public void Dispose() => _owner?.Dispose();

    /// <inheritdoc cref="SimpleClient.DeleteObjectsAsync" />
    /// <remarks>Deletes up to 1,000 objects or generations. GCS versioning and soft-delete settings determine whether deleted objects remain recoverable.</remarks>
    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    /// <inheritdoc cref="SimpleClient.GetBucketTaggingAsync" />
    /// <remarks>Returns GCS bucket labels through the interoperable tagging XML format.</remarks>
    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => _client.GetBucketTaggingAsync(bucketName, config, token);

    /// <inheritdoc cref="SimpleClient.PutBucketTaggingAsync" />
    /// <remarks>Sets GCS bucket labels. Keys and values must satisfy GCS label restrictions.</remarks>
    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => _client.PutBucketTaggingAsync(bucketName, tags, config, token);

    /// <inheritdoc cref="SimpleClient.GetObjectLockConfigurationAsync" />
    /// <remarks>Reports whether object retention is enabled. GCS does not return an S3 default-retention rule; a bucket without object retention returns HTTP 404.</remarks>
    public Task<GetObjectLockConfigurationResponse> GetObjectLockConfigurationAsync(string bucketName, Action<GetObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => _client.GetObjectLockConfigurationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => _client.CreateBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => _client.DeleteBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => _client.ListBucketsAsync(config, token);

    /// <inheritdoc />
    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => _client.HeadBucketAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => _client.GetBucketAclAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => _client.PutBucketAclAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => _client.GetBucketLocationAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.GetBucketVersioningAsync(bucketName, config, token);

    /// <summary>Enables or disables GCS object versioning.</summary>
    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => _client.PutBucketVersioningAsync(bucketName, enabled, config, token);

    /// <inheritdoc />
    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => _client.DeleteObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => _client.HeadObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.GetObjectAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => _client.PutObjectAsync(bucketName, objectKey, data, config, token);

    /// <inheritdoc />
    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => _client.ListObjectsAsync(bucketName, config, token);

    /// <inheritdoc />
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
    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => _client.ListObjectVersionsAsync(bucketName, config, token);

    /// <inheritdoc />
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    /// <inheritdoc />
    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => _client.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    /// <inheritdoc />
    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => _client.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    /// <inheritdoc />
    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => _client.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    /// <inheritdoc />
    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => _client.ListMultipartUploadsAsync(bucketName, config, token);

    /// <inheritdoc />
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => _client.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    /// <inheritdoc />
    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => _client.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);
}