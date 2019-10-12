using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Service;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    [PublicAPI]
    public interface IBucketOperations
    {
        Task<GetBucketResponse> GetAsync(string bucketName, Action<GetBucketRequest> config = null, CancellationToken token = default);
        Task<PutBucketResponse> PutAsync(string bucketName, Action<PutBucketRequest> config = null, CancellationToken token = default);
        Task<DeleteBucketResponse> DeleteAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default);
        Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default);
        Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default);
    }
}