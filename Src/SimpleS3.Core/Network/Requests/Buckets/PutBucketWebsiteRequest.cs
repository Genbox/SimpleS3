using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketWebsiteRequest</c> API.</summary>
public sealed class PutBucketWebsiteRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketWebsiteRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketWebsiteRequest</c> API.</summary>
    public PutBucketWebsiteRequest(string bucketName, S3WebsiteConfiguration websiteConfiguration) : this()
    {
        Initialize(bucketName, websiteConfiguration);
    }

    /// <summary>Provides the <c>WebsiteConfiguration</c> API.</summary>
    public S3WebsiteConfiguration WebsiteConfiguration { get; private set; } = null!;

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, S3WebsiteConfiguration websiteConfiguration)
    {
        BucketName = bucketName;
        WebsiteConfiguration = websiteConfiguration;
    }

    internal override void Reset()
    {
        ContentMd5 = null;
        WebsiteConfiguration = null!;
        BucketName = null!;
        base.Reset();
    }
}