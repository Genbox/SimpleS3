#if COMMERCIAL
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Clients
{
    internal class PooledObjectClient : IObjectClient
    {
        public PooledObjectClient(IObjectOperations operations)
        {
            ObjectOperations = operations;
        }

        public IObjectOperations ObjectOperations { get; }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(DeleteObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            Task<DeleteObjectResponse> Action(DeleteObjectRequest request) => ObjectOperations.DeleteObjectAsync(request, token);

            return ObjectPool<DeleteObjectRequest>.Shared.RentAndUseAsync(Setup, Action);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(DeleteObjectsRequest req)
            {
                req.Initialize(bucketName, objectKeys);
                config?.Invoke(req);
            }

            return ObjectPool<DeleteObjectsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.DeleteObjectsAsync(request, token));
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(HeadObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<HeadObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.HeadObjectAsync(request, token));
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectAsync(request, token));
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutObjectRequest req)
            {
                req.Initialize(bucketName, objectKey, data);
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectAsync(request, token));
        }

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(ListObjectsRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            return ObjectPool<ListObjectsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.ListObjectsAsync(request, token));
        }

        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(RestoreObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<RestoreObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.RestoreObjectsAsync(request, token));
        }

        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(CopyObjectRequest req)
            {
                req.Initialize(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey);
                config?.Invoke(req);
            }

            return ObjectPool<CopyObjectRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.CopyObjectsAsync(request, token));
        }

        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutObjectAclRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectAclAsync(request, token));
        }

        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetObjectAclRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectAclAsync(request, token));
        }

        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetObjectLegalHoldRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.GetObjectLegalHoldAsync(request, token));
        }

        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutObjectLegalHoldRequest req)
            {
                req.Initialize(bucketName, objectKey, lockStatus);
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.PutObjectLegalHoldAsync(request, token));
        }

        public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(ListObjectVersionsRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            return ObjectPool<ListObjectVersionsRequest>.Shared.RentAndUseAsync(Setup, request => ObjectOperations.ListObjectVersionsAsync(request, token));
        }
    }
}
#endif