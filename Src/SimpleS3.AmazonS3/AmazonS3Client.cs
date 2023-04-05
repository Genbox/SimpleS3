using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.AmazonS3;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and
/// objects at the same time.</summary>
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
    public AmazonS3Client(IAccessKey credentials, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AmazonS3Config(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(AmazonS3Config config, NetworkConfig? networkConfig = null) : base(new AmazonS3InputValidator(), config, networkConfig) {}

    public AmazonS3Client(AmazonS3Config config, INetworkDriver networkDriver) : base(new AmazonS3InputValidator(), config, networkDriver) {}

    internal AmazonS3Client(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedObjectClient signedObjectClient) : base(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient) {}

    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => Client.CreateBucketAsync(bucketName, config, token);

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketAsync(bucketName, config, token);

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default) => Client.ListBucketsAsync(config, token);

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => Client.HeadBucketAsync(bucketName, config, token);

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketLockConfigurationAsync(bucketName, enabled, config, token);

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketLockConfigurationAsync(bucketName, config, token);

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.GetBucketTaggingAsync(bucketName, config, token);

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.PutBucketTaggingAsync(bucketName, tags, config, token);

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => Client.DeleteBucketTaggingAsync(bucketName, config, token);

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketAccelerateConfigurationAsync(bucketName, enabled, config, token);

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketAccelerateConfigurationAsync(bucketName, config, token);

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => Client.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => Client.PutBucketVersioningAsync(bucketName, enabled, config, token);

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => Client.GetBucketVersioningAsync(bucketName, config, token);

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => Client.GetBucketLifecycleConfigurationAsync(bucketName, config, token);

    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectAsync(bucketName, objectKey, config, token);

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectsAsync(bucketName, objectKeys, config, token);

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => Client.HeadObjectAsync(bucketName, objectKey, config, token);

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => Client.GetObjectAsync(bucketName, objectKey, config, token);

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => Client.PutObjectAsync(bucketName, objectKey, data, config, token);

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => Client.ListObjectsAsync(bucketName, config, token);

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default) => Client.RestoreObjectAsync(bucketName, objectKey, config, token);

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => Client.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => Client.PutObjectAclAsync(bucketName, objectKey, config, token);

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => Client.GetObjectAclAsync(bucketName, objectKey, config, token);

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => Client.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => Client.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => Client.ListObjectVersionsAsync(bucketName, config, token);

    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.CreateMultipartUploadAsync(bucketName, objectKey, config, token);

    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => Client.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);

    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => Client.ListPartsAsync(bucketName, objectKey, uploadId, config, token);

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => Client.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => Client.ListMultipartUploadsAsync(bucketName, config, token);

    public IUpload CreateUpload(string bucket, string objectKey) => Client.CreateUpload(bucket, objectKey);

    public IDownload CreateDownload(string bucket, string objectKey) => Client.CreateDownload(bucket, objectKey);

    public string SignPutObject(string bucketName, string objectKey, Stream? content, TimeSpan expires, Action<PutObjectRequest>? config = null) => Client.SignPutObject(bucketName, objectKey, content, expires, config);

    public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default) => Client.PutObjectAsync(url, content, config, token);

    public string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null) => Client.SignGetObject(bucketName, objectKey, expires, config);

    public Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default) => Client.GetObjectAsync(url, config, token);

    public string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null) => Client.SignDeleteObject(bucketName, objectKey, expires, config);

    public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default) => Client.DeleteObjectAsync(url, config, token);

    public string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null) => Client.SignHeadObject(bucketName, objectKey, expires, config);

    public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default) => Client.HeadObjectAsync(url, config, token);

#if COMMERCIAL
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default) => Client.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => Client.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default) => Client.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);
#endif
}