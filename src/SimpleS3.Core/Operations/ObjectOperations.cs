using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utils;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Operations
{
    [PublicAPI]
    public class ObjectOperations : IObjectOperations
    {
        private readonly IRequestHandler _requestHandler;

        public ObjectOperations(IRequestHandler requestHandler, IEnumerable<IRequestWrapper> requestWrappers, IEnumerable<IResponseWrapper> responseWrappers)
        {
            _requestHandler = requestHandler;

            if (requestWrappers != null)
                RequestWrappers = requestWrappers.ToList();

            if (responseWrappers != null)
                ResponseWrappers = responseWrappers.ToList();
        }

        public IList<IRequestWrapper> RequestWrappers { get; }
        public IList<IResponseWrapper> ResponseWrappers { get; }

        public Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteObjectRequest, DeleteObjectResponse>(request, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(HeadObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<HeadObjectRequest, HeadObjectResponse>(request, token);
        }

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

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteObjectsRequest, DeleteObjectsResponse>(request, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default)
        {
            Validator.RequireNotNull(request, nameof(request));

            if (RequestWrappers != null)
            {
                foreach (IRequestWrapper wrapper in RequestWrappers)
                {
                    if (wrapper.IsSupported(request))
                        request.Content = wrapper.Wrap(request.Content, request);
                }
            }

            return _requestHandler.SendRequestAsync<PutObjectRequest, PutObjectResponse>(request, token);
        }

        public async Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken token = default)
        {
            GetObjectResponse response = await _requestHandler.SendRequestAsync<GetObjectRequest, GetObjectResponse>(request, token).ConfigureAwait(false);

            foreach (IResponseWrapper wrapper in ResponseWrappers)
                response.Content.InputStream = wrapper.Wrap(response.Content.AsStream(), response);

            return response;
        }

        public Task<ListObjectsResponse> ListObjectsAsync(ListObjectsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListObjectsRequest, ListObjectsResponse>(request, token);
        }
    }
}