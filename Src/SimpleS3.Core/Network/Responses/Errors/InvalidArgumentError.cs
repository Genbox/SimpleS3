using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>InvalidArgumentError</c> API.</summary>
[PublicAPI]
public class InvalidArgumentError : GenericError
{
    internal InvalidArgumentError(IDictionary<string, string> lookup) : base(lookup)
    {
        ArgumentName = lookup.GetOptionalValue("ArgumentName");
        ArgumentValue = lookup.GetOptionalValue("ArgumentValue");
    }

    /// <summary>Provides the <c>ArgumentName</c> API.</summary>
    public string? ArgumentName { get; }

    /// <summary>Provides the <c>ArgumentValue</c> API.</summary>
    public string? ArgumentValue { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => $"Argument: {ArgumentName} - Value: {ArgumentValue}";
}