using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal class PooledObjectClient(IObjectOperations operations) : IObjectClient
{
    public IObjectOperations ObjectOperations { get; } = operations;

    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteObjectRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }

        Task<DeleteObjectResponse> ActionAsync(DeleteObjectRequest request) => ObjectOperations.DeleteObjectAsync(request, token);
    }

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteObjectsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.DeleteObjectsAsync(request, token));

        void Setup(DeleteObjectsRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, objectKeys);
            config?.Invoke(req);
        }
    }

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<HeadObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.HeadObjectAsync(request, token));

        void Setup(HeadObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectAsync(request, token));

        void Setup(GetObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectAsync(request, token));

        void Setup(PutObjectRequest req)
        {
            req.Initialize(bucketName, objectKey, data);
            config?.Invoke(req);
        }
    }

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListObjectsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.ListObjectsAsync(request, token));

        void Setup(ListObjectsRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<RestoreObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.RestoreObjectsAsync(request, token));

        void Setup(RestoreObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CopyObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.CopyObjectsAsync(request, token));

        void Setup(CopyObjectRequest req)
        {
            req.Initialize(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectAclAsync(request, token));

        void Setup(PutObjectAclRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectAclAsync(request, token));

        void Setup(GetObjectAclRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectLegalHoldAsync(request, token));

        void Setup(GetObjectLegalHoldRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectLegalHoldAsync(request, token));

        void Setup(PutObjectLegalHoldRequest req)
        {
            req.Initialize(bucketName, objectKey, lockStatus);
            config?.Invoke(req);
        }
    }

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListObjectVersionsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.ListObjectVersionsAsync(request, token));

        void Setup(ListObjectVersionsRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }
}