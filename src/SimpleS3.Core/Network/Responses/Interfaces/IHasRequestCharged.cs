namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

public interface IHasRequestCharged
{
    /// <summary>If present, indicates that the requester was successfully charged for the request.</summary>
    bool RequestCharged { get; }
}