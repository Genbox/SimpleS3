using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class PutBucketCorsRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner, IContentMd5Config
{
    internal PutBucketCorsRequest() : base(HttpMethodType.PUT) {}
    public PutBucketCorsRequest(string bucketName, IEnumerable<S3CorsRule> rules) : this() => Initialize(bucketName, rules);
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string? ExpectedBucketOwner { get; set; }
    public IEnumerable<S3CorsRule> Rules { get; private set; } = Array.Empty<S3CorsRule>();
    internal void Initialize(string bucketName, IEnumerable<S3CorsRule> rules) { BucketName = bucketName; Rules = rules; }
    public override void Reset() { ContentMd5 = null; ExpectedBucketOwner = null; Rules = Array.Empty<S3CorsRule>(); base.Reset(); }
}