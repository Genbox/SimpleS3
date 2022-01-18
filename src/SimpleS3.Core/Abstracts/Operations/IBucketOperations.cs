using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

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

    /// <summary>Check to see if a bucket exists. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_HeadBucket.html for details</summary>
    Task<HeadBucketResponse> HeadBucketAsync(HeadBucketRequest request, CancellationToken token = default);

    /// <summary>
    /// Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock configuration will be applied by default
    /// to every new object placed in the specified bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObjectLockConfiguration.html for
    /// details
    /// </summary>
    Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(PutBucketLockConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new
    /// object placed in the specified bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObjectLockConfiguration.html for details
    /// </summary>
    Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(GetBucketLockConfigurationRequest request, CancellationToken token = default);

    /// <summary>Returns the tag set associated with the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketTagging.html for details</summary>
    Task<GetBucketTaggingResponse> GetBucketTaggingAsync(GetBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>Sets the tags for a bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketTagging.html for details.</summary>
    Task<PutBucketTaggingResponse> PutBucketTaggingAsync(PutBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>Deletes the tags from the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_DeleteBucketTagging.html for details</summary>
    Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, CancellationToken token = default);

    /// <summary>
    /// Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to
    /// perform faster data transfers to Amazon S3.
    /// </summary>
    Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Returns the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer Acceleration is a bucket-level
    /// feature that enables you to perform faster data transfers to and from Amazon S3.
    /// </summary>
    Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketLifecycleConfiguration.html for details
    /// </summary>
    Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(PutBucketLifecycleConfigurationRequest request, CancellationToken token = default);

    /// <summary>
    /// Sets the versioning state of an existing bucket. To set the versioning state, you must be the bucket owner. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketVersioning.html for details.
    /// </summary>
    Task<PutBucketVersioningResponse> PutBucketVersioningAsync(PutBucketVersioningRequest request, CancellationToken token = default);

    /// <summary>Returns the versioning state of a bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketVersioning.html for details.</summary>
    Task<GetBucketVersioningResponse> GetBucketVersioningAsync(GetBucketVersioningRequest request, CancellationToken token = default);

    /// <summary>Returns the lifecycle configuration information set on the bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketLifecycleConfiguration.html for details.</summary>
    Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(GetBucketLifecycleConfigurationRequest request, CancellationToken token = default);
}