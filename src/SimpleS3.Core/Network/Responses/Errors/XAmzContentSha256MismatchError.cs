using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

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

    public string? ClientSha256 { get; }
    public string? S3Sha256 { get; }
    public string? RequestId { get; }
    public string? HostId { get; }

    public override string GetErrorDetails() => $"Client SHA256: {ClientSha256} S3Sha256: {S3Sha256}";
}