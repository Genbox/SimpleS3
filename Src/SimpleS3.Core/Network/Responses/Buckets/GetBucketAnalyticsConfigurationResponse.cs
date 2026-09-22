using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing an analytics configuration.</summary>
public sealed class GetBucketAnalyticsConfigurationResponse : BaseResponse
{
    /// <summary>Analytics configuration.</summary>
    public S3AnalyticsConfiguration Configuration { get; internal set; } = null!;
}