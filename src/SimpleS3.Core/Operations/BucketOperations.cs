using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Operations
{
    [PublicAPI]
    public class BucketOperations : IBucketOperations
    {
        private readonly IRequestHandler _requestHandler;

        public BucketOperations(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default)
        {
            ListBucketsRequest req = new ListBucketsRequest();
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<ListBucketsRequest, ListBucketsResponse>(req, token);
        }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest> config = null, CancellationToken token = default)
        {
            CreateBucketRequest req = new CreateBucketRequest(bucketName);
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<CreateBucketRequest, CreateBucketResponse>(req, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
        {
            DeleteBucketRequest req = new DeleteBucketRequest(bucketName);
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<DeleteBucketRequest, DeleteBucketResponse>(req, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default)
        {
            ListMultipartUploadsRequest req = new ListMultipartUploadsRequest(bucketName);
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<ListMultipartUploadsRequest, ListMultipartUploadsResponse>(req, token);
        }
    }
}