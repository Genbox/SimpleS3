using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class InvalidArgumentError : GenericError
{
    internal InvalidArgumentError(IDictionary<string, string> lookup) : base(lookup)
    {
        ArgumentName = lookup.GetOptionalValue("ArgumentName");
        ArgumentValue = lookup.GetOptionalValue("ArgumentValue");
    }

    public string? ArgumentName { get; }
    public string? ArgumentValue { get; }

    public override string GetErrorDetails() => $"Argument: {ArgumentName} - Value: {ArgumentValue}";
}