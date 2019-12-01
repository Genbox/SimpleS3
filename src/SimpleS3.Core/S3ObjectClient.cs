using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3ObjectClient : IS3ObjectClient
    {
        public S3ObjectClient(IObjectOperations operations)
        {
            ObjectOperations = operations;
        }

        public IObjectOperations ObjectOperations { get; }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest> config = null, CancellationToken token = default)
        {
            DeleteObjectRequest req = new DeleteObjectRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.DeleteObjectAsync(req, token);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default)
        {
            DeleteObjectsRequest req = new DeleteObjectsRequest(bucketName, objectKeys);
            config?.Invoke(req);

            return ObjectOperations.DeleteObjectsAsync(req, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest> config = null, CancellationToken token = default)
        {
            HeadObjectRequest req = new HeadObjectRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.HeadObjectAsync(req, token);
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            GetObjectRequest req = new GetObjectRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.GetObjectAsync(req, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            PutObjectRequest req = new PutObjectRequest(bucketName, objectKey, data);
            config?.Invoke(req);

            return ObjectOperations.PutObjectAsync(req, token);
        }

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default)
        {
            ListObjectsRequest req = new ListObjectsRequest(bucketName);
            config?.Invoke(req);

            return ObjectOperations.ListObjectsAsync(req, token);
        }

        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest> config = null, CancellationToken token = default)
        {
            RestoreObjectRequest req = new RestoreObjectRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.RestoreObjectsAsync(req, token);
        }

        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest> config = null, CancellationToken token = default)
        {
            CopyObjectRequest req = new CopyObjectRequest(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey);
            config?.Invoke(req);

            return ObjectOperations.CopyObjectsAsync(req, token);
        }

        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest> config = null, CancellationToken token = default)
        {
            PutObjectAclRequest req = new PutObjectAclRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.PutObjectAclAsync(req, token);
        }

        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest> config = null, CancellationToken token = default)
        {
            GetObjectAclRequest req = new GetObjectAclRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.GetObjectAclAsync(req, token);
        }
    }
}