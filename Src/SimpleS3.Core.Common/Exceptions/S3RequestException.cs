using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Common.Exceptions;

public class S3RequestException(IResponse response, string? message = null, Exception? innerException = null) : S3Exception(message, innerException)
{
    public IResponse Response { get; } = response;
}