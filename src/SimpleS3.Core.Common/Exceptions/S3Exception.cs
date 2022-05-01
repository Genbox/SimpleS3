namespace Genbox.SimpleS3.Core.Common.Exceptions;

public class S3Exception : Exception
{
    public S3Exception(string? message = null, Exception? innException = null) : base(message, innException) {}
}