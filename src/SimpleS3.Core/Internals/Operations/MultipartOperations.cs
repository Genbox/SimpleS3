using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal class MultipartOperations : IMultipartOperations
{
    private readonly IRequestHandler _requestHandler;

    public MultipartOperations(IRequestHandler requestHandler, IEnumerable<IRequestWrapper>? requestWrappers = null, IEnumerable<IResponseWrapper>? responseWrappers = null)
    {
        _requestHandler = requestHandler;

        if (requestWrappers != null)
            RequestWrappers = requestWrappers.ToList();
        else
            RequestWrappers = Array.Empty<IRequestWrapper>();

        if (responseWrappers != null)
            ResponseWrappers = responseWrappers.ToList();
        else
            ResponseWrappers = Array.Empty<IResponseWrapper>();
    }

    public IList<IRequestWrapper> RequestWrappers { get; }
    public IList<IResponseWrapper> ResponseWrappers { get; }

    public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<CreateMultipartUploadRequest, CreateMultipartUploadResponse>(request, token);

    public Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<UploadPartRequest, UploadPartResponse>(request, token);

    public Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<ListPartsRequest, ListPartsResponse>(request, token);

    public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>(request, token);

    public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<AbortMultipartUploadRequest, AbortMultipartUploadResponse>(request, token);

    public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(ListMultipartUploadsRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<ListMultipartUploadsRequest, ListMultipartUploadsResponse>(request, token);
}