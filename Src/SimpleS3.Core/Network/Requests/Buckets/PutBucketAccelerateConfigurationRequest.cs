using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to perform faster data transfers to
/// Amazon S3. To use this operation, you must have permission to perform the s3:PutAccelerateConfiguration action. The bucket owner has this permission by default. The bucket owner
/// can grant this permission to others.
/// </summary>
public class PutBucketAccelerateConfigurationRequest : BaseRequest, IHasBucketName, IHasChecksumProperties
{
    internal PutBucketAccelerateConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketAccelerateConfigurationRequest</c> API.</summary>
    public PutBucketAccelerateConfigurationRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    /// <summary>Provides the <c>AccelerationEnabled</c> API.</summary>
    public bool AccelerationEnabled { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Checksum algorithm for the request body.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Optional precalculated checksum of the request body.</summary>
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        AccelerationEnabled = enabled;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        AccelerationEnabled = false;
        BucketName = null!;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        base.Reset();
    }
}