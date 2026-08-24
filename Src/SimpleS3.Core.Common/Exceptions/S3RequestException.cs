using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Common.Exceptions;

/// <summary>Provides the <c>S3RequestException</c> API.</summary>
public class S3RequestException(IResponse response, string? message = null, Exception? innerException = null) : S3Exception(message, innerException)
{
    /// <summary>Provides the <c>Response</c> API.</summary>
    public IResponse Response { get; } = response;
}