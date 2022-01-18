using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

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

    /// <summary>A standard MIME type describing the format of the contents.</summary>
    ContentTypeBuilder ContentType { get; }
}