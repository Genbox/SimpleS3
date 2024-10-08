﻿using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class MultipartClient(IMultipartOperations operations) : IMultipartClient
{
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
        config?.Invoke(req);

        return operations.CreateMultipartUploadAsync(req, token);
    }

    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
    {
        UploadPartRequest req = new UploadPartRequest(bucketName, objectKey, uploadId, partNumber, content);
        config?.Invoke(req);

        return operations.UploadPartAsync(req, token);
    }

    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
    {
        ListPartsRequest req = new ListPartsRequest(bucketName, objectKey, uploadId);
        config?.Invoke(req);

        return operations.ListPartsAsync(req, token);
    }

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        CompleteMultipartUploadRequest req = new CompleteMultipartUploadRequest(bucketName, objectKey, uploadId, parts);
        config?.Invoke(req);

        return operations.CompleteMultipartUploadAsync(req, token);
    }

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        AbortMultipartUploadRequest req = new AbortMultipartUploadRequest(bucketName, objectKey, uploadId);
        config?.Invoke(req);

        return operations.AbortMultipartUploadAsync(req, token);
    }

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
    {
        ListMultipartUploadsRequest request = new ListMultipartUploadsRequest(bucketName);
        config?.Invoke(request);

        return operations.ListMultipartUploadsAsync(request, token);
    }
}