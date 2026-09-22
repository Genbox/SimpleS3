using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing an inventory configuration.</summary>
public sealed class GetBucketInventoryConfigurationResponse : BaseResponse
{
    /// <summary>Inventory configuration.</summary>
    public S3InventoryConfiguration Configuration { get; internal set; } = null!;
}