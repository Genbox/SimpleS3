using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public class SimpleClient : ISimpleClient
{
    private IBucketClient _bucketClient;
    private IMultipartClient _multipartClient;
    private IMultipartTransfer _multipartTransfer;
    private IObjectClient _objectClient;
    private ITransfer _transfer;
    private ISignedObjectClient _signedObjectClient;

    public SimpleClient(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedObjectClient signedObject)
    {
        Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObject);
    }

    protected SimpleClient() { }

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.ListObjectsAsync(bucketName, config, token);
    }

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.RestoreObjectAsync(bucketName, objectKey, config, token);
    }

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);
    }

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.PutObjectAclAsync(bucketName, objectKey, config, token);
    }

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.GetObjectAclAsync(bucketName, objectKey, config, token);
    }

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);
    }

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);
    }

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.ListObjectVersionsAsync(bucketName, config, token);
    }

    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.CreateBucketAsync(bucketName, config, token);
    }

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.DeleteBucketAsync(bucketName, config, token);
    }

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.ListMultipartUploadsAsync(bucketName, config, token);
    }

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.ListBucketsAsync(config, token);
    }

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.HeadBucketAsync(bucketName, config, token);
    }

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.PutBucketLockConfigurationAsync(bucketName, enabled, config, token);
    }

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.GetBucketLockConfigurationAsync(bucketName, config, token);
    }

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.GetBucketTaggingAsync(bucketName, config, token);
    }

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.PutBucketTaggingAsync(bucketName, tags, config, token);
    }

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.DeleteBucketTaggingAsync(bucketName, config, token);
    }

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.PutBucketAccelerateConfigurationAsync(bucketName, enabled, config, token);
    }

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.GetBucketAccelerateConfigurationAsync(bucketName, config, token);
    }

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);
    }

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.PutBucketVersioningAsync(bucketName, enabled, config, token);
    }

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.GetBucketVersioningAsync(bucketName, config, token);
    }

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return _bucketClient.GetBucketLifecycleConfigurationAsync(bucketName, config, token);
    }

    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.DeleteObjectAsync(bucketName, objectKey, config, token);
    }

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.DeleteObjectsAsync(bucketName, objectKeys, config, token);
    }

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.HeadObjectAsync(bucketName, objectKey, config, token);
    }

    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.CreateMultipartUploadAsync(bucketName, objectKey, config, token);
    }

    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);
    }

    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.ListPartsAsync(bucketName, objectKey, uploadId, config, token);
    }

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);
    }

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return _multipartClient.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);
    }

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.GetObjectAsync(bucketName, objectKey, config, token);
    }

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        return _objectClient.PutObjectAsync(bucketName, objectKey, data, config, token);
    }

    public IUpload CreateUpload(string bucket, string objectKey)
    {
        return _transfer.CreateUpload(bucket, objectKey);
    }

    public IDownload CreateDownload(string bucket, string objectKey)
    {
        return _transfer.CreateDownload(bucket, objectKey);
    }

    protected void Initialize(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedObjectClient signedObjectClient)
    {
        _objectClient = objectClient;
        _bucketClient = bucketClient;
        _multipartClient = multipartClient;
        _multipartTransfer = multipartTransfer;
        _transfer = transfer;
        _signedObjectClient = signedObjectClient;
    }

#if COMMERCIAL
    public IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default)
    {
        return _multipartTransfer.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);
    }

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
    {
        return _multipartTransfer.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, onPartResponse, token);
    }

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
    {
        return _multipartTransfer.MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);
    }
#endif
    public string SignPutObject(string bucketName, string objectKey, Stream? content, TimeSpan expires, Action<PutObjectRequest>? config = null)
    {
        return _signedObjectClient.SignPutObject(bucketName, objectKey, content, expires, config);
    }

    public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default)
    {
        return _signedObjectClient.PutObjectAsync(url, content, config, token);
    }

    public string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null)
    {
        return _signedObjectClient.SignGetObject(bucketName, objectKey, expires, config);
    }

    public Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default)
    {
        return _signedObjectClient.GetObjectAsync(url, config, token);
    }

    public string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null)
    {
        return _signedObjectClient.SignDeleteObject(bucketName, objectKey, expires, config);
    }

    public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default)
    {
        return _signedObjectClient.DeleteObjectAsync(url, config, token);
    }

    public string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null)
    {
        return _signedObjectClient.SignHeadObject(bucketName, objectKey, expires, config);
    }

    public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default)
    {
        return _signedObjectClient.HeadObjectAsync(url, config, token);
    }
}