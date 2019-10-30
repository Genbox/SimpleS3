using System;
using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Properties
{
    [PublicAPI]
    public interface IHasContentProps
    {
        /// <summary>Specifies presentational information for the object.</summary>
        ContentDispositionBuilder ContentDisposition { get; }

        /// <summary>
        /// Specifies what content encodings have been applied to the object and thus what decoding mechanisms must be applied to obtain the media-type
        /// referenced by the Content-Type header field.
        /// </summary>
        ContentEncodingBuilder ContentEncoding { get; }

        /// <summary>
        /// 128-bit MD5 digest of the message (without the headers) according to RFC 1864. This header can be used as a message integrity check to
        /// verify that the data is the same data that was originally sent. Although it is optional, we recommend using the Content-MD5 mechanism as an
        /// end-to-end integrity check.
        /// </summary>
        byte[] ContentMd5 { get; set; }

        /// <summary>A standard MIME type describing the format of the contents.</summary>
        ContentTypeBuilder ContentType { get; }

        /// <summary>The date and time at which the object is no longer able to be cached.</summary>
        DateTimeOffset? Expires { get; set; }
    }
}