using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Return the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer
/// Acceleration is a bucket-level feature that enables you to perform faster data transfers to and from Amazon S3. To use
/// this operation, you must have permission to perform the s3:GetAccelerateConfiguration action. The bucket owner has this
/// permission by default. The bucket owner can grant this permission to others.</summary>
public class GetBucketAccelerateConfigurationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketAccelerateConfigurationRequest() : base(HttpMethodType.GET) { }

    public GetBucketAccelerateConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    public string BucketName { get; set; }

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }
}