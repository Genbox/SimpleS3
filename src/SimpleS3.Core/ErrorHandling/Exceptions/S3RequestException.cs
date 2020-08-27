using System;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.ErrorHandling.Exceptions
{
    [PublicAPI]
    public class S3RequestException : S3Exception
    {
        public S3RequestException(int statusCode, string? message = null, Exception? innException = null) : base(message, innException)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}