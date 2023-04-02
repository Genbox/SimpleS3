using System.Globalization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class TooManyBucketsError : GenericError
{
    internal TooManyBucketsError(IDictionary<string, string> lookup) : base(lookup)
    {
        if (lookup.TryGetValue("CurrentNumberOfBuckets", out string? current))
            CurrentNumberOfBuckets = int.Parse(current, NumberFormatInfo.InvariantInfo);

        if (lookup.TryGetValue("AllowedNumberOfBuckets", out string? allowed))
            AllowedNumberOfBuckets = int.Parse(allowed, NumberFormatInfo.InvariantInfo);
    }

    public int CurrentNumberOfBuckets { get; }
    public int AllowedNumberOfBuckets { get; }

    public override string GetErrorDetails() => $"CurrentNumberOfBuckets: {CurrentNumberOfBuckets} - AllowedNumberOfBuckets: {AllowedNumberOfBuckets}";
}