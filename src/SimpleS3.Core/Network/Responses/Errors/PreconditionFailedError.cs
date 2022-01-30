using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class PreconditionFailedError : GenericError
{
    internal PreconditionFailedError(IDictionary<string, string> lookup) : base(lookup)
    {
        //The condition field is optional since only Amazon S3 returns it
        Condition = lookup.GetOptionalValue("Condition");
    }

    public string? Condition { get; }

    public override string GetErrorDetails() => "Condition: " + Condition;
}