using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Pools
{
    internal class PooledMultipartClient : IMultipartClient
    {
        public PooledMultipartClient(IMultipartOperations multipartOperations)
        {
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
    }
}