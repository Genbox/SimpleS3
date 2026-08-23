using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class PutBucketOwnershipControlsRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner, IContentMd5Config
{
    internal PutBucketOwnershipControlsRequest() : base(HttpMethodType.PUT) {}
    public PutBucketOwnershipControlsRequest(string bucketName, IEnumerable<S3OwnershipControlsRule> rules) : this() => Initialize(bucketName, rules);
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string? ExpectedBucketOwner { get; set; }
    public IEnumerable<S3OwnershipControlsRule> Rules { get; private set; } = Array.Empty<S3OwnershipControlsRule>();
    internal void Initialize(string bucketName, IEnumerable<S3OwnershipControlsRule> rules) { BucketName = bucketName; Rules = rules; }
    public override void Reset() { ContentMd5 = null; ExpectedBucketOwner = null; Rules = Array.Empty<S3OwnershipControlsRule>(); base.Reset(); }
}