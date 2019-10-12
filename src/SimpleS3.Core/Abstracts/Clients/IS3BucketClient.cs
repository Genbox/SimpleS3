using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Service;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IS3BucketClient
    {
        Task<GetBucketResponse> GetBucketAsync(string bucketName, Action<GetBucketRequest> config = null, CancellationToken token = default);

        /// <summary>Creates a bucket</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request before sending</param>
        /// <param name="token">A cancellation token</param>
        Task<PutBucketResponse> PutBucketAsync(string bucketName, Action<PutBucketRequest> config = null, CancellationToken token = default);

        /// <summary>
        /// Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can
        /// be deleted.
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request before sending</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default);

        Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default);
        Task<DeleteBucketStatus> EmptyBucketAsync(string bucketName, CancellationToken token = default);
        Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default);
    }
}