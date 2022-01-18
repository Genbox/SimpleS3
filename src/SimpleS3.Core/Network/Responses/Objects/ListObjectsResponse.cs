using System.Collections.Generic;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class ListObjectsResponse : BaseResponse, IHasTruncated, IHasTruncatedExt, IHasRequestCharged, IHasBucketName
{
    public ListObjectsResponse()
    {
        Objects = new List<S3Object>();
        CommonPrefixes = new List<string>();
    }

    /// <summary>Name of the bucket.</summary>
    public string BucketName { get; internal set; }

    /// <summary>
    /// KeyCount is the number of keys returned with this request. KeyCount will always be less than equals to MaxKeys field. Say you ask for 50
    /// keys, your result will include less than equals 50 keys
    /// </summary>
    public int KeyCount { get; internal set; }

    /// <summary>Sets the maximum number of keys returned in the response. The response might contain fewer keys but will never contain more.</summary>
    public int MaxKeys { get; internal set; }

    /// <summary>If ContinuationToken was sent with the request, it is included in the response.</summary>
    public string ContinuationToken { get; internal set; }

    /// <summary>
    /// NextContinuationToken is sent when isTruncated is true which means there are more keys in the bucket that can be listed. The next list
    /// requests to Amazon S3 can be continued with this NextContinuationToken. NextContinuationToken is obfuscated and is not a real key
    /// </summary>
    public string NextContinuationToken { get; internal set; }

    /// <summary>If StartAfter was sent with the request, it is included in the response.</summary>
    public string StartAfter { get; internal set; }

    /// <summary>The list of objects</summary>
    public IList<S3Object> Objects { get; }

    public bool RequestCharged { get; internal set; }

    public bool IsTruncated { get; internal set; }
    public EncodingType EncodingType { get; internal set; }
    public string? Prefix { get; internal set; }
    public string? Delimiter { get; internal set; }
    public IList<string> CommonPrefixes { get; }
}