using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>PreconditionFailedError</c> API.</summary>
[PublicAPI]
public class PreconditionFailedError : GenericError
{
    internal PreconditionFailedError(IDictionary<string, string> lookup) : base(lookup)
    {
        //The condition field is optional since only Amazon S3 returns it
        Condition = lookup.GetOptionalValue("Condition");
    }

    /// <summary>Provides the <c>Condition</c> API.</summary>
    public string? Condition { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => "Condition: " + Condition;
}