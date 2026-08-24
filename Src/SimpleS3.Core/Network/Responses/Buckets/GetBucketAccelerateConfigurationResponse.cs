namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>GetBucketAccelerateConfiguration</summary>
public class GetBucketAccelerateConfigurationResponse : BaseResponse
{
    /// <summary>Provides the <c>AccelerateEnabled</c> API.</summary>
    public bool AccelerateEnabled { get; internal set; }
}