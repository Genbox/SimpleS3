using System;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface IResponseContentProperties
    {
        /// <summary>The MIME type of the content. For example, Content-Type: text/html; charset=utf-8</summary>
        string ContentType { get; }

        string ContentDisposition { get; }
        string ContentEncoding { get; }
        string ContentLanguage { get; }

        /// <summary>The date and time at which the object is no longer able to be cached.</summary>
        DateTimeOffset? Expires { get; }

        DateTimeOffset? LastModified { get; }
    }
}