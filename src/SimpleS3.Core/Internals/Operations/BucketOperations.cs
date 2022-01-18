using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal class BucketOperations : IBucketOperations
{
    private readonly IRequestHandler _requestHandler;

    public BucketOperations(IRequestHandler requestHandler)
    {
        _requestHandler = requestHandler;
    }

    public Task<CreateBucketResponse> CreateBucketAsync(CreateBucketRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<CreateBucketRequest, CreateBucketResponse>(request, token);
    }

    public Task<DeleteBucketResponse> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<DeleteBucketRequest, DeleteBucketResponse>(request, token);
    }

    public Task<ListBucketsResponse> ListBucketsAsync(ListBucketsRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<ListBucketsRequest, ListBucketsResponse>(request, token);
    }

    public Task<HeadBucketResponse> HeadBucketAsync(HeadBucketRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<HeadBucketRequest, HeadBucketResponse>(request, token);
    }

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(PutBucketLockConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<PutBucketLockConfigurationRequest, PutBucketLockConfigurationResponse>(request, token);
    }

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(GetBucketLockConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<GetBucketLockConfigurationRequest, GetBucketLockConfigurationResponse>(request, token);
    }

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(GetBucketTaggingRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<GetBucketTaggingRequest, GetBucketTaggingResponse>(request, token);
    }

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(PutBucketTaggingRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<PutBucketTaggingRequest, PutBucketTaggingResponse>(request, token);
    }

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse>(request, token);
    }

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse>(request, token);
    }

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse>(request, token);
    }

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(PutBucketLifecycleConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<PutBucketLifecycleConfigurationRequest, PutBucketLifecycleConfigurationResponse>(request, token);
    }

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(PutBucketVersioningRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<PutBucketVersioningRequest, PutBucketVersioningResponse>(request, token);
    }

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(GetBucketVersioningRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<GetBucketVersioningRequest, GetBucketVersioningResponse>(request, token);
    }

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(GetBucketLifecycleConfigurationRequest request, CancellationToken token = default)
    {
        return _requestHandler.SendRequestAsync<GetBucketLifecycleConfigurationRequest, GetBucketLifecycleConfigurationResponse>(request, token);
    }
}