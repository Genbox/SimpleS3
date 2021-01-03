using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3BucketClient : IBucketClient
    {
        public S3BucketClient(IBucketOperations bucketOperations)
        {
            BucketOperations = bucketOperations;
        }

        public IBucketOperations BucketOperations { get; }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
        {
            CreateBucketRequest request = new CreateBucketRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.CreateBucketAsync(request, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
        {
            DeleteBucketRequest request = new DeleteBucketRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.DeleteBucketAsync(request, token);
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
        {
            ListBucketsRequest request = new ListBucketsRequest();
            config?.Invoke(request);

            return BucketOperations.ListBucketsAsync(request, token);
        }

        public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
        {
            HeadBucketRequest request = new HeadBucketRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.HeadBucketAsync(request, token);
        }

        public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            PutBucketLockConfigurationRequest request = new PutBucketLockConfigurationRequest(bucketName, enabled);
            config?.Invoke(request);

            return BucketOperations.PutBucketLockConfigurationAsync(request, token);
        }

        public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            GetBucketLockConfigurationRequest request = new GetBucketLockConfigurationRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.GetBucketLockConfigurationAsync(request, token);
        }

        public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            GetBucketTaggingRequest request = new GetBucketTaggingRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.GetBucketTaggingAsync(request, token);
        }

        public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            PutBucketTaggingRequest request = new PutBucketTaggingRequest(bucketName, tags);
            config?.Invoke(request);

            return BucketOperations.PutBucketTaggingAsync(request, token);
        }

        public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            DeleteBucketTaggingRequest request = new DeleteBucketTaggingRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.DeleteBucketTaggingAsync(request, token);
        }

        public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            PutBucketAccelerateConfigurationRequest request = new PutBucketAccelerateConfigurationRequest(bucketName, enabled);
            config?.Invoke(request);

            return BucketOperations.PutBucketAccelerateConfigurationAsync(request, token);
        }

        public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            GetBucketAccelerateConfigurationRequest request = new GetBucketAccelerateConfigurationRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.GetBucketAccelerateConfigurationAsync(request, token);
        }

        public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
        {
            PutBucketLifecycleConfigurationRequest request = new PutBucketLifecycleConfigurationRequest(bucketName, rules);
            config?.Invoke(request);

            return BucketOperations.PutBucketLifecycleConfigurationAsync(request, token);
        }

        public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
        {
            PutBucketVersioningRequest request = new PutBucketVersioningRequest(bucketName, enabled);
            config?.Invoke(request);

            return BucketOperations.PutBucketVersioningAsync(request, token);
        }

        public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
        {
            GetBucketVersioningRequest request = new GetBucketVersioningRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.GetBucketVersioningAsync(request, token);
        }
    }
}