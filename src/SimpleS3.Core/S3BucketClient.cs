using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3BucketClient : IS3BucketClient
    {
        private readonly IS3ObjectClient _objectClient;

        public S3BucketClient(IBucketOperations bucketOperations, IS3ObjectClient objectClient)
        {
            BucketOperations = bucketOperations;
            _objectClient = objectClient;
        }

        public IBucketOperations BucketOperations { get; }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest> config = null, CancellationToken token = default)
        {
            CreateBucketRequest request = new CreateBucketRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.CreateBucketAsync(request, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
        {
            DeleteBucketRequest request = new DeleteBucketRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.DeleteBucketAsync(request, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default)
        {
            ListMultipartUploadsRequest request = new ListMultipartUploadsRequest(bucketName);
            config?.Invoke(request);

            return BucketOperations.ListMultipartUploadsAsync(request, token);
        }

        public async Task<EmptyBucketStatus> EmptyBucketAsync(string bucketName, CancellationToken token = default)
        {
            string continuationToken = null;
            ListObjectsResponse response;

            do
            {
                if (token.IsCancellationRequested)
                    break;

                string cToken = continuationToken;
                response = await _objectClient.ListObjectsAsync(bucketName, req => req.ContinuationToken = cToken, token).ConfigureAwait(false);

                if (!response.IsSuccess)
                    return EmptyBucketStatus.RequestFailed;

                DeleteObjectsResponse multiDelResponse = await _objectClient.DeleteObjectsAsync(bucketName, response.Objects.Select(x => x.ObjectKey), true, token: token).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    return EmptyBucketStatus.RequestFailed;

                continuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return EmptyBucketStatus.Ok;
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default)
        {
            ListBucketsRequest request = new ListBucketsRequest();
            config?.Invoke(request);

            return BucketOperations.ListBucketsAsync(request, token);
        }
    }
}