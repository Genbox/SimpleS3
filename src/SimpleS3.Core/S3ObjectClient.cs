using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;
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

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            return ObjectOperations.CreateMultipartUploadAsync(req, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default)
        {
            UploadPartRequest req = new UploadPartRequest(bucketName, objectKey, partNumber, uploadId, content);
            config?.Invoke(req);

            return ObjectOperations.UploadPartAsync(req, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default)
        {
            ListPartsRequest req = new ListPartsRequest(bucketName, objectKey, uploadId);
            config?.Invoke(req);

            return ObjectOperations.ListPartsAsync(req, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            CompleteMultipartUploadRequest req = new CompleteMultipartUploadRequest(bucketName, objectKey, uploadId, parts);
            config?.Invoke(req);

            return ObjectOperations.CompleteMultipartUploadAsync(req, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            AbortMultipartUploadRequest req = new AbortMultipartUploadRequest(bucketName, objectKey, uploadId);
            config?.Invoke(req);

            return ObjectOperations.AbortMultipartUploadAsync(req, token);
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

        public async Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            IAsyncEnumerable<UploadPartResponse> asyncEnum = MultipartHelper.MultipartUploadAsync(ObjectOperations, req, data, partSize, numParallelParts, token);

            await foreach (UploadPartResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartUploadStatus.Incomplete;
            }

            return MultipartUploadStatus.Ok;
        }

        public async Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, CancellationToken token = default)
        {
            IAsyncEnumerable<GetObjectResponse> asyncEnum = MultipartHelper.MultipartDownloadAsync(ObjectOperations, bucketName, objectKey, output, bufferSize, numParallelParts, null, token);

            await foreach (GetObjectResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartDownloadStatus.Incomplete;
            }

            return MultipartDownloadStatus.Ok;
        }
    }
}