using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IS3BucketClient
    {
        IBucketOperations BucketOperations { get; }

        /// <summary>List all objects within a bucket</summary>
        /// <param name="bucketName"></param>
        /// <param name="config"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default);

        /// <summary>Creates a bucket</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest> config = null, CancellationToken token = default);

        /// <summary>
        /// Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can
        /// be deleted.
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default);

        /// <summary>List all multipart uploads within a bucket</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        /// <returns></returns>
        Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default);

        /// <summary>Remove everything within a bucket, except for the bucket itself.</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="token">A cancellation token </param>
        Task<DeleteBucketStatus> EmptyBucketAsync(string bucketName, CancellationToken token = default);

        /// <summary>List all buckets you own</summary>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default);
    }
}