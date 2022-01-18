using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class HeadersNotSignedError : GenericError
{
    internal HeadersNotSignedError(IDictionary<string, string> lookup) : base(lookup)
    {
        HeadersNotSigned = lookup.GetOptionalValue("HeadersNotSigned");
    }

    public string? HeadersNotSigned { get; }

    public override string GetErrorDetails()
    {
        return "Headers not signed: " + HeadersNotSigned;
    }
}