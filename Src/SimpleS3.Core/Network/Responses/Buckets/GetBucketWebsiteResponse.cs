using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketWebsiteResponse : BaseResponse
{
    public S3WebsiteConfiguration WebsiteConfiguration { get; internal set; } = new S3WebsiteConfiguration();
}