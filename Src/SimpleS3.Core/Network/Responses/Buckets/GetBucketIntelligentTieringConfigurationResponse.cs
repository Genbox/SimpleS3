using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing an Intelligent-Tiering configuration.</summary>
public sealed class GetBucketIntelligentTieringConfigurationResponse : BaseResponse
{
    /// <summary>Intelligent-Tiering configuration.</summary>
    public S3IntelligentTieringConfiguration Configuration { get; internal set; } = null!;
}