using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketOwnershipControlsRequest</c> API.</summary>
public sealed class PutBucketOwnershipControlsRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner, IContentMd5Config
{
    internal PutBucketOwnershipControlsRequest() : base(HttpMethodType.PUT) {}
    /// <summary>Provides the <c>PutBucketOwnershipControlsRequest</c> API.</summary>
    public PutBucketOwnershipControlsRequest(string bucketName, IEnumerable<S3OwnershipControlsRule> rules) : this() => Initialize(bucketName, rules);
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    /// <summary>Provides the <c>ExpectedBucketOwner</c> API.</summary>
    public string? ExpectedBucketOwner { get; set; }
    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IEnumerable<S3OwnershipControlsRule> Rules { get; private set; } = Array.Empty<S3OwnershipControlsRule>();
    internal void Initialize(string bucketName, IEnumerable<S3OwnershipControlsRule> rules) { BucketName = bucketName; Rules = rules; }
    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset() { ContentMd5 = null; ExpectedBucketOwner = null; Rules = Array.Empty<S3OwnershipControlsRule>(); base.Reset(); }
}