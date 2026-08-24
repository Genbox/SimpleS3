using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>HeadersNotSignedError</c> API.</summary>
[PublicAPI]
public class HeadersNotSignedError : GenericError
{
    internal HeadersNotSignedError(IDictionary<string, string> lookup) : base(lookup)
    {
        HeadersNotSigned = lookup.GetOptionalValue("HeadersNotSigned");
    }

    /// <summary>Provides the <c>HeadersNotSigned</c> API.</summary>
    public string? HeadersNotSigned { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => "Headers not signed: " + HeadersNotSigned;
}