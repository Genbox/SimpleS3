using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketCorsResponse</c> API.</summary>
public sealed class GetBucketCorsResponse : BaseResponse
{
    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3CorsRule> Rules { get; } = new List<S3CorsRule>();
}