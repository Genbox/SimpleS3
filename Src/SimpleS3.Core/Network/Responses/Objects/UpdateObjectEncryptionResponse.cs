using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Response from UpdateObjectEncryption.</summary>
public sealed class UpdateObjectEncryptionResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Whether the requester was charged.</summary>
    public bool RequestCharged { get; internal set; }
}