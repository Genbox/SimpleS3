using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketCorsRequest</c> API.</summary>
public sealed class PutBucketCorsRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasChecksumProperties
{
    internal PutBucketCorsRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketCorsRequest</c> API.</summary>
    public PutBucketCorsRequest(string bucketName, IEnumerable<S3CorsRule> rules) : this()
    {
        Initialize(bucketName, rules);
    }

    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IEnumerable<S3CorsRule> Rules { get; private set; } = Array.Empty<S3CorsRule>();

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Checksum algorithm used for the request body.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Optional precalculated checksum of the request body.</summary>
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, IEnumerable<S3CorsRule> rules)
    {
        BucketName = bucketName;
        Rules = rules;
    }

    /// <summary>Resets the request for reuse.</summary>
    public override void Reset()
    {
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        Rules = [];
        BucketName = null!;
        base.Reset();
    }
}