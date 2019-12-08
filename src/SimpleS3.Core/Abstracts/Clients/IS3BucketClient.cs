using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IS3BucketClient
    {
        IBucketOperations BucketOperations { get; }

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

        /// <summary>List all buckets you own</summary>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default);

        /// <summary>Use this to determine if a bucket exists and you have permission to access it</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest> config = null, CancellationToken token = default);

        /// <summary>Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in the specified bucket.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, LockMode lockMode, DateTimeOffset lockRemainUntil, Action<PutBucketLockConfigurationRequest> config = null, CancellationToken token = default);

        /// <summary>Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in the specified bucket.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest> config = null, CancellationToken token = default);
    }
}