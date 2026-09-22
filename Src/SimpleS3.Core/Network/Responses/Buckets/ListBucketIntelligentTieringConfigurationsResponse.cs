using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing Intelligent-Tiering configurations.</summary>
public sealed class ListBucketIntelligentTieringConfigurationsResponse : BaseResponse
{
    /// <summary>Intelligent-Tiering configurations.</summary>
    public IList<S3IntelligentTieringConfiguration> Configurations { get; } = new List<S3IntelligentTieringConfiguration>();

    /// <summary>Whether more results are available.</summary>
    public bool IsTruncated { get; internal set; }

    /// <summary>Token supplied in the request.</summary>
    public string? ContinuationToken { get; internal set; }

    /// <summary>Token to request the next page.</summary>
    public string? NextContinuationToken { get; internal set; }
}