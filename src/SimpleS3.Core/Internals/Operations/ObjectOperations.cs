using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal class ObjectOperations : IObjectOperations
{
    private readonly IRequestHandler _requestHandler;
    private readonly List<IRequestWrapper> _requestWrappers;
    private readonly List<IResponseWrapper> _responseWrappers;

    public ObjectOperations(IRequestHandler requestHandler, IEnumerable<IRequestWrapper> requestWrappers, IEnumerable<IResponseWrapper> responseWrappers)
    {
        _requestHandler = requestHandler;
        _requestWrappers = requestWrappers.ToList();
        _responseWrappers = responseWrappers.ToList();
    }

    public Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<DeleteObjectRequest, DeleteObjectResponse>(request, token);

    public Task<HeadObjectResponse> HeadObjectAsync(HeadObjectRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<HeadObjectRequest, HeadObjectResponse>(request, token);

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<DeleteObjectsRequest, DeleteObjectsResponse>(request, token);

    public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default)
    {
        Validator.RequireNotNull(request, nameof(request));

        if (request.Content != null)
        {
            foreach (IRequestWrapper wrapper in _requestWrappers)
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

        foreach (IResponseWrapper wrapper in _responseWrappers)
            response.Content = wrapper.Wrap(response.Content, response);

        return response;
    }

    public Task<ListObjectsResponse> ListObjectsAsync(ListObjectsRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<ListObjectsRequest, ListObjectsResponse>(request, token);

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(ListObjectVersionsRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<ListObjectVersionsRequest, ListObjectVersionsResponse>(request, token);

    public Task<RestoreObjectResponse> RestoreObjectsAsync(RestoreObjectRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<RestoreObjectRequest, RestoreObjectResponse>(request, token);

    public Task<CopyObjectResponse> CopyObjectsAsync(CopyObjectRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<CopyObjectRequest, CopyObjectResponse>(request, token);

    public Task<PutObjectAclResponse> PutObjectAclAsync(PutObjectAclRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<PutObjectAclRequest, PutObjectAclResponse>(request, token);

    public Task<GetObjectAclResponse> GetObjectAclAsync(GetObjectAclRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<GetObjectAclRequest, GetObjectAclResponse>(request, token);

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(GetObjectLegalHoldRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<GetObjectLegalHoldRequest, GetObjectLegalHoldResponse>(request, token);

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(PutObjectLegalHoldRequest request, CancellationToken token = default) => _requestHandler.SendRequestAsync<PutObjectLegalHoldRequest, PutObjectLegalHoldResponse>(request, token);
}