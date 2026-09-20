using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>ListObjectVersionsResponse</c> API.</summary>
public class ListObjectVersionsResponse : BaseResponse, IHasTruncated, IHasTruncatedExt, IHasBucketName
{
    /// <summary>Provides the <c>ListObjectVersionsResponse</c> API.</summary>
    public ListObjectVersionsResponse()
    {
        Versions = new List<S3Version>();
        DeleteMarkers = new List<S3DeleteMarker>();
        CommonPrefixes = new List<string>();
    }

    /// <summary>Provides the <c>KeyMarker</c> API.</summary>
    public string? KeyMarker { get; internal set; }

    /// <summary>Provides the <c>VersionIdMarker</c> API.</summary>
    public string? VersionIdMarker { get; internal set; }

    /// <summary>Provides the <c>NextKeyMarker</c> API.</summary>
    public string? NextKeyMarker { get; internal set; }

    /// <summary>Provides the <c>NextVersionIdMarker</c> API.</summary>
    public string? NextVersionIdMarker { get; internal set; }

    /// <summary>Provides the <c>Versions</c> API.</summary>
    public IList<S3Version> Versions { get; }

    /// <summary>Provides the <c>DeleteMarkers</c> API.</summary>
    public IList<S3DeleteMarker> DeleteMarkers { get; }

    /// <summary>Provides the <c>MaxKeys</c> API.</summary>
    public int MaxKeys { get; internal set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; internal set; } = null!;

    /// <summary>Provides the <c>IsTruncated</c> API.</summary>
    public bool IsTruncated { get; internal set; }

    /// <summary>Provides the <c>EncodingType</c> API.</summary>
    public EncodingType EncodingType { get; internal set; }

    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public string? Prefix { get; internal set; }

    /// <summary>Provides the <c>Delimiter</c> API.</summary>
    public string? Delimiter { get; internal set; }

    /// <summary>Provides the <c>CommonPrefixes</c> API.</summary>
    public IList<string> CommonPrefixes { get; }
}