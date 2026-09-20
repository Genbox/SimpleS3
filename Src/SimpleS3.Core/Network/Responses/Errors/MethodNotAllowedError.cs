using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>MethodNotAllowedError</c> API.</summary>
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

    /// <summary>Provides the <c>Method</c> API.</summary>
    public string? Method { get; }

    /// <summary>Provides the <c>ResourceType</c> API.</summary>
    public string? ResourceType { get; }

    /// <summary>Provides the <c>RequestId</c> API.</summary>
    public string? RequestId { get; }

    /// <summary>Provides the <c>HostId</c> API.</summary>
    public string? HostId { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => $"Method: {Method} ResourceType: {ResourceType}";
}