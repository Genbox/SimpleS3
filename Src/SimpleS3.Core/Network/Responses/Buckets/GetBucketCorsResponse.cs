using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketCorsResponse : BaseResponse
{
    public IList<S3CorsRule> Rules { get; } = new List<S3CorsRule>();
}