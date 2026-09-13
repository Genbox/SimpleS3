using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal sealed class BucketOperations(IRequestHandler handler) : IBucketOperations
{
    public Task<CreateBucketResponse> CreateBucketAsync(CreateBucketRequest request, CancellationToken token = default) => handler.SendRequestAsync<CreateBucketRequest, CreateBucketResponse>(request, token);

    public Task<DeleteBucketResponse> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketRequest, DeleteBucketResponse>(request, token);

    public Task<ListBucketsResponse> ListBucketsAsync(ListBucketsRequest request, CancellationToken token = default) => handler.SendRequestAsync<ListBucketsRequest, ListBucketsResponse>(request, token);

    public Task<HeadBucketResponse> HeadBucketAsync(HeadBucketRequest request, CancellationToken token = default) => handler.SendRequestAsync<HeadBucketRequest, HeadBucketResponse>(request, token);

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(PutBucketLockConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketLockConfigurationRequest, PutBucketLockConfigurationResponse>(request, token);

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(GetBucketLockConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketLockConfigurationRequest, GetBucketLockConfigurationResponse>(request, token);

    public Task<GetBucketLoggingResponse> GetBucketLoggingAsync(GetBucketLoggingRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketLoggingRequest, GetBucketLoggingResponse>(request, token);

    public Task<PutBucketLoggingResponse> PutBucketLoggingAsync(PutBucketLoggingRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketLoggingRequest, PutBucketLoggingResponse>(request, token);

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(GetBucketTaggingRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketTaggingRequest, GetBucketTaggingResponse>(request, token);

    public Task<GetBucketAclResponse> GetBucketAclAsync(GetBucketAclRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketAclRequest, GetBucketAclResponse>(request, token);

    public Task<PutBucketAclResponse> PutBucketAclAsync(PutBucketAclRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketAclRequest, PutBucketAclResponse>(request, token);

    public Task<GetBucketCorsResponse> GetBucketCorsAsync(GetBucketCorsRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketCorsRequest, GetBucketCorsResponse>(request, token);

    public Task<PutBucketCorsResponse> PutBucketCorsAsync(PutBucketCorsRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketCorsRequest, PutBucketCorsResponse>(request, token);

    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(DeleteBucketCorsRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketCorsRequest, DeleteBucketCorsResponse>(request, token);

    public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(GetBucketOwnershipControlsRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketOwnershipControlsRequest, GetBucketOwnershipControlsResponse>(request, token);

    public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(PutBucketOwnershipControlsRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketOwnershipControlsRequest, PutBucketOwnershipControlsResponse>(request, token);

    public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(DeleteBucketOwnershipControlsRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketOwnershipControlsRequest, DeleteBucketOwnershipControlsResponse>(request, token);

    public Task<GetBucketLocationResponse> GetBucketLocationAsync(GetBucketLocationRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketLocationRequest, GetBucketLocationResponse>(request, token);

    public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(GetBucketRequestPaymentRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse>(request, token);

    public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(PutBucketRequestPaymentRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse>(request, token);

    public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(GetBucketPolicyStatusRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketPolicyStatusRequest, GetBucketPolicyStatusResponse>(request, token);

    public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(GetBucketWebsiteRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketWebsiteRequest, GetBucketWebsiteResponse>(request, token);

    public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(PutBucketWebsiteRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketWebsiteRequest, PutBucketWebsiteResponse>(request, token);

    public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(DeleteBucketWebsiteRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse>(request, token);

    public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(DeleteBucketAnalyticsConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse>(request, token);

    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse>(request, token);

    public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(DeleteBucketMetricsConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse>(request, token);

    public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(DeleteBucketIntelligentTieringConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketIntelligentTieringConfigurationRequest, DeleteBucketIntelligentTieringConfigurationResponse>(request, token);

    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(DeleteBucketReplicationRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse>(request, token);

    public Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(UpdateBucketMetadataAnnotationTableConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<UpdateBucketMetadataAnnotationTableConfigurationRequest, UpdateBucketMetadataAnnotationTableConfigurationResponse>(request, token);

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(PutBucketTaggingRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketTaggingRequest, PutBucketTaggingResponse>(request, token);

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse>(request, token);

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse>(request, token);

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse>(request, token);

    public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(PutBucketLifecycleConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketLifecycleConfigurationRequest, PutBucketLifecycleConfigurationResponse>(request, token);

    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(DeleteBucketLifecycleRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketLifecycleRequest, DeleteBucketLifecycleResponse>(request, token);

    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(PutBucketEncryptionRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketEncryptionRequest, PutBucketEncryptionResponse>(request, token);

    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(GetBucketEncryptionRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketEncryptionRequest, GetBucketEncryptionResponse>(request, token);

    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(DeleteBucketEncryptionRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketEncryptionRequest, DeleteBucketEncryptionResponse>(request, token);

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(PutBucketVersioningRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketVersioningRequest, PutBucketVersioningResponse>(request, token);

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(GetBucketVersioningRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketVersioningRequest, GetBucketVersioningResponse>(request, token);

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(GetBucketLifecycleConfigurationRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketLifecycleConfigurationRequest, GetBucketLifecycleConfigurationResponse>(request, token);

    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(PutPublicAccessBlockRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutPublicAccessBlockRequest, PutPublicAccessBlockResponse>(request, token);

    public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(GetPublicAccessBlockRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetPublicAccessBlockRequest, GetPublicAccessBlockResponse>(request, token);

    public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(DeletePublicAccessBlockRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeletePublicAccessBlockRequest, DeletePublicAccessBlockResponse>(request, token);

    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(GetBucketPolicyRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketPolicyRequest, GetBucketPolicyResponse>(request, token);

    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(DeleteBucketPolicyRequest request, CancellationToken token = default) => handler.SendRequestAsync<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse>(request, token);

    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(PutBucketPolicyRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketPolicyRequest, PutBucketPolicyResponse>(request, token);

    public Task<GetBucketAbacResponse> GetBucketAbacAsync(GetBucketAbacRequest request, CancellationToken token = default) => handler.SendRequestAsync<GetBucketAbacRequest, GetBucketAbacResponse>(request, token);

    public Task<PutBucketAbacResponse> PutBucketAbacAsync(PutBucketAbacRequest request, CancellationToken token = default) => handler.SendRequestAsync<PutBucketAbacRequest, PutBucketAbacResponse>(request, token);
}