using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketLockConfigurationResponse</c> API.</summary>
public class GetBucketLockConfigurationResponse : BaseResponse, IHasLock
{
    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; internal set; }
    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; internal set; }
}