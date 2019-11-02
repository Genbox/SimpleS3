using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Operations
{
    [PublicAPI]
    public class MultipartOperations : IMultipartOperations
    {
        private readonly IRequestHandler _requestHandler;

        public MultipartOperations(IRequestHandler requestHandler, IEnumerable<IRequestWrapper> requestWrappers, IEnumerable<IResponseWrapper> responseWrappers)
        {
            _requestHandler = requestHandler;

            if (requestWrappers != null)
                RequestWrappers = requestWrappers.ToList();

            if (responseWrappers != null)
                ResponseWrappers = responseWrappers.ToList();
        }

        public IList<IRequestWrapper> RequestWrappers { get; }
        public IList<IResponseWrapper> ResponseWrappers { get; }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CreateMultipartUploadRequest, CreateMultipartUploadResponse>(request, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<UploadPartRequest, UploadPartResponse>(request, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListPartsRequest, ListPartsResponse>(request, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>(request, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<AbortMultipartUploadRequest, AbortMultipartUploadResponse>(request, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(ListMultipartUploadsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListMultipartUploadsRequest, ListMultipartUploadsResponse>(request, token);
        }
    }
}