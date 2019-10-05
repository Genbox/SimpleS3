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

        public Task<GetBucketResponse> GetAsync(string bucketName, Action<GetBucketRequest> config = null, CancellationToken token = default)
        {
            GetBucketRequest req = new GetBucketRequest(bucketName);
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<GetBucketRequest, GetBucketResponse>(req, token);
        }

        public Task<PutBucketResponse> PutAsync(string bucketName, Action<PutBucketRequest> config = null, CancellationToken token = default)
        {
            PutBucketRequest req = new PutBucketRequest(bucketName);
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<PutBucketRequest, PutBucketResponse>(req, token);
        }

        public Task<DeleteBucketResponse> DeleteAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
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