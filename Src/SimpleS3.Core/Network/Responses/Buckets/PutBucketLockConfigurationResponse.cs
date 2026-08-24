using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>PutBucketLockConfigurationResponse</c> API.</summary>
public class PutBucketLockConfigurationResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}