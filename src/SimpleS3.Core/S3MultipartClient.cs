﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3MultipartClient : IMultipartClient
    {
        private readonly IObjectOperations _objectOperations;

        public S3MultipartClient(IMultipartOperations multipartOperations, IObjectOperations objectOperations)
        {
            _objectOperations = objectOperations;
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

        public async Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            MultipartUploadStatus status = MultipartUploadStatus.Ok;

            CompleteMultipartUploadResponse? uploadResp = await MultipartOperations.MultipartUploadAsync(req, data, onPartResponse: response =>
            {
                if (!response.IsSuccess)
                    status = MultipartUploadStatus.Incomplete;
            }, token: token);

            if (uploadResp == null || !uploadResp.IsSuccess)
                status = MultipartUploadStatus.Incomplete;

            return status;
        }

        public async Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default)
        {
            IAsyncEnumerable<GetObjectResponse> asyncEnum = _objectOperations.MultipartDownloadAsync(bucketName, objectKey, output, bufferSize, numParallelParts, config, token);

            await foreach (GetObjectResponse obj in asyncEnum.WithCancellation(token))
            {
                if (!obj.IsSuccess)
                    return MultipartDownloadStatus.Incomplete;
            }

            return MultipartDownloadStatus.Ok;
        }
    }
}