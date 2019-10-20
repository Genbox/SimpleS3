using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    [PublicAPI]
    public interface IBucketOperations
    {
        /// <summary>List objects within a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListObjectsV2.html for details</summary>
        Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default);

        /// <summary>Creates a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CreateBucket.html for details</summary>
        Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest> config = null, CancellationToken token = default);

        /// <summary>Delete a bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteBucket.html for details</summary>
        Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default);

        /// <summary>List in-progress multipart uploads See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListMultipartUploads.html for details</summary>
        Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default);

        /// <summary>
        /// List all buckets owned by the authenticated sender of the request See
        /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListBuckets.html for details
        /// </summary>
        Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default);
    }
}