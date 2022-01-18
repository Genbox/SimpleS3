using System.Collections.Generic;
using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class MethodNotAllowedError : GenericError
{
    internal MethodNotAllowedError(IDictionary<string, string> lookup) : base(lookup)
    {
        Method = lookup.GetOptionalValue("Method");
        ResourceType = lookup.GetOptionalValue("ResourceType");
        RequestId = lookup.GetOptionalValue("RequestId");
        HostId = lookup.GetOptionalValue("HostId");
    }

    public string? Method { get; }
    public string? ResourceType { get; }
    public string? RequestId { get; }
    public string? HostId { get; }

    public override string GetErrorDetails()
    {
        return $"Method: {Method} ResourceType: {ResourceType}";
    }
}