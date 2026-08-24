using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>RestoreObjectResponse</c> API.</summary>
public class RestoreObjectResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>RestoreOutputPath</c> API.</summary>
    public string? RestoreOutputPath { get; internal set; }
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}