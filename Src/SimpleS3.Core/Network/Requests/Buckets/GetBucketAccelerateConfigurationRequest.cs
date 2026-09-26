using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Return the Transfer Acceleration state of a bucket, which is either Enabled or Suspended. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to
/// perform faster data transfers to and from Amazon S3. To use this operation, you must have permission to perform the s3:GetAccelerateConfiguration action. The bucket owner has this
/// permission by default. The bucket owner can grant this permission to others.
/// </summary>
public class GetBucketAccelerateConfigurationRequest : BaseRequest, IHasBucketName, IHasRequestPayer
{
    internal GetBucketAccelerateConfigurationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketAccelerateConfigurationRequest</c> API.</summary>
    public GetBucketAccelerateConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Specifies that the requester agrees to pay request and data transfer charges.</summary>
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName) => BucketName = bucketName;
    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        RequestPayer = Payer.Unknown;
        BucketName = null!;
        base.Reset();
    }
}