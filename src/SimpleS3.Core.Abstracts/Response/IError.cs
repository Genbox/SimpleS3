using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Response
{
    public interface IError
    {
        /// <summary>Contains the error code returned from AWS S3</summary>
        ErrorCode Code { get; }

        /// <summary>Contains the message returned from AWS S3</summary>
        string Message { get; }

        /// <summary>Additional info that was returned from the error</summary>
        IDictionary<string, string> Data { get; }

        /// <summary>Returns a string with the most important fields. This only works if the error is a specialized subtype of the GenericError class</summary>
        string GetErrorDetails();
    }
}