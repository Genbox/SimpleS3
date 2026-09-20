using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

/// <summary>Provides the <c>ListMultipartUploadsResponse</c> API.</summary>
public class ListMultipartUploadsResponse : BaseResponse, IHasTruncated, IHasTruncatedExt
{
    /// <summary>Provides the <c>ListMultipartUploadsResponse</c> API.</summary>
    public ListMultipartUploadsResponse()
    {
        Uploads = new List<S3Upload>();
        CommonPrefixes = new List<string>();
    }

    /// <summary>Name of the bucket to which the multipart upload was initiated.</summary>
    public string Bucket { get; internal set; } = null!;

    /// <summary>The key at or after which the listing began.</summary>
    public string? KeyMarker { get; internal set; }

    /// <summary>Upload ID after which listing began.</summary>
    public string? UploadIdMarker { get; internal set; }

    /// <summary>When a list is truncated, this element specifies the value that should be used for the key-marker request parameter in a subsequent request.</summary>
    public string? NextKeyMarker { get; internal set; }

    /// <summary>When a list is truncated, this element specifies the value that should be used for the upload-id-marker request parameter in a subsequent request.</summary>
    public string? NextUploadIdMarker { get; internal set; }

    /// <summary>Maximum number of multipart uploads that could have been included in the response.</summary>
    public int MaxUploads { get; internal set; }

    /// <summary>
    /// A list of all the uploads in this response. The uploads are sorted by key. If your application has initiated more than one multipart upload using the same object key,
    /// then uploads in the response are first sorted by key. Additionally, uploads are sorted in ascending order within each key by the upload initiation time.
    /// </summary>
    public IList<S3Upload> Uploads { get; }

    /// <summary>Provides the <c>EncodingType</c> API.</summary>
    public EncodingType EncodingType { get; internal set; }

    /// <summary>Provides the <c>IsTruncated</c> API.</summary>
    public bool IsTruncated { get; internal set; }

    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public string? Prefix { get; internal set; }

    /// <summary>Provides the <c>Delimiter</c> API.</summary>
    public string? Delimiter { get; internal set; }

    /// <summary>Provides the <c>CommonPrefixes</c> API.</summary>
    public IList<string>? CommonPrefixes { get; }
}