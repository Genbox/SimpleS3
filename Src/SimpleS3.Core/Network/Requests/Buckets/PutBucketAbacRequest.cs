using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Sets the attribute-based access control status of a bucket.</summary>
public sealed class PutBucketAbacRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasChecksumProperties
{
    internal PutBucketAbacRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public PutBucketAbacRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    /// <summary>Whether ABAC is enabled.</summary>
    public bool Enabled { get; set; }

    /// <summary>MD5 checksum of the body.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Name of the bucket.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Checksum algorithm.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Checksum value.</summary>
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        Enabled = enabled;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        Enabled = false;
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        BucketName = null!;
        base.Reset();
    }
}