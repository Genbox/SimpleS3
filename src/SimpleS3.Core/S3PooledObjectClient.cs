using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    /// <summary>
    /// This client pools all request objects to minimize the pressure on the garbage collector and free up memory.
    /// </summary>
    [PublicAPI]
    public class S3PooledObjectClient : IObjectClient
    {
        public S3PooledObjectClient(IObjectOperations operations)
        {
            ObjectOperations = operations;
        }

        public IObjectOperations ObjectOperations { get; }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(DeleteObjectRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            Task<DeleteObjectResponse> Action(DeleteObjectRequest request) => ObjectOperations.DeleteObjectAsync(request, token);

            return ObjectPool<DeleteObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default)
        {
            void Setup(DeleteObjectsRequest req)
            {
                req.BucketName = bucketName;
                req.Objects = objectKeys.ToList();
                config?.Invoke(req);
            }

            return ObjectPool<DeleteObjectsRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.DeleteObjectsAsync(request, token));
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(HeadObjectRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<HeadObjectRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.HeadObjectAsync(request, token));
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(GetObjectRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.GetObjectAsync(request, token));
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(PutObjectRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                req.Content = data;
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.PutObjectAsync(request, token));
        }

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default)
        {
            void Setup(ListObjectsRequest req)
            {
                req.BucketName = bucketName;
                config?.Invoke(req);
            }

            return ObjectPool<ListObjectsRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.ListObjectsAsync(request, token));
        }

        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(RestoreObjectRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<RestoreObjectRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.RestoreObjectsAsync(request, token));
        }

        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest> config = null, CancellationToken token = default)
        {
            void Setup(CopyObjectRequest req)
            {
                req.SourceBucketName = sourceBucketName;
                req.SourceObjectKey = sourceObjectKey;
                req.DestinationBucketName = destinationBucket;
                req.DestinationObjectKey = destinationObjectKey;
                config?.Invoke(req);
            }

            return ObjectPool<CopyObjectRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.CopyObjectsAsync(request, token));
        }

        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest> config = null, CancellationToken token = default)
        {
            void Setup(PutObjectAclRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectAclRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.PutObjectAclAsync(request, token));
        }

        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest> config = null, CancellationToken token = default)
        {
            void Setup(GetObjectAclRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectAclRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.GetObjectAclAsync(request, token));
        }

        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest> config = null, CancellationToken token = default)
        {
            void Setup(GetObjectLegalHoldRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                config?.Invoke(req);
            }

            return ObjectPool<GetObjectLegalHoldRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.GetObjectLegalHoldAsync(request, token));
        }

        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest> config = null, CancellationToken token = default)
        {
            void Setup(PutObjectLegalHoldRequest req)
            {
                req.BucketName = bucketName;
                req.ObjectKey = objectKey;
                req.LockLegalHold = lockStatus;
                config?.Invoke(req);
            }

            return ObjectPool<PutObjectLegalHoldRequest>.Shared.RentAndUse(Setup, request => ObjectOperations.PutObjectLegalHoldAsync(request, token));
        }
    }
}