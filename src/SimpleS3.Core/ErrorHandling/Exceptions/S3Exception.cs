using System;

namespace Genbox.SimpleS3.Core.ErrorHandling.Exceptions
{
    public class S3Exception : Exception
    {
        public S3Exception(string? message = null, Exception? innException = null) : base(message, innException) { }
    }
}