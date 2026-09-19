using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Common.Exceptions;

/// <summary>Provides the <c>S3RequestException</c> API.</summary>
[SuppressMessage("Design", "CA1032", Justification = "This exception requires the failed response so callers can inspect its S3 error details.")]
public class S3RequestException(IResponse response, string? message = null, Exception? innerException = null) : S3Exception(message, innerException)
{
    /// <summary>Provides the <c>Response</c> API.</summary>
    public IResponse Response { get; } = response;
}