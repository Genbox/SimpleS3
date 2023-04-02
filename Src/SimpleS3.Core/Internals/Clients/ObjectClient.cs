using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal class ObjectClient : IObjectClient
{
    private readonly IObjectOperations _objectOperations;

    public ObjectClient(IObjectOperations operations)
    {
        _objectOperations = operations;
    }

    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
    {
        DeleteObjectRequest req = new DeleteObjectRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.DeleteObjectAsync(req, token);
    }

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
    {
        DeleteObjectsRequest req = new DeleteObjectsRequest(bucketName, objectKeys);
        config?.Invoke(req);

        return _objectOperations.DeleteObjectsAsync(req, token);
    }

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
    {
        HeadObjectRequest req = new HeadObjectRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.HeadObjectAsync(req, token);
    }

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
    {
        GetObjectRequest req = new GetObjectRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.GetObjectAsync(req, token);
    }

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        PutObjectRequest req = new PutObjectRequest(bucketName, objectKey, data);
        config?.Invoke(req);

        return _objectOperations.PutObjectAsync(req, token);
    }

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
    {
        ListObjectsRequest req = new ListObjectsRequest(bucketName);
        config?.Invoke(req);

        return _objectOperations.ListObjectsAsync(req, token);
    }

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
    {
        ListObjectVersionsRequest req = new ListObjectVersionsRequest(bucketName);
        config?.Invoke(req);

        return _objectOperations.ListObjectVersionsAsync(req, token);
    }

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
    {
        RestoreObjectRequest req = new RestoreObjectRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.RestoreObjectsAsync(req, token);
    }

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
    {
        CopyObjectRequest req = new CopyObjectRequest(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey);
        config?.Invoke(req);

        return _objectOperations.CopyObjectsAsync(req, token);
    }

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
    {
        PutObjectAclRequest req = new PutObjectAclRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.PutObjectAclAsync(req, token);
    }

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
    {
        GetObjectAclRequest req = new GetObjectAclRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.GetObjectAclAsync(req, token);
    }

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        GetObjectLegalHoldRequest req = new GetObjectLegalHoldRequest(bucketName, objectKey);
        config?.Invoke(req);

        return _objectOperations.GetObjectLegalHoldAsync(req, token);
    }

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        PutObjectLegalHoldRequest req = new PutObjectLegalHoldRequest(bucketName, objectKey, lockStatus);
        config?.Invoke(req);

        return _objectOperations.PutObjectLegalHoldAsync(req, token);
    }
}