using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectRetentionResponse</c> API.</summary>
public sealed class GetObjectRetentionResponse : BaseResponse, IHasLock, IHasRequestCharged
{
    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; internal set; }
    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; internal set; }
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}