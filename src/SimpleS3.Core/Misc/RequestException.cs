using System;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Misc
{
    [PublicAPI]
    public class RequestException : Exception
    {
        public RequestException(int statusCode, string message = null, Exception innException = null) : base(message, innException)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}