using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Returns metadata about all versions of the objects in a bucket. You can also use request parameters as
/// selection criteria to return metadata about a subset of all the object versions.</summary>
public class ListObjectVersionsRequest : BaseRequest, IHasBucketName
{
    internal ListObjectVersionsRequest() : base(HttpMethodType.GET) {}

    public ListObjectVersionsRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>A delimiter is a character that you specify to group keys. All keys that contain the same string between the
    /// prefix and the first occurrence of the delimiter are grouped under a single result element in CommonPrefixes. These
    /// groups are counted as one result against the max-keys limitation. These keys are not returned elsewhere in the
    /// response.</summary>
    public string? Delimiter { get; set; }

    /// <summary>Requests Amazon S3 to encode the object keys in the response and specifies the encoding method to use. An
    /// object key may contain any Unicode character; however, XML 1.0 parser cannot parse some characters, such as characters
    /// with an ASCII value from 0 to 10. For characters that are not supported in XML 1.0, you can add this parameter to
    /// request that Amazon S3 encode the keys in the response.</summary>
    public EncodingType EncodingType { get; set; }

    /// <summary>Specifies the key to start with when listing objects in a bucket.</summary>
    public string? KeyMarker { get; set; }

    /// <summary>Sets the maximum number of keys returned in the response. By default the API returns up to 1,000 key names.
    /// The response might contain fewer keys but will never contain more. If additional keys satisfy the search criteria, but
    /// were not returned because max-keys was exceeded, the response contains <isTruncated>true</isTruncated>. To return the
    /// additional keys, see key-marker and version-id-marker.</summary>
    public int? MaxKeys { get; set; }

    /// <summary>Limits the response to keys that begin with the specified prefix. You can use prefixes to separate a bucket
    /// into different groupings of keys. You can think of using prefix to make groups in the same way you'd use a folder in a
    /// file system.</summary>
    public string? Prefix { get; set; }

    /// <summary>Specifies the object version you want to start listing from.</summary>
    public string? VersionIdMarker { get; set; }

    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }

    public override void Reset()
    {
        Delimiter = null;
        MaxKeys = null;
        EncodingType = EncodingType.Unknown;
        Prefix = null;
        KeyMarker = null;
        VersionIdMarker = null;

        base.Reset();
    }
}