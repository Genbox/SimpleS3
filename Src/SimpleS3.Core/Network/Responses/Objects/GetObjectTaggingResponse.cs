using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectTaggingResponse</c> API.</summary>
public sealed class GetObjectTaggingResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>Tags</c> API.</summary>
    public IDictionary<string, string> Tags { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}