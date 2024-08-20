using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class PooledMultipartClient(IMultipartOperations operations) : IMultipartClient
{
    public IMultipartOperations MultipartOperations { get; } = operations;

    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CreateMultipartUploadRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(CreateMultipartUploadRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }

        Task<CreateMultipartUploadResponse> ActionAsync(CreateMultipartUploadRequest request) => MultipartOperations.CreateMultipartUploadAsync(request, token);
    }

    public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<UploadPartRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(UploadPartRequest req)
        {
            req.Initialize(bucketName, objectKey, uploadId, partNumber, content);
            config?.Invoke(req);
        }

        Task<UploadPartResponse> ActionAsync(UploadPartRequest request) => MultipartOperations.UploadPartAsync(request, token);
    }

    public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListPartsRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(ListPartsRequest req)
        {
            req.Initialize(bucketName, objectKey, uploadId);
            config?.Invoke(req);
        }

        Task<ListPartsResponse> ActionAsync(ListPartsRequest request) => MultipartOperations.ListPartsAsync(request, token);
    }

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CompleteMultipartUploadRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(CompleteMultipartUploadRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, objectKey, uploadId, parts);
            config?.Invoke(req);
        }

        Task<CompleteMultipartUploadResponse> ActionAsync(CompleteMultipartUploadRequest request) => MultipartOperations.CompleteMultipartUploadAsync(request, token);
    }

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<AbortMultipartUploadRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(AbortMultipartUploadRequest req)
        {
            req.Initialize(bucketName, objectKey, uploadId);
            config?.Invoke(req);
        }

        Task<AbortMultipartUploadResponse> ActionAsync(AbortMultipartUploadRequest request) => MultipartOperations.AbortMultipartUploadAsync(request, token);
    }

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListMultipartUploadsRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(ListMultipartUploadsRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<ListMultipartUploadsResponse> ActionAsync(ListMultipartUploadsRequest request) => MultipartOperations.ListMultipartUploadsAsync(request, token);
    }
}