using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>DeleteObjectResponse</c> API.</summary>
public class DeleteObjectResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasDeleteMarker
{
    /// <summary>Provides the <c>IsDeleteMarker</c> API.</summary>
    public bool IsDeleteMarker { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}