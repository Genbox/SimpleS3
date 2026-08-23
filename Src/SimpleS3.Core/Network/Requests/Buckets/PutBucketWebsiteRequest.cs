using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class PutBucketWebsiteRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner, IContentMd5Config
{
    internal PutBucketWebsiteRequest() : base(HttpMethodType.PUT) {}
    public PutBucketWebsiteRequest(string bucketName, S3WebsiteConfiguration websiteConfiguration) : this() => Initialize(bucketName, websiteConfiguration);
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string? ExpectedBucketOwner { get; set; }
    public S3WebsiteConfiguration WebsiteConfiguration { get; private set; } = null!;
    internal void Initialize(string bucketName, S3WebsiteConfiguration websiteConfiguration) { BucketName = bucketName; WebsiteConfiguration = websiteConfiguration; }
    public override void Reset() { ContentMd5 = null; ExpectedBucketOwner = null; WebsiteConfiguration = null!; base.Reset(); }
}