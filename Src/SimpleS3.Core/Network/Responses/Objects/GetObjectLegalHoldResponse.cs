using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectLegalHoldResponse</c> API.</summary>
public class GetObjectLegalHoldResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>LegalHold</c> API.</summary>
    public bool LegalHold { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}