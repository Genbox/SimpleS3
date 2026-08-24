using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketWebsiteResponse</c> API.</summary>
public sealed class GetBucketWebsiteResponse : BaseResponse
{
    /// <summary>Provides the <c>WebsiteConfiguration</c> API.</summary>
    public S3WebsiteConfiguration WebsiteConfiguration { get; internal set; } = new S3WebsiteConfiguration();
}