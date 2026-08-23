using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class GetObjectTaggingResponse : BaseResponse, IHasRequestCharged
{
    public bool RequestCharged { get; internal set; }
    public IDictionary<string, string> Tags { get; } = new Dictionary<string, string>();
}