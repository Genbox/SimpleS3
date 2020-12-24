using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Pools
{
    internal class PooledMultipartClient : IMultipartClient
    {
        private readonly IObjectOperations _objectOperations;

        public PooledMultipartClient(IMultipartOperations multipartOperations, IObjectOperations objectOperations)
        {
            _objectOperations = objectOperations;
            MultipartOperations = multipartOperations;
        }

        public IMultipartOperations MultipartOperations { get; }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            void Setup(CreateMultipartUploadRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            Task<CreateMultipartUploadResponse> Action(CreateMultipartUploadRequest request) => MultipartOperations.CreateMultipartUploadAsync(request, token);

            return ObjectPool<CreateMultipartUploadRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
        {
            void Setup(UploadPartRequest req)
            {
                req.Initialize(bucketName, objectKey, partNumber, uploadId, content);
                config?.Invoke(req);
            }

            Task<UploadPartResponse> Action(UploadPartRequest request) => MultipartOperations.UploadPartAsync(request, token);

            return ObjectPool<UploadPartRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(ListPartsRequest req)
            {
                req.Initialize(bucketName, objectKey, uploadId);
                config?.Invoke(req);
            }

            Task<ListPartsResponse> Action(ListPartsRequest request) => MultipartOperations.ListPartsAsync(request, token);

            return ObjectPool<ListPartsRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            void Setup(CompleteMultipartUploadRequest req)
            {
                req.Initialize(bucketName, objectKey, uploadId, parts);
                config?.Invoke(req);
            }

            Task<CompleteMultipartUploadResponse> Action(CompleteMultipartUploadRequest request) => MultipartOperations.CompleteMultipartUploadAsync(request, token);

            return ObjectPool<CompleteMultipartUploadRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            void Setup(AbortMultipartUploadRequest req)
            {
                req.Initialize(bucketName, objectKey, uploadId);
                config?.Invoke(req);
            }

            Task<AbortMultipartUploadResponse> Action(AbortMultipartUploadRequest request) => MultipartOperations.AbortMultipartUploadAsync(request, token);

            return ObjectPool<AbortMultipartUploadRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(ListMultipartUploadsRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<ListMultipartUploadsResponse> Action(ListMultipartUploadsRequest request) => MultipartOperations.ListMultipartUploadsAsync(request, token);

            return ObjectPool<ListMultipartUploadsRequest>.Shared.RentAndUse(Setup, Action);
        }

        public async Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            IAsyncEnumerable<UploadPartResponse> asyncEnum = MultipartOperations.MultipartUploadAsync(req, data, partSize, numParallelParts, token);

            await foreach (UploadPartResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartUploadStatus.Incomplete;
            }

            return MultipartUploadStatus.Ok;
        }

        public async Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, CancellationToken token = default)
        {
            IAsyncEnumerable<GetObjectResponse> asyncEnum = _objectOperations.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, null, token);

            await foreach (GetObjectResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartDownloadStatus.Incomplete;
            }

            return MultipartDownloadStatus.Ok;
        }
    }
}