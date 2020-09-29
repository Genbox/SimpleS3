using System;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IResponse
    {
        /// <summary>The length in bytes of the body in the response.</summary>
        long ContentLength { get; set; }

        /// <summary>Specifies whether the connection to the server is open or closed.</summary>
        bool ConnectionClosed { get; set; }

        /// <summary>The date and time S3 responded.</summary>
        DateTimeOffset Date { get; set; }

        /// <summary>The name of the server that created the response.</summary>
        string? Server { get; set; }

        /// <summary>A special token that is used together with the <see cref="RequestId" /> to help troubleshoot problems.</summary>
        string? ResponseId { get; set; }

        /// <summary>A value created by S3 that uniquely identifies the request.</summary>
        string? RequestId { get; set; }

        /// <summary>Return true if the request was successfully processed.</summary>
        bool IsSuccess { get; set; }

        int StatusCode { get; set; }

        IError Error { get; set; }
    }
}