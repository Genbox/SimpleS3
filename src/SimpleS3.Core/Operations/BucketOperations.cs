using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
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

        public Task<CreateBucketResponse> CreateBucketAsync(CreateBucketRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CreateBucketRequest, CreateBucketResponse>(request, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteBucketRequest, DeleteBucketResponse>(request, token);
        }

        public Task<ListBucketsResponse> ListBucketsAsync(ListBucketsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListBucketsRequest, ListBucketsResponse>(request, token);
        }

        public Task<HeadBucketResponse> HeadBucketAsync(HeadBucketRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<HeadBucketRequest, HeadBucketResponse>(request, token);
        }
    }
}