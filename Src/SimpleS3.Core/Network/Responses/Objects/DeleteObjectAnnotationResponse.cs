using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>DeleteObjectAnnotationResponse</c> API.</summary>
public sealed class DeleteObjectAnnotationResponse : BaseResponse, IHasRequestCharged, IHasVersionId
{
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}