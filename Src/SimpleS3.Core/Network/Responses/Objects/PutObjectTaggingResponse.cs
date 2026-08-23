using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class PutObjectTaggingResponse : BaseResponse, IHasRequestCharged
{
    public bool RequestCharged { get; internal set; }
    public string? VersionId { get; internal set; }
}