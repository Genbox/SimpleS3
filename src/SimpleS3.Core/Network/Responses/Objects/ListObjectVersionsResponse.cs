using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class ListObjectVersionsResponse : BaseResponse, IHasTruncated, IHasTruncatedExt, IHasBucketName
{
    public ListObjectVersionsResponse()
    {
        Versions = new List<S3Version>();
        DeleteMarkers = new List<S3DeleteMarker>();
        CommonPrefixes = new List<string>();
    }

    public string KeyMarker { get; internal set; }
    public string VersionIdMarker { get; internal set; }
    public string NextKeyMarker { get; internal set; }
    public string NextVersionIdMarker { get; internal set; }
    public IList<S3Version> Versions { get; }
    public IList<S3DeleteMarker> DeleteMarkers { get; }
    public string BucketName { get; internal set; }
    public int MaxKeys { get; internal set; }
    public bool IsTruncated { get; internal set; }
    public EncodingType EncodingType { get; internal set; }
    public string? Prefix { get; internal set; }
    public string? Delimiter { get; internal set; }
    public IList<string> CommonPrefixes { get; }
}