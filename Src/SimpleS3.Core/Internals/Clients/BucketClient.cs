using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class BucketClient(IBucketOperations operations) : IBucketClient
{
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
    {
        CreateBucketRequest request = new CreateBucketRequest(bucketName);
        config?.Invoke(request);

        return operations.CreateBucketAsync(request, token);
    }

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketRequest request = new DeleteBucketRequest(bucketName);
        config?.Invoke(request);

        return operations.DeleteBucketAsync(request, token);
    }

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
    {
        ListBucketsRequest request = new ListBucketsRequest();
        config?.Invoke(request);

        return operations.ListBucketsAsync(request, token);
    }

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
    {
        HeadBucketRequest request = new HeadBucketRequest(bucketName);
        config?.Invoke(request);

        return operations.HeadBucketAsync(request, token);
    }

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        PutBucketLockConfigurationRequest request = new PutBucketLockConfigurationRequest(bucketName, enabled);
        config?.Invoke(request);

        return operations.PutBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        GetBucketLockConfigurationRequest request = new GetBucketLockConfigurationRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        GetBucketTaggingRequest request = new GetBucketTaggingRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketTaggingAsync(request, token);
    }

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        PutBucketTaggingRequest request = new PutBucketTaggingRequest(bucketName, tags);
        config?.Invoke(request);

        return operations.PutBucketTaggingAsync(request, token);
    }

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketTaggingRequest request = new DeleteBucketTaggingRequest(bucketName);
        config?.Invoke(request);

        return operations.DeleteBucketTaggingAsync(request, token);
    }

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        PutBucketAccelerateConfigurationRequest request = new PutBucketAccelerateConfigurationRequest(bucketName, enabled);
        config?.Invoke(request);

        return operations.PutBucketAccelerateConfigurationAsync(request, token);
    }

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        GetBucketAccelerateConfigurationRequest request = new GetBucketAccelerateConfigurationRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketAccelerateConfigurationAsync(request, token);
    }

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        PutBucketLifecycleConfigurationRequest request = new PutBucketLifecycleConfigurationRequest(bucketName, rules);
        config?.Invoke(request);

        return operations.PutBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        PutBucketVersioningRequest request = new PutBucketVersioningRequest(bucketName, enabled);
        config?.Invoke(request);

        return operations.PutBucketVersioningAsync(request, token);
    }

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        GetBucketVersioningRequest request = new GetBucketVersioningRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketVersioningAsync(request, token);
    }

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        GetBucketLifecycleConfigurationRequest request = new GetBucketLifecycleConfigurationRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        PutPublicAccessBlockRequest request = new PutPublicAccessBlockRequest(bucketName);
        config?.Invoke(request);

        return operations.PutPublicAccessBlockAsync(request, token);
    }

    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        GetBucketPolicyRequest request = new GetBucketPolicyRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketPolicyAsync(request, token);
    }

    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketPolicyRequest request = new DeleteBucketPolicyRequest(bucketName);
        config?.Invoke(request);

        return operations.DeleteBucketPolicyAsync(request, token);
    }

    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        PutBucketPolicyRequest request = new PutBucketPolicyRequest(bucketName, policy);
        config?.Invoke(request);

        return operations.PutBucketPolicyAsync(request, token);
    }
}