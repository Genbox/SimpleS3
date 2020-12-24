using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Pools
{
    internal class PooledBucketClient : IBucketClient
    {
        public PooledBucketClient(IBucketOperations bucketOperations)
        {
            BucketOperations = bucketOperations;
        }

        public IBucketOperations BucketOperations { get; }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
        {
            void Setup(CreateBucketRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<CreateBucketResponse> Action(CreateBucketRequest request) => BucketOperations.CreateBucketAsync(request, token);

            return ObjectPool<CreateBucketRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
        {
            void Setup(DeleteBucketRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<DeleteBucketResponse> Action(DeleteBucketRequest request) => BucketOperations.DeleteBucketAsync(request, token);

            return ObjectPool<DeleteBucketRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
        {
            void Setup(ListBucketsRequest req) => config?.Invoke(req);

            Task<ListBucketsResponse> Action(ListBucketsRequest request) => BucketOperations.ListBucketsAsync(request, token);

            return ObjectPool<ListBucketsRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
        {
            void Setup(HeadBucketRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<HeadBucketResponse> Action(HeadBucketRequest request) => BucketOperations.HeadBucketAsync(request, token);

            return ObjectPool<HeadBucketRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutBucketLockConfigurationRequest req)
            {
                req.Initialize(bucketName, enabled);
                config?.Invoke(req);
            }

            Task<PutBucketLockConfigurationResponse> Action(PutBucketLockConfigurationRequest request) => BucketOperations.PutBucketLockConfigurationAsync(request, token);

            return ObjectPool<PutBucketLockConfigurationRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetBucketLockConfigurationRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<GetBucketLockConfigurationResponse> Action(GetBucketLockConfigurationRequest request) => BucketOperations.GetBucketLockConfigurationAsync(request, token);

            return ObjectPool<GetBucketLockConfigurationRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetBucketTaggingRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<GetBucketTaggingResponse> Action(GetBucketTaggingRequest request) => BucketOperations.GetBucketTaggingAsync(request, token);

            return ObjectPool<GetBucketTaggingRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutBucketTaggingRequest req)
            {
                req.Initialize(bucketName, tags);
                config?.Invoke(req);
            }

            Task<PutBucketTaggingResponse> Action(PutBucketTaggingRequest request) => BucketOperations.PutBucketTaggingAsync(request, token);

            return ObjectPool<PutBucketTaggingRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            void Setup(DeleteBucketTaggingRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<DeleteBucketTaggingResponse> Action(DeleteBucketTaggingRequest request) => BucketOperations.DeleteBucketTaggingAsync(request, token);

            return ObjectPool<DeleteBucketTaggingRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutBucketAccelerateConfigurationRequest req)
            {
                req.Initialize(bucketName, enabled);
                config?.Invoke(req);
            }

            Task<PutBucketAccelerateConfigurationResponse> Action(PutBucketAccelerateConfigurationRequest request) => BucketOperations.PutBucketAccelerateConfigurationAsync(request, token);

            return ObjectPool<PutBucketAccelerateConfigurationRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            void Setup(GetBucketAccelerateConfigurationRequest req)
            {
                req.Initialize(bucketName);
                config?.Invoke(req);
            }

            Task<GetBucketAccelerateConfigurationResponse> Action(GetBucketAccelerateConfigurationRequest request) => BucketOperations.GetBucketAccelerateConfigurationAsync(request, token);

            return ObjectPool<GetBucketAccelerateConfigurationRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
        {
            void Setup(PutBucketLifecycleConfigurationRequest req)
            {
                req.Initialize(bucketName, rules);
                config?.Invoke(req);
            }

            Task<PutBucketLifecycleConfigurationResponse> Action(PutBucketLifecycleConfigurationRequest request) => BucketOperations.PutBucketLifecycleConfigurationAsync(request, token);

            return ObjectPool<PutBucketLifecycleConfigurationRequest>.Shared.RentAndUse(Setup, Action);
        }
    }
}