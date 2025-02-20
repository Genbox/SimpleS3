using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal class PooledBucketClient(IBucketOperations operations) : IBucketClient
{
    public IBucketOperations BucketOperations { get; } = operations;

    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CreateBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(CreateBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<CreateBucketResponse> ActionAsync(CreateBucketRequest request) => BucketOperations.CreateBucketAsync(request, token);
    }

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketResponse> ActionAsync(DeleteBucketRequest request) => BucketOperations.DeleteBucketAsync(request, token);
    }

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListBucketsRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(ListBucketsRequest req) => config?.Invoke(req);

        Task<ListBucketsResponse> ActionAsync(ListBucketsRequest request) => BucketOperations.ListBucketsAsync(request, token);
    }

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<HeadBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(HeadBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<HeadBucketResponse> ActionAsync(HeadBucketRequest request) => BucketOperations.HeadBucketAsync(request, token);
    }

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketLockConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketLockConfigurationRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketLockConfigurationResponse> ActionAsync(PutBucketLockConfigurationRequest request) => BucketOperations.PutBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketLockConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketLockConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketLockConfigurationResponse> ActionAsync(GetBucketLockConfigurationRequest request) => BucketOperations.GetBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketTaggingRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketTaggingResponse> ActionAsync(GetBucketTaggingRequest request) => BucketOperations.GetBucketTaggingAsync(request, token);
    }

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketTaggingRequest req)
        {
            req.Initialize(bucketName, tags);
            config?.Invoke(req);
        }

        Task<PutBucketTaggingResponse> ActionAsync(PutBucketTaggingRequest request) => BucketOperations.PutBucketTaggingAsync(request, token);
    }

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketTaggingRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketTaggingResponse> ActionAsync(DeleteBucketTaggingRequest request) => BucketOperations.DeleteBucketTaggingAsync(request, token);
    }

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketAccelerateConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketAccelerateConfigurationRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketAccelerateConfigurationResponse> ActionAsync(PutBucketAccelerateConfigurationRequest request) => BucketOperations.PutBucketAccelerateConfigurationAsync(request, token);
    }

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketAccelerateConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketAccelerateConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketAccelerateConfigurationResponse> ActionAsync(GetBucketAccelerateConfigurationRequest request) => BucketOperations.GetBucketAccelerateConfigurationAsync(request, token);
    }

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketLifecycleConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketLifecycleConfigurationRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, rules);
            config?.Invoke(req);
        }

        Task<PutBucketLifecycleConfigurationResponse> ActionAsync(PutBucketLifecycleConfigurationRequest request) => BucketOperations.PutBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketVersioningRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketVersioningRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketVersioningResponse> ActionAsync(PutBucketVersioningRequest request) => BucketOperations.PutBucketVersioningAsync(request, token);
    }

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketVersioningRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketVersioningRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketVersioningResponse> ActionAsync(GetBucketVersioningRequest request) => BucketOperations.GetBucketVersioningAsync(request, token);
    }

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketLifecycleConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketLifecycleConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketLifecycleConfigurationResponse> ActionAsync(GetBucketLifecycleConfigurationRequest request) => BucketOperations.GetBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutPublicAccessBlockRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutPublicAccessBlockRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<PutPublicAccessBlockResponse> ActionAsync(PutPublicAccessBlockRequest request) => BucketOperations.PutPublicAccessBlockAsync(request, token);
    }

    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketPolicyRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketPolicyResponse> ActionAsync(GetBucketPolicyRequest request) => BucketOperations.GetBucketPolicyAsync(request, token);
    }

    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketPolicyRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketPolicyResponse> ActionAsync(DeleteBucketPolicyRequest request) => BucketOperations.DeleteBucketPolicyAsync(request, token);
    }

    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketPolicyRequest req)
        {
            req.Initialize(bucketName, policy);
            config?.Invoke(req);
        }

        Task<PutBucketPolicyResponse> ActionAsync(PutBucketPolicyRequest request) => BucketOperations.PutBucketPolicyAsync(request, token);
    }
}