using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal sealed class MultipartOperations(IRequestHandler requestHandler) : IMultipartOperations
{
    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<CreateMultipartUploadRequest, CreateMultipartUploadResponse>(request, token);

    public Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<UploadPartRequest, UploadPartResponse>(request, token);

    public Task<UploadPartCopyResponse> UploadPartCopyAsync(UploadPartCopyRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<UploadPartCopyRequest, UploadPartCopyResponse>(request, token);

    public Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<ListPartsRequest, ListPartsResponse>(request, token);

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>(request, token);

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<AbortMultipartUploadRequest, AbortMultipartUploadResponse>(request, token);

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(ListMultipartUploadsRequest request, CancellationToken token = default) => requestHandler.SendRequestAsync<ListMultipartUploadsRequest, ListMultipartUploadsResponse>(request, token);
}