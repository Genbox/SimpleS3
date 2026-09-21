using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>GetBucketAccelerateConfiguration</summary>
public class GetBucketAccelerateConfigurationResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>AccelerateEnabled</c> API.</summary>
    public bool AccelerateEnabled { get; internal set; }

    /// <summary>Whether the requester was charged for the request.</summary>
    public bool RequestCharged { get; internal set; }
}