using System.Globalization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>TooManyBucketsError</c> API.</summary>
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

    /// <summary>Provides the <c>CurrentNumberOfBuckets</c> API.</summary>
    public int CurrentNumberOfBuckets { get; }

    /// <summary>Provides the <c>AllowedNumberOfBuckets</c> API.</summary>
    public int AllowedNumberOfBuckets { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public override string GetErrorDetails() => $"CurrentNumberOfBuckets: {CurrentNumberOfBuckets.ToString(NumberFormatInfo.InvariantInfo)} - AllowedNumberOfBuckets: {AllowedNumberOfBuckets.ToString(NumberFormatInfo.InvariantInfo)}";
}