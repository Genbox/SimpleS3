using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing a bucket metrics configuration.</summary>
public sealed class GetBucketMetricsConfigurationResponse : BaseResponse
{
    /// <summary>Metrics configuration.</summary>
    public S3MetricsConfiguration Configuration { get; internal set; } = null!;
}