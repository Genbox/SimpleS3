using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
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
        private readonly IObjectOperations _operations;

        public S3ObjectClient(IObjectOperations operations)
        {
            _operations = operations;
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string resource, Action<DeleteObjectRequest> config = null, CancellationToken token = default)
        {
            DeleteObjectRequest req = new DeleteObjectRequest(bucketName, resource);
            config?.Invoke(req);

            return _operations.DeleteAsync(req, token);
        }

        public Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> resources, Action<DeleteMultipleObjectsRequest> config = null, CancellationToken token = default)
        {
            DeleteMultipleObjectsRequest req = new DeleteMultipleObjectsRequest(bucketName, resources);
            config?.Invoke(req);

            return _operations.DeleteMultipleAsync(req, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string resource, Action<HeadObjectRequest> config = null, CancellationToken token = default)
        {
            HeadObjectRequest req = new HeadObjectRequest(bucketName, resource);
            config?.Invoke(req);

            return _operations.HeadAsync(req, token);
        }

        public Task<InitiateMultipartUploadResponse> InitiateMultipartUploadAsync(string bucketName, string resource, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            InitiateMultipartUploadRequest req = new InitiateMultipartUploadRequest(bucketName, resource);
            config?.Invoke(req);

            return _operations.InitiateMultipartUploadAsync(req, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string resource, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default)
        {
            UploadPartRequest req = new UploadPartRequest(bucketName, resource, partNumber, uploadId, content);
            config?.Invoke(req);

            return _operations.UploadPartAsync(req, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string resource, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default)
        {
            ListPartsRequest req = new ListPartsRequest(bucketName, resource, uploadId);
            config?.Invoke(req);

            return _operations.ListPartsAsync(req, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string resource, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            CompleteMultipartUploadRequest req = new CompleteMultipartUploadRequest(bucketName, resource, uploadId, parts);
            config?.Invoke(req);

            return _operations.CompleteMultipartUploadAsync(req, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string resource, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            AbortMultipartUploadRequest req = new AbortMultipartUploadRequest(bucketName, resource, uploadId);
            config?.Invoke(req);

            return _operations.AbortMultipartUploadAsync(req, token);
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string resource, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            GetObjectRequest req = new GetObjectRequest(bucketName, resource);
            config?.Invoke(req);

            return _operations.GetAsync(req, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string resource, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            PutObjectRequest req = new PutObjectRequest(bucketName, resource, data);
            config?.Invoke(req);

            return _operations.PutAsync(req, token);
        }

        public async Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string resource, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            InitiateMultipartUploadRequest req = new InitiateMultipartUploadRequest(bucketName, resource);
            config?.Invoke(req);

            IAsyncEnumerable<UploadPartResponse> asyncEnum = _operations.MultipartUploadAsync(req, data, partSize, numParallelParts, token);

            await foreach (UploadPartResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartUploadStatus.Incomplete;
            }

            return MultipartUploadStatus.Ok;
        }

        public async Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string resource, Stream output, int bufferSize = 16777216, int numParallelParts = 4, CancellationToken token = default)
        {
            IAsyncEnumerable<GetObjectResponse> asyncEnum = _operations.MultipartDownloadAsync(bucketName, resource, output, bufferSize, numParallelParts, null, token);

            await foreach (GetObjectResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartDownloadStatus.Incomplete;
            }

            return MultipartDownloadStatus.Ok;
        }
    }
}