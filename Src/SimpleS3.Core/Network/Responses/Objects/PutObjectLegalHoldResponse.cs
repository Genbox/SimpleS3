using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>PutObjectLegalHoldResponse</c> API.</summary>
public class PutObjectLegalHoldResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}