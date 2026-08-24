using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>InvalidBucketNameError</c> API.</summary>
[PublicAPI]
public class InvalidBucketNameError : GenericError
{
    internal InvalidBucketNameError(IDictionary<string, string> lookup) : base(lookup)
    {
        BucketName = lookup.GetOptionalValue("BucketName");
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string? BucketName { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => "BucketName: " + BucketName;
}