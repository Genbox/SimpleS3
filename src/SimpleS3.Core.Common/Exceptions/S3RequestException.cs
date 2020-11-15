using System;

namespace Genbox.SimpleS3.Core.Common.Exceptions
{
    public class S3RequestException : S3Exception
    {
        public S3RequestException(int statusCode, string? message = null, Exception? innException = null) : base(message, innException)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}