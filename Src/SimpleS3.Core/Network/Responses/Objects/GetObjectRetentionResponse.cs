using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class GetObjectRetentionResponse : BaseResponse, IHasLock, IHasRequestCharged
{
    public LockMode LockMode { get; internal set; }
    public DateTimeOffset? LockRetainUntil { get; internal set; }
    public bool RequestCharged { get; internal set; }
}