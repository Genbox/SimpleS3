using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3MultipartClient : IMultipartClient
    {
        public S3MultipartClient(IMultipartOperations multipartOperations)
        {
            MultipartOperations = multipartOperations;
        }

        public IMultipartOperations MultipartOperations { get; }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            return MultipartOperations.CreateMultipartUploadAsync(req, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
        {
            UploadPartRequest req = new UploadPartRequest(bucketName, objectKey, partNumber, uploadId, content);
            config?.Invoke(req);

            return MultipartOperations.UploadPartAsync(req, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
        {
            ListPartsRequest req = new ListPartsRequest(bucketName, objectKey, uploadId);
            config?.Invoke(req);

            return MultipartOperations.ListPartsAsync(req, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            CompleteMultipartUploadRequest req = new CompleteMultipartUploadRequest(bucketName, objectKey, uploadId, parts);
            config?.Invoke(req);

            return MultipartOperations.CompleteMultipartUploadAsync(req, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            AbortMultipartUploadRequest req = new AbortMultipartUploadRequest(bucketName, objectKey, uploadId);
            config?.Invoke(req);

            return MultipartOperations.AbortMultipartUploadAsync(req, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            ListMultipartUploadsRequest request = new ListMultipartUploadsRequest(bucketName);
            config?.Invoke(request);

            return MultipartOperations.ListMultipartUploadsAsync(request, token);
        }
    }
}