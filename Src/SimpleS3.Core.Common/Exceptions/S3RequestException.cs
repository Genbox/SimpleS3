using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Common.Exceptions;

public class S3RequestException : S3Exception
{
    public S3RequestException(IResponse response, string? message = null, Exception? innerException = null) : base(message, innerException)
    {
        Response = response;
    }

    public IResponse Response { get; }
}