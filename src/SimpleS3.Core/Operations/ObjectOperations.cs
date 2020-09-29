using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
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
            RequestWrappers = requestWrappers.ToList();
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

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteObjectsRequest, DeleteObjectsResponse>(request, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default)
        {
            Validator.RequireNotNull(request, nameof(request));

            if (request.Content != null)
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
            {
                response.Content = wrapper.Wrap(response.Content, response);
            }

            return response;
        }

        public Task<ListObjectsResponse> ListObjectsAsync(ListObjectsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListObjectsRequest, ListObjectsResponse>(request, token);
        }

        public Task<RestoreObjectResponse> RestoreObjectsAsync(RestoreObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<RestoreObjectRequest, RestoreObjectResponse>(request, token);
        }

        public Task<CopyObjectResponse> CopyObjectsAsync(CopyObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CopyObjectRequest, CopyObjectResponse>(request, token);
        }

        public Task<PutObjectAclResponse> PutObjectAclAsync(PutObjectAclRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<PutObjectAclRequest, PutObjectAclResponse>(request, token);
        }

        public Task<GetObjectAclResponse> GetObjectAclAsync(GetObjectAclRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<GetObjectAclRequest, GetObjectAclResponse>(request, token);
        }

        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(GetObjectLegalHoldRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<GetObjectLegalHoldRequest, GetObjectLegalHoldResponse>(request, token);
        }

        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(PutObjectLegalHoldRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<PutObjectLegalHoldRequest, PutObjectLegalHoldResponse>(request, token);
        }
    }
}