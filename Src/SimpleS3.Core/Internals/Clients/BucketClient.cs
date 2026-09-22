using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class BucketClient(IBucketOperations operations) : IBucketClient
{
    public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CreateBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(CreateBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<CreateBucketResponse> ActionAsync(CreateBucketRequest request) => operations.CreateBucketAsync(request, token);
    }

    public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketResponse> ActionAsync(DeleteBucketRequest request) => operations.DeleteBucketAsync(request, token);
    }

    public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListBucketsRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(ListBucketsRequest req) => config?.Invoke(req);

        Task<ListBucketsResponse> ActionAsync(ListBucketsRequest request) => operations.ListBucketsAsync(request, token);
    }

    public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<HeadBucketRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(HeadBucketRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<HeadBucketResponse> ActionAsync(HeadBucketRequest request) => operations.HeadBucketAsync(request, token);
    }

    public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketLockConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketLockConfigurationRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketLockConfigurationResponse> ActionAsync(PutBucketLockConfigurationRequest request) => operations.PutBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketLockConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketLockConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketLockConfigurationResponse> ActionAsync(GetBucketLockConfigurationRequest request) => operations.GetBucketLockConfigurationAsync(request, token);
    }

    public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketTaggingRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketTaggingResponse> ActionAsync(GetBucketTaggingRequest request) => operations.GetBucketTaggingAsync(request, token);
    }

    public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketAclRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketAclAsync(req, token));

    public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketAclRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.PutBucketAclAsync(req, token));

    public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketCorsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketCorsAsync(req, token));

    public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketCorsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, rules);
        config?.Invoke(req);
    }, req => operations.PutBucketCorsAsync(req, token));

    public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketCorsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.DeleteBucketCorsAsync(req, token));

    public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketOwnershipControlsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketOwnershipControlsAsync(req, token));

    public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketOwnershipControlsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, rules);
        config?.Invoke(req);
    }, req => operations.PutBucketOwnershipControlsAsync(req, token));

    public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketOwnershipControlsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.DeleteBucketOwnershipControlsAsync(req, token));

    public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketLocationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketLocationAsync(req, token));

    public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketRequestPaymentRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketRequestPaymentAsync(req, token));

    public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketRequestPaymentRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, payer);
        config?.Invoke(req);
    }, req => operations.PutBucketRequestPaymentAsync(req, token));

    public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketPolicyStatusRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketPolicyStatusAsync(req, token));

    public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketWebsiteRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketWebsiteAsync(req, token));

    public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketWebsiteRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, websiteConfiguration);
        config?.Invoke(req);
    }, req => operations.PutBucketWebsiteAsync(req, token));

    public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketWebsiteRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.DeleteBucketWebsiteAsync(req, token));

    public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketAnalyticsConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configurationId);
        config?.Invoke(req);
    }, req => operations.DeleteBucketAnalyticsConfigurationAsync(req, token));

    public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketInventoryConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configurationId);
        config?.Invoke(req);
    }, req => operations.DeleteBucketInventoryConfigurationAsync(req, token));

    public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketMetricsConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configurationId);
        config?.Invoke(req);
    }, req => operations.DeleteBucketMetricsConfigurationAsync(req, token));

    public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketIntelligentTieringConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configurationId);
        config?.Invoke(req);
    }, req => operations.DeleteBucketIntelligentTieringConfigurationAsync(req, token));

    public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketReplicationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.DeleteBucketReplicationAsync(req, token));

    public Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<UpdateBucketMetadataAnnotationTableConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configuration);
        config?.Invoke(req);
    }, req => operations.UpdateBucketMetadataAnnotationTableConfigurationAsync(req, token));

    public Task<CreateBucketMetadataConfigurationResponse> CreateBucketMetadataConfigurationAsync(string bucketName, S3MetadataConfiguration configuration, Action<CreateBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<CreateBucketMetadataConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configuration);
        config?.Invoke(req);
    }, req => operations.CreateBucketMetadataConfigurationAsync(req, token));

    public Task<DeleteBucketMetadataConfigurationResponse> DeleteBucketMetadataConfigurationAsync(string bucketName, Action<DeleteBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteBucketMetadataConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.DeleteBucketMetadataConfigurationAsync(req, token));

    public Task<GetBucketMetadataConfigurationResponse> GetBucketMetadataConfigurationAsync(string bucketName, Action<GetBucketMetadataConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketMetadataConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName);
        config?.Invoke(req);
    }, req => operations.GetBucketMetadataConfigurationAsync(req, token));

    public Task<UpdateBucketMetadataInventoryTableConfigurationResponse> UpdateBucketMetadataInventoryTableConfigurationAsync(string bucketName, S3InventoryTableConfiguration configuration, Action<UpdateBucketMetadataInventoryTableConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<UpdateBucketMetadataInventoryTableConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configuration);
        config?.Invoke(req);
    }, req => operations.UpdateBucketMetadataInventoryTableConfigurationAsync(req, token));

    public Task<UpdateBucketMetadataJournalTableConfigurationResponse> UpdateBucketMetadataJournalTableConfigurationAsync(string bucketName, S3JournalTableConfiguration configuration, Action<UpdateBucketMetadataJournalTableConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<UpdateBucketMetadataJournalTableConfigurationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, configuration);
        config?.Invoke(req);
    }, req => operations.UpdateBucketMetadataJournalTableConfigurationAsync(req, token));

    public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketTaggingRequest req)
        {
            req.Initialize(bucketName, tags);
            config?.Invoke(req);
        }

        Task<PutBucketTaggingResponse> ActionAsync(PutBucketTaggingRequest request) => operations.PutBucketTaggingAsync(request, token);
    }

    public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketTaggingRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketTaggingRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketTaggingResponse> ActionAsync(DeleteBucketTaggingRequest request) => operations.DeleteBucketTaggingAsync(request, token);
    }

    public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketAccelerateConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketAccelerateConfigurationRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketAccelerateConfigurationResponse> ActionAsync(PutBucketAccelerateConfigurationRequest request) => operations.PutBucketAccelerateConfigurationAsync(request, token);
    }

    public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketAccelerateConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketAccelerateConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketAccelerateConfigurationResponse> ActionAsync(GetBucketAccelerateConfigurationRequest request) => operations.GetBucketAccelerateConfigurationAsync(request, token);
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

        Task<PutBucketLifecycleConfigurationResponse> ActionAsync(PutBucketLifecycleConfigurationRequest request) => operations.PutBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketLifecycleRequest>.Shared.RentAndUseAsync(Setup, request => operations.DeleteBucketLifecycleAsync(request, token));

        void Setup(DeleteBucketLifecycleRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketEncryptionRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketEncryptionRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, rules);
            config?.Invoke(req);
        }

        Task<PutBucketEncryptionResponse> ActionAsync(PutBucketEncryptionRequest request) => operations.PutBucketEncryptionAsync(request, token);
    }

    public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketEncryptionRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetBucketEncryptionAsync(request, token));

        void Setup(GetBucketEncryptionRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketEncryptionRequest>.Shared.RentAndUseAsync(Setup, request => operations.DeleteBucketEncryptionAsync(request, token));

        void Setup(DeleteBucketEncryptionRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketVersioningRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketVersioningRequest req)
        {
            req.Initialize(bucketName, enabled);
            config?.Invoke(req);
        }

        Task<PutBucketVersioningResponse> ActionAsync(PutBucketVersioningRequest request) => operations.PutBucketVersioningAsync(request, token);
    }

    public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketVersioningRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketVersioningRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketVersioningResponse> ActionAsync(GetBucketVersioningRequest request) => operations.GetBucketVersioningAsync(request, token);
    }

    public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketLifecycleConfigurationRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketLifecycleConfigurationRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketLifecycleConfigurationResponse> ActionAsync(GetBucketLifecycleConfigurationRequest request) => operations.GetBucketLifecycleConfigurationAsync(request, token);
    }

    public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutPublicAccessBlockRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutPublicAccessBlockRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<PutPublicAccessBlockResponse> ActionAsync(PutPublicAccessBlockRequest request) => operations.PutPublicAccessBlockAsync(request, token);
    }

    public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetPublicAccessBlockRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetPublicAccessBlockAsync(request, token));

        void Setup(GetPublicAccessBlockRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeletePublicAccessBlockRequest>.Shared.RentAndUseAsync(Setup, request => operations.DeletePublicAccessBlockAsync(request, token));

        void Setup(DeletePublicAccessBlockRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(GetBucketPolicyRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<GetBucketPolicyResponse> ActionAsync(GetBucketPolicyRequest request) => operations.GetBucketPolicyAsync(request, token);
    }

    public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteBucketPolicyRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }

        Task<DeleteBucketPolicyResponse> ActionAsync(DeleteBucketPolicyRequest request) => operations.DeleteBucketPolicyAsync(request, token);
    }

    public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutBucketPolicyRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(PutBucketPolicyRequest req)
        {
            req.Initialize(bucketName, policy);
            config?.Invoke(req);
        }

        Task<PutBucketPolicyResponse> ActionAsync(PutBucketPolicyRequest request) => operations.PutBucketPolicyAsync(request, token);
    }

    public Task<GetBucketAbacResponse> GetBucketAbacAsync(string bucketName, Action<GetBucketAbacRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketAbacRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName);
        config?.Invoke(request);
    }, request => operations.GetBucketAbacAsync(request, token));

    public Task<PutBucketAbacResponse> PutBucketAbacAsync(string bucketName, bool enabled, Action<PutBucketAbacRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketAbacRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, enabled);
        config?.Invoke(request);
    }, request => operations.PutBucketAbacAsync(request, token));

    public Task<GetBucketAnalyticsConfigurationResponse> GetBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketAnalyticsConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId);
        config?.Invoke(request);
    }, request => operations.GetBucketAnalyticsConfigurationAsync(request, token));

    public Task<PutBucketAnalyticsConfigurationResponse> PutBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, S3AnalyticsConfiguration configuration, Action<PutBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketAnalyticsConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId, configuration);
        config?.Invoke(request);
    }, request => operations.PutBucketAnalyticsConfigurationAsync(request, token));

    public Task<ListBucketAnalyticsConfigurationsResponse> ListBucketAnalyticsConfigurationsAsync(string bucketName, Action<ListBucketAnalyticsConfigurationsRequest>? config = null, CancellationToken token = default) => ObjectPool<ListBucketAnalyticsConfigurationsRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName);
        config?.Invoke(request);
    }, request => operations.ListBucketAnalyticsConfigurationsAsync(request, token));

    public Task<GetBucketInventoryConfigurationResponse> GetBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<GetBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketInventoryConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId);
        config?.Invoke(request);
    }, request => operations.GetBucketInventoryConfigurationAsync(request, token));

    public Task<PutBucketInventoryConfigurationResponse> PutBucketInventoryConfigurationAsync(string bucketName, string configurationId, S3InventoryConfiguration configuration, Action<PutBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketInventoryConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId, configuration);
        config?.Invoke(request);
    }, request => operations.PutBucketInventoryConfigurationAsync(request, token));

    public Task<ListBucketInventoryConfigurationsResponse> ListBucketInventoryConfigurationsAsync(string bucketName, Action<ListBucketInventoryConfigurationsRequest>? config = null, CancellationToken token = default) => ObjectPool<ListBucketInventoryConfigurationsRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName);
        config?.Invoke(request);
    }, request => operations.ListBucketInventoryConfigurationsAsync(request, token));

    public Task<GetBucketMetricsConfigurationResponse> GetBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<GetBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketMetricsConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId);
        config?.Invoke(request);
    }, request => operations.GetBucketMetricsConfigurationAsync(request, token));

    public Task<PutBucketMetricsConfigurationResponse> PutBucketMetricsConfigurationAsync(string bucketName, string configurationId, S3MetricsConfiguration configuration, Action<PutBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketMetricsConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId, configuration);
        config?.Invoke(request);
    }, request => operations.PutBucketMetricsConfigurationAsync(request, token));

    public Task<ListBucketMetricsConfigurationsResponse> ListBucketMetricsConfigurationsAsync(string bucketName, Action<ListBucketMetricsConfigurationsRequest>? config = null, CancellationToken token = default) => ObjectPool<ListBucketMetricsConfigurationsRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName);
        config?.Invoke(request);
    }, request => operations.ListBucketMetricsConfigurationsAsync(request, token));

    public Task<GetBucketIntelligentTieringConfigurationResponse> GetBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<GetBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetBucketIntelligentTieringConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId);
        config?.Invoke(request);
    }, request => operations.GetBucketIntelligentTieringConfigurationAsync(request, token));

    public Task<PutBucketIntelligentTieringConfigurationResponse> PutBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration, Action<PutBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => ObjectPool<PutBucketIntelligentTieringConfigurationRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName, configurationId, configuration);
        config?.Invoke(request);
    }, request => operations.PutBucketIntelligentTieringConfigurationAsync(request, token));

    public Task<ListBucketIntelligentTieringConfigurationsResponse> ListBucketIntelligentTieringConfigurationsAsync(string bucketName, Action<ListBucketIntelligentTieringConfigurationsRequest>? config = null, CancellationToken token = default) => ObjectPool<ListBucketIntelligentTieringConfigurationsRequest>.Shared.RentAndUseAsync(request =>
    {
        request.Initialize(bucketName);
        config?.Invoke(request);
    }, request => operations.ListBucketIntelligentTieringConfigurationsAsync(request, token));
}