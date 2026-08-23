using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
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

    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default)
    {
        GetBucketAclRequest request = new GetBucketAclRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketAclAsync(request, token);
    }

    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default)
    {
        PutBucketAclRequest request = new PutBucketAclRequest(bucketName);
        config?.Invoke(request);
        return operations.PutBucketAclAsync(request, token);
    }

    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default)
    {
        GetBucketCorsRequest request = new GetBucketCorsRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketCorsAsync(request, token);
    }

    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default)
    {
        PutBucketCorsRequest request = new PutBucketCorsRequest(bucketName, rules);
        config?.Invoke(request);
        return operations.PutBucketCorsAsync(request, token);
    }

    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketCorsRequest request = new DeleteBucketCorsRequest(bucketName);
        config?.Invoke(request);
        return operations.DeleteBucketCorsAsync(request, token);
    }

    public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default)
    {
        GetBucketOwnershipControlsRequest request = new GetBucketOwnershipControlsRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketOwnershipControlsAsync(request, token);
    }

    public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default)
    {
        PutBucketOwnershipControlsRequest request = new PutBucketOwnershipControlsRequest(bucketName, rules);
        config?.Invoke(request);
        return operations.PutBucketOwnershipControlsAsync(request, token);
    }

    public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketOwnershipControlsRequest request = new DeleteBucketOwnershipControlsRequest(bucketName);
        config?.Invoke(request);
        return operations.DeleteBucketOwnershipControlsAsync(request, token);
    }

    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default)
    {
        GetBucketLocationRequest request = new GetBucketLocationRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketLocationAsync(request, token);
    }

    public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default)
    {
        GetBucketRequestPaymentRequest request = new GetBucketRequestPaymentRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketRequestPaymentAsync(request, token);
    }

    public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default)
    {
        PutBucketRequestPaymentRequest request = new PutBucketRequestPaymentRequest(bucketName, payer);
        config?.Invoke(request);
        return operations.PutBucketRequestPaymentAsync(request, token);
    }

    public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default)
    {
        GetBucketPolicyStatusRequest request = new GetBucketPolicyStatusRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketPolicyStatusAsync(request, token);
    }

    public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default)
    {
        GetBucketWebsiteRequest request = new GetBucketWebsiteRequest(bucketName);
        config?.Invoke(request);
        return operations.GetBucketWebsiteAsync(request, token);
    }

    public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default)
    {
        PutBucketWebsiteRequest request = new PutBucketWebsiteRequest(bucketName, websiteConfiguration);
        config?.Invoke(request);
        return operations.PutBucketWebsiteAsync(request, token);
    }

    public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketWebsiteRequest request = new DeleteBucketWebsiteRequest(bucketName);
        config?.Invoke(request);
        return operations.DeleteBucketWebsiteAsync(request, token);
    }

    public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketAnalyticsConfigurationRequest request = new DeleteBucketAnalyticsConfigurationRequest(bucketName, configurationId);
        config?.Invoke(request);
        return operations.DeleteBucketAnalyticsConfigurationAsync(request, token);
    }

    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketInventoryConfigurationRequest request = new DeleteBucketInventoryConfigurationRequest(bucketName, configurationId);
        config?.Invoke(request);
        return operations.DeleteBucketInventoryConfigurationAsync(request, token);
    }

    public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketMetricsConfigurationRequest request = new DeleteBucketMetricsConfigurationRequest(bucketName, configurationId);
        config?.Invoke(request);
        return operations.DeleteBucketMetricsConfigurationAsync(request, token);
    }

    public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketIntelligentTieringConfigurationRequest request = new DeleteBucketIntelligentTieringConfigurationRequest(bucketName, configurationId);
        config?.Invoke(request);
        return operations.DeleteBucketIntelligentTieringConfigurationAsync(request, token);
    }

    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketReplicationRequest request = new DeleteBucketReplicationRequest(bucketName);
        config?.Invoke(request);
        return operations.DeleteBucketReplicationAsync(request, token);
    }

    public Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config = null, CancellationToken token = default)
    {
        UpdateBucketMetadataAnnotationTableConfigurationRequest request = new UpdateBucketMetadataAnnotationTableConfigurationRequest(bucketName, configuration);
        config?.Invoke(request);
        return operations.UpdateBucketMetadataAnnotationTableConfigurationAsync(request, token);
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

    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketLifecycleRequest request = new DeleteBucketLifecycleRequest(bucketName);
        config?.Invoke(request);

        return operations.DeleteBucketLifecycleAsync(request, token);
    }

    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        PutBucketEncryptionRequest request = new PutBucketEncryptionRequest(bucketName, rules);
        config?.Invoke(request);

        return operations.PutBucketEncryptionAsync(request, token);
    }

    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        GetBucketEncryptionRequest request = new GetBucketEncryptionRequest(bucketName);
        config?.Invoke(request);

        return operations.GetBucketEncryptionAsync(request, token);
    }

    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        DeleteBucketEncryptionRequest request = new DeleteBucketEncryptionRequest(bucketName);
        config?.Invoke(request);

        return operations.DeleteBucketEncryptionAsync(request, token);
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

    public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        GetPublicAccessBlockRequest request = new GetPublicAccessBlockRequest(bucketName);
        config?.Invoke(request);

        return operations.GetPublicAccessBlockAsync(request, token);
    }

    public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        DeletePublicAccessBlockRequest request = new DeletePublicAccessBlockRequest(bucketName);
        config?.Invoke(request);

        return operations.DeletePublicAccessBlockAsync(request, token);
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