using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    [PublicAPI]
    public interface IBucketOperations
    {
        /// <summary>Creates a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CreateBucket.html for details</summary>
        Task<CreateBucketResponse> CreateBucketAsync(CreateBucketRequest request, CancellationToken token = default);

        /// <summary>Delete a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteBucket.html for details</summary>
        Task<DeleteBucketResponse> DeleteBucketAsync(DeleteBucketRequest request, CancellationToken token = default);

        /// <summary>
        /// List all buckets owned by the authenticated sender of the request See
        /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListBuckets.html for details
        /// </summary>
        Task<ListBucketsResponse> ListBucketsAsync(ListBucketsRequest request, CancellationToken token = default);
    }
}