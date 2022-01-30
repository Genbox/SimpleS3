using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

[PublicAPI]
public interface IHasResponseContent : IHasLastModified
{
    /// <summary>The MIME type of the content. For example, Content-Type: text/html; charset=utf-8</summary>
    string? ContentType { get; }

    /// <summary>Indicates if the content is expected to be displayed inline in the browser, that is, as a Web page or as part
    /// of a Web page, or as an attachment, that is downloaded and saved locally.</summary>
    string? ContentDisposition { get; }

    /// <summary>Used to compress the media-type. When present, its value indicates which encodings were applied to the
    /// entity-body.</summary>
    string? ContentEncoding { get; }

    /// <summary>Used to describe the language(s) intended for the audience, so that it allows a user to differentiate
    /// according to the users' own preferred language.</summary>
    string? ContentLanguage { get; }

    /// <summary>The range of bytes returned in the response. Only set if the response is partial.</summary>
    string? ContentRange { get; }

    /// <summary>When this has a value, it indicates the type of partial requests the server supports.</summary>
    string? AcceptRanges { get; }
}