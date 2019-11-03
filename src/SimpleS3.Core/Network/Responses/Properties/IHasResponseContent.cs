using System;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    [PublicAPI]
    public interface IHasResponseContent
    {
        /// <summary>The MIME type of the content. For example, Content-Type: text/html; charset=utf-8</summary>
        string ContentType { get; }

        string ContentDisposition { get; }
        string ContentEncoding { get; }
        string ContentLanguage { get; }

        DateTimeOffset? LastModified { get; }
    }
}