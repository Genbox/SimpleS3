using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>XAmzContentSha256MismatchError</c> API.</summary>
[PublicAPI]
public class XAmzContentSha256MismatchError : GenericError
{
    internal XAmzContentSha256MismatchError(IDictionary<string, string> lookup) : base(lookup)
    {
        ClientSha256 = lookup.GetOptionalValue("ClientComputedContentSHA256");
        S3Sha256 = lookup.GetOptionalValue("S3ComputedContentSHA256");
        RequestId = lookup.GetOptionalValue("RequestId");
        HostId = lookup.GetOptionalValue("HostId");
    }

    /// <summary>Provides the <c>ClientSha256</c> API.</summary>
    public string? ClientSha256 { get; }
    /// <summary>Provides the <c>S3Sha256</c> API.</summary>
    public string? S3Sha256 { get; }
    /// <summary>Provides the <c>RequestId</c> API.</summary>
    public string? RequestId { get; }
    /// <summary>Provides the <c>HostId</c> API.</summary>
    public string? HostId { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => $"Client SHA256: {ClientSha256} S3Sha256: {S3Sha256}";
}