using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IBucketClient
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
        /// <param name="enabled">A boolean indicating if locking is enabled or not</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest> config = null, CancellationToken token = default);

        /// <summary>Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in the specified bucket.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest> config = null, CancellationToken token = default);

        /// <summary>Get the tags associated with the bucket.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest> config = null, CancellationToken token = default);

        /// <summary>Set tags associated with the bucket. This can be used to track bucket usage on your invoices.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="tags">The tags</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest> config = null, CancellationToken token = default);

        /// <summary>Delete tags associated with a bucket</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest> config = null, CancellationToken token = default);

        /// <summary>Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to perform faster data transfers to Amazon S3.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="enabled">Set to true to enable acceleration. Set to false to disable it.</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest> config = null, CancellationToken token = default);

        /// <summary>Returns the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to perform faster data transfers to and from Amazon S3.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest> config = null, CancellationToken token = default);

        /// <summary>Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration.</summary>
        /// <param name="bucketName">The bucket name</param>
        /// <param name="rules">A list of rules you wish to use</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token </param>
        Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest> config = null, CancellationToken token = default);
    }
}